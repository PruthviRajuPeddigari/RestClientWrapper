using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSClientWrapper.Concerns.Core
{
    public interface ITransactionContext
    {
        //ClientSession ClientSession { get; set; }

        //IClientContext ClientContext { get; set; }

        //string TxnKey { get; set; }

        //void SetTransactionSession(ClientSession transactionSession);

        //void Update(string[] sessionKeys);

        void SetCallbackUrl(string callbackUrl);
    }
}
