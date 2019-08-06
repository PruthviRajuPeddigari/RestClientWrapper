using Newtonsoft.Json;
using RSClientWrapper.Concerns.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
