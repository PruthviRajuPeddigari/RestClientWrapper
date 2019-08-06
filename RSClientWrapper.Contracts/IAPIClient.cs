using RSClientWrapper.Concerns;
using RSClientWrapper.Concerns.API;

namespace RSClientWrapper.Contract
{
    public interface IApiClient
    {
        string BaseUrl { get; }

        IResponseConcern Get(string url, IRequestConcern request = null);

        IResponseConcern<T> Get<T>(string url, IRequestConcern request = null);

        IResponseConcern Post(string url, IRequestConcern request = null);

        IResponseConcern<T> Post<T>(string url, IRequestConcern request = null);

        IResponseConcern<T> Post<T, ReqConcern>(string url, IRequestConcern<ReqConcern> request = null) where ReqConcern : new();

        IResponseConcern Put(string url, IRequestConcern request = null);

        IResponseConcern<T> Put<T>(string url, IRequestConcern request = null);

        IResponseConcern Delete(string url, IRequestConcern request = null);

        IResponseConcern<T> Delete<T>(string url, IRequestConcern request = null);
    }
}
