namespace RSClientWrapper.Core.Logger
{
    /// <summary>
    ///     A static <see cref="IAppLogger" />
    /// </summary>
    public static class LoggerFactory
    {
        public static IAppLogger Instance { get; }

        #region ctor

        /// <summary>
        ///     Create a new <see cref="ILogger" /> instance with the given properties
        /// </summary>
        /// <param name="logfile">The file to log to</param>
        /// <returns>An initialized <see cref="ILogger" /></returns>
        public static IAppLogger New(string logfile) => new MetaLogger(logfile);


        #endregion
    }
}