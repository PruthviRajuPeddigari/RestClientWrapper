using RSClientWrapper.Concerns;
using RSClientWrapper.Concerns.API;
using RSClientWrapper.Concerns.Core;
using RSClientWrapper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSClientWrapper.Core
{
    public class BaseTransactionAPIClient : BaseApiClient, ITransactionAPIClient
    {
        public ITransactionContext TransactionContext { get; set; }

        public BaseTransactionAPIClient(string baseUrl, ITransactionContext transactionContext) : base(baseUrl)
        {
            TransactionContext = transactionContext;
        }

        public IResponseConcern<T> PostWithCallback<T>(string url, string callbackUrl, IRequestConcern request = null) where T : new()
        {
            this.TransactionContext?.SetCallbackUrl(callbackUrl);
            return base.Post<T>(url, request);
        }

        public IResponseConcern<T> PostWithCallback<T, ReqConcern>(string url, string callbackUrl, IRequestConcern<ReqConcern> request = null)
            where T : new()
            where ReqConcern : new()
        {
            this.TransactionContext?.SetCallbackUrl(callbackUrl);
            return base.Post<T, ReqConcern>(url, request);
        }
    }
}
