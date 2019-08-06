using RSClientWrapper.Concerns.Params;
using System.Collections.Generic;

namespace RSClientWrapper.Concerns
{
    public interface IRequestConcern : IConcern
    {
        List<IRequestParam> QueryParams { get; set; }
    }

    public interface IRequestConcern<T> : IRequestConcern where T : new()
    {
        T Concern { get; set; }
    }
}
