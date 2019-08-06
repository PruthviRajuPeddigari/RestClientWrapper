using RSClientWrapper.Concern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSClientWrapper.Concerns.API
{
    public class BaseResponseConcern : IResponseConcern
    {
        public bool IsSuccess { get; set; }

        public string[] Errors { get; set; }
    }

    public class BaseResponseConcern<T> : BaseResponseConcern, IResponseConcern<T>
    {
        public T Concern { get; set; }
    }

    public class BaseResponseConcernList<T> : BaseResponseConcern
    {
        public T[] Concerns { get; set; }
    }

    public class FileResponseConcern : BaseResponseConcern
    {
        public byte[] RawBytes { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }
    }
}
