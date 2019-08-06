using RSClientWrapper.Concerns;
using RSClientWrapper.Concerns.API;
using RSClientWrapper.Contract;

namespace RSClientWrapper.Contracts
{
    public interface ITransactionAPIClient : IApiClient
    {
        IResponseConcern<T> PostWithCallback<T>(string url, string callbackUrl, IRequestConcern request = null) where T : new();

        IResponseConcern<T> PostWithCallback<T, ReqConcern>(string url, string callbackUrl, IRequestConcern<ReqConcern> request = null) where T : new() where ReqConcern : new();
    }
}
