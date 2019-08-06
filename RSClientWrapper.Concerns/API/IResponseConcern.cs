namespace RSClientWrapper.Concerns.API
{
    public interface IResponseConcern : IConcern
    {
        bool IsSuccess { get; set; }

        string[] Errors { get; set; }
    }

    public interface IResponseConcern<T> : IResponseConcern
    {
        T Concern { get; set; }
    }
}
