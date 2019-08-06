using Newtonsoft.Json;
using RSClientWrapper.Concerns.API;

namespace RSClientWrapper.Extensions
{
    public static class Extensions
    {
        public static string GetErrorMessage(this string responseContent)
        {
            return JsonConvert.DeserializeObject<IErrorConcern>(responseContent).ToString();
        }
    }
}
