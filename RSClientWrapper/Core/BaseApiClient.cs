using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RSClientWrapper.Concerns;
using RSClientWrapper.Concerns.API;
using RSClientWrapper.Contract;
using RSClientWrapper.Contracts;
using RSClientWrapper.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace RSClientWrapper.Core
{
    public class BaseApiClient : IApiClient
    {

        protected RestClient restClient { get; set; }

        public string BaseUrl { get; }

        public ILoggerContract Logger { get; set; }

        public BaseApiClient(string baseUrl, ILoggerContract logger = null)
        {
            BaseUrl = baseUrl;
            restClient = new RestClient(baseUrl)
            {
                Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds
            };
            Logger = logger;
            if (Logger == null)
            {
                Logger = new DefaultLogger(string.Empty, string.Empty);//, null);
            }
        }

        public void AddHandler(string contentType, Func<IDeserializer> deserializerFactory)
        {
            restClient.AddHandler(contentType, deserializerFactory);
        }

        public void ChangeDefaultTimeout(int timeout)
        {
            restClient.Timeout = timeout;
        }


        protected void TimeoutCheck(IRestRequest request, IRestResponse response)
        {
            if (response.StatusCode == 0)
            {
                Log(this.restClient.BaseUrl, request, response);
            }
        }

        public virtual void BeforeRequest(IRestClient client, IRestRequest request)
        {

        }
        public virtual void AfterResponse(IRestClient client, IRestResponse request)
        {

        }
        protected virtual IResponseConcern Execute(IRestRequest request)
        {
            IRestResponse response = default(IRestResponse);
            IResponseConcern apiResponse = new BaseResponseConcern();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                BeforeRequest(restClient, request);
                Stopwatch watcher = Stopwatch.StartNew();
                response = restClient.Execute(request);
                watcher.Stop();
                Logger.Log($"Time Taken For api:{restClient.BaseUrl}{request.Resource} Time: {watcher.Elapsed.TotalSeconds} sec");
                AfterResponse(restClient, response);
                LogRestApi(BaseUrl, request, response);
                TimeoutCheck(request, response);
                apiResponse = new BaseResponseConcern();
                if (response.IsSuccessful)
                {
                    apiResponse.IsSuccess = true;
                }
                else
                {
                    apiResponse.IsSuccess = false;

                    if (!string.IsNullOrEmpty(response.Content))
                    {
                        apiResponse.Errors = new string[] { response.Content.GetErrorMessage() };
                    }
                    else
                    {
                        apiResponse.Errors = new string[] { response.ErrorMessage };
                    }
                }
            }
            catch
            {
                Log(restClient.BaseUrl, request, response);
            }

            return apiResponse;
        }

        public IResponseConcern<T> Execute<T>(IRestRequest request)
        {
            IResponseConcern<T> apiResponse = new BaseResponseConcern<T>();
            IRestResponse rawResponse = default(IRestResponse);

            try
            {

                BeforeRequest(restClient, request);
                Stopwatch watcher = Stopwatch.StartNew();
                try
                {
                    rawResponse = restClient.Execute(request);
                    watcher.Stop();
                    Logger.Log($"Time Taken For api:{restClient.BaseUrl}{request.Resource} Time: {watcher.Elapsed.TotalSeconds} sec");
                }
                catch (Exception ex)
                {
                    watcher.Stop();
                    Logger.Log($"Time Taken For api:{restClient.BaseUrl}{request.Resource} Time: {watcher.Elapsed.TotalSeconds} sec  with Errors");
                    Logger.Log(ex);
                }

                AfterResponse(restClient, rawResponse);
                watcher.Stop();
                LogRestApi(BaseUrl, request, rawResponse);
                TimeoutCheck(request, rawResponse);

                if (rawResponse.IsSuccessful)
                {
                    try
                    {
                        apiResponse.Concern = JsonConvert.DeserializeObject<T>(rawResponse.Content);

                    }
                    catch (Exception ex)
                    {
                        Logger.Log("******************Deserialization failed****************");
                        Logger.LogCritical(ex);
                        //TODO: tosolve the type mismatch issue but need to log message properly
                        apiResponse.Concern = JsonConvert.DeserializeObject<T>(rawResponse.Content, new JsonSerializerSettings()
                        {
                            Converters = new List<JsonConverter>() {
                                    new DefaultValueConverter()
                                }
                        });
                        apiResponse.Errors = new string[] { ex.Message };
                    }
                    apiResponse.IsSuccess = true;
                }
                else
                {
                    apiResponse.IsSuccess = false;

                    if (!string.IsNullOrEmpty(rawResponse.Content))
                    {
                        apiResponse.Errors = new string[] { rawResponse.Content.GetErrorMessage() };
                    }
                    else
                    {
                        apiResponse.Errors = new string[] { rawResponse.ErrorMessage };
                    }
                }
            }
            catch
            {
                Log(restClient.BaseUrl, request, rawResponse);
            }

            return apiResponse;
        }

        protected void Log(Uri baseUrl, IRestRequest request, IRestResponse response)
        {
            if (Logger == null)
                return;

            //Get the values of the parameters passed to the API

            StringBuilder sb = new StringBuilder($" Url :{baseUrl.AbsoluteUri}{request.Resource}");
            if (response != null && response.ErrorException != null)
            {
                Logger.Log(response.ErrorException, sb.ToString());
            }
            else
            {
                Logger.Log(sb.ToString());
            }

        }
        protected void LogRestApi(string url, IRestRequest baseRequest, IRestResponse baseResponse)
        {
            if (Logger == null)
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append($" Raw Url :{url}{baseRequest.Resource} Status:{baseResponse.ResponseStatus.ToString()} Code:{baseResponse.StatusCode}");
            if (baseResponse != null && baseResponse.ErrorException != null)
            {
                Logger.Log(baseResponse.ErrorException, sb.ToString());
            }


            Logger.LogApi(sb.ToString(), $@"{{
                                                    ""Url"":""{url}{baseRequest.Resource}"",
                                                    ""Request.Params"":{baseRequest?.Parameters.Where(_ => _.Type == ParameterType.QueryString).ToJson()},
                                                    ""Request.Body"":{JsonSerializer.Default.Serialize(baseRequest.Parameters)},
                                                    ""Reponse.Status"":""{baseResponse?.StatusCode.ToString()}"",
                                                    ""Response.RawBody"":{(string.IsNullOrWhiteSpace(baseResponse.Content) ? "\"\"" : baseResponse.Content)}
                                                }}");

        }
        protected void LogApi(string url, IRequestConcern request, IResponseConcern response, IRestRequest baseRequest = null, IRestResponse baseResponse = null)
        {
            if (ConfigurationManager.AppSettings["ENABLE:SERIALIZATIONLOG"] != null)
            {
                if (Logger == null)
                    return;

                StringBuilder sb = new StringBuilder();
                sb.Append($" Url :{url}{baseRequest.Resource} Status: {baseResponse?.ResponseStatus.ToString()} Code:{baseResponse?.StatusCode}");
                if (baseResponse != null && baseResponse.ErrorException != null)
                {
                    Logger.Log(baseResponse.ErrorException, sb.ToString());
                }
                Logger.LogApi(sb.ToString(), $@"{{
                                                ""Url"":""{url}{baseRequest.Resource}"",
                                                ""Request.Params"":{JsonSerializer.Default.Serialize(baseRequest?.Parameters.Where(_ => _.Type == ParameterType.QueryString))},
                                                ""Request.Body"":{JsonSerializer.Default.Serialize(request)},
                                                ""Reponse.Status"":""{baseResponse?.StatusCode.ToString()}  {response.IsSuccess.ToString()}"",
                                                ""Response.Body"":{response.ToJson()}
                                                }}");
            }

        }

        public virtual IResponseConcern Get(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.GET);
            if (request != null && request.QueryParams != null && request.QueryParams.Any())
            {
                foreach (var param in request.QueryParams)
                {
                    restRequest.AddQueryParameter(param.Key, param.Value);
                }
            }

            return Execute(restRequest);
        }

        public virtual IResponseConcern<T> Get<T>(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.GET);
            if (request != null && request.QueryParams.Any())
            {
                foreach (var param in request.QueryParams)
                {
                    restRequest.AddQueryParameter(param.Key, param.Value);
                }
            }

            return Execute<T>(restRequest);
        }
        private void AddJsonBody(RestRequest restRequest, IRequestConcern request)
        {
            try
            {
                var concernProp = request.GetType().GetProperty("Concern");
                if (concernProp != null)
                {
                    restRequest.AddJsonBody(concernProp.GetValue(request));
                }
                else
                {
                    restRequest.AddJsonBody(request);
                }
            }
            catch (Exception ex)
            {

            }

        }
        public virtual IResponseConcern Post(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.POST) { JsonSerializer = JsonSerializer.Default };
            if (request != null)
            {
                if (request.QueryParams != null && request.QueryParams.Any())
                {
                    foreach (var param in request.QueryParams)
                    {
                        restRequest.AddQueryParameter(param.Key, param.Value);
                    }
                }
                AddJsonBody(restRequest, request);
            }
            var response = Execute(restRequest);
            LogApi(BaseUrl, request, response, restRequest);
            return response;
        }

        public virtual IResponseConcern<T> Post<T>(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.POST) { JsonSerializer = JsonSerializer.Default, RequestFormat = DataFormat.Json };

            if (request != null)
            {
                if (request.QueryParams != null && request.QueryParams.Any())
                {
                    foreach (var param in request.QueryParams)
                    {
                        restRequest.AddQueryParameter(param.Key, param.Value);
                    }
                }
                AddJsonBody(restRequest, request);

            }

            var response = Execute<T>(restRequest);
            LogApi(BaseUrl, request, response, restRequest);
            return response;
        }

        public virtual IResponseConcern<T> Post<T, ReqConcern>(string url, IRequestConcern<ReqConcern> request = null)
            where ReqConcern : new()
        {
            RestRequest restRequest = new RestRequest(url, Method.POST) { JsonSerializer = JsonSerializer.Default, RequestFormat = DataFormat.Json };

            if (request != null)
            {
                if (request.QueryParams != null && request.QueryParams.Any())
                {
                    foreach (var param in request.QueryParams)
                    {
                        restRequest.AddQueryParameter(param.Key, param.Value);
                    }
                }
                //TO DO: Need to find better way to handle 
                if (request != null && request.Concern != null)
                {
                    restRequest.AddJsonBody(request.Concern);
                }

            }

            var response = Execute<T>(restRequest);
            LogApi(BaseUrl, request, response, restRequest);
            return response;
        }

        public virtual IResponseConcern Put(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.PUT) { JsonSerializer = JsonSerializer.Default, RequestFormat = DataFormat.Json };
            if (request != null)
            {
                if (request.QueryParams != null && request.QueryParams.Any())
                {
                    foreach (var param in request.QueryParams)
                    {
                        restRequest.AddQueryParameter(param.Key, param.Value);
                    }
                }
                restRequest.AddJsonBody(request);
            }

            var response = Execute(restRequest);
            LogApi(BaseUrl, request, response, restRequest);
            return response;
        }

        public virtual IResponseConcern<T> Put<T>(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.PUT) { JsonSerializer = JsonSerializer.Default };
            if (request != null)
            {
                if (request.QueryParams != null && request.QueryParams.Any())
                {
                    foreach (var param in request.QueryParams)
                    {
                        restRequest.AddQueryParameter(param.Key, param.Value);
                    }
                }
                AddJsonBody(restRequest, request);
            }

            var response = Execute<T>(restRequest);
            LogApi(BaseUrl, request, response, restRequest);
            return response;
        }

        public virtual IResponseConcern Delete(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.DELETE);
            if (request != null && request.QueryParams != null && request.QueryParams.Any())
            {
                foreach (var param in request.QueryParams)
                {
                    restRequest.AddQueryParameter(param.Key, param.Value);
                }
            }

            var response = Execute(restRequest);
            LogApi(BaseUrl, request, response, restRequest);
            return response;
        }

        public virtual IResponseConcern<T> Delete<T>(string url, IRequestConcern request = null)
        {
            RestRequest restRequest = new RestRequest(url, Method.DELETE) { JsonSerializer = JsonSerializer.Default, RequestFormat = DataFormat.Json };

            if (request != null)
            {
                if (request.QueryParams != null && request.QueryParams.Any())
                {
                    foreach (var param in request.QueryParams)
                    {
                        restRequest.AddQueryParameter(param.Key, param.Value);
                    }
                }
                AddJsonBody(restRequest, request);

            }

            var response = Execute<T>(restRequest);
            LogApi(BaseUrl, request, response, restRequest);
            return response;
        }
    }

    public class DefaultValueConverter : JsonConverter
    {
        readonly Newtonsoft.Json.JsonSerializer defaultSerializer = new Newtonsoft.Json.JsonSerializer();

        public override bool CanConvert(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(long) || type == typeof(ulong) || type == typeof(int) || type == typeof(uint)
                || type == typeof(short) || type == typeof(ushort) || type == typeof(byte) || type == typeof(sbyte)
                || type == typeof(float) || type == typeof(decimal) || type == typeof(double)
                )
                return true;
            return false;
        }

        public override object ReadJson(JsonReader reader, Type destinationType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            try
            {
                return defaultSerializer.Deserialize(reader, destinationType);
            }
            catch (Exception ex)
            {
                //ex has to logged and reported properly
                return GetDefaultValue(destinationType);
            }
        }
        object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
        public override bool CanWrite { get { return false; } }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //  base.WriteJson(writer, value, serializer);
        }
    }
}
