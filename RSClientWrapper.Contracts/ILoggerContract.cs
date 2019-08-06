using RSClientWrapper.Concerns;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RSClientWrapper.Contracts
{
    public interface ILoggerContract
    {
        void Log(string message);

        void Log(Exception ex);
        void Log(Exception ex, string info);
        void LogCritical(Exception ex);
        void LogDebug(string message);
        void LogWarning(string message);
        void LogApi(string title, string content);

        T LogCritical<T>(Func<T> func);
    }
}

/// <summary>
///     A MetaLog logger
/// </summary>
public interface IAppLogger : IDisposable
{
    /// <summary>
    ///     The <see cref="Encoding" /> this <see cref="ILogger" /> instance uses
    /// </summary>
    Encoding Encoding { get; set; }


    /// <summary>
    ///     The minimum <see cref="LogSeverity" /> to log by this Logger instance
    ///     (It is recommended to use higher values such as <see cref="LogSeverity.Error" />
    ///     for release builds)
    /// </summary>
    LogSeverity MinimumSeverity { get; set; }

    /// <summary>
    ///     Log a new message to the log stream
    /// </summary>
    /// <param name="severity">The <see cref="LogSeverity" /> of this message</param>
    /// <param name="message">The actual log-message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Log(LogSeverity severity, string message,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new <see cref="Exception" /> tree (up to most
    ///     inner <see cref="Exception" />) to the <see cref="Stream" />
    /// </summary>
    /// <param name="severity">The <see cref="LogSeverity" /> of this message</param>
    /// <param name="exception">An occured <see cref="Exception" /></param>
    /// <param name="indent">The amount of whitespaces to put before the Exception tree</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Log(LogSeverity severity, Exception exception, int indent = 2,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new message to the <see cref="Stream" /> async
    /// </summary>
    /// <param name="severity">The <see cref="LogSeverity" /> of this message</param>
    /// <param name="message">The actual log-message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    Task LogAsync(LogSeverity severity, string message,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new <see cref="Exception" /> tree (up to most
    ///     inner <see cref="Exception" />) to the <see cref="Stream" /> async
    /// </summary>
    /// <param name="severity">The <see cref="LogSeverity" /> of this message</param>
    /// <param name="exception">An occured <see cref="Exception" /></param>
    /// <param name="indent">The amount of whitespaces to put before the Exception tree</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    Task LogAsync(LogSeverity severity, Exception exception, int indent = 2,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new message to the <see cref="Stream" /> of severity <see cref="LogSeverity.Debug" />
    /// </summary>
    /// <param name="message">The actual log-message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Debug(string message,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new message to the <see cref="Stream" /> of severity <see cref="LogSeverity.Info" />
    /// </summary>
    /// <param name="message">The actual log-message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Info(string message,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new message to the <see cref="Stream" /> of severity <see cref="LogSeverity.Warning" />
    /// </summary>
    /// <param name="message">The actual log-message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Warning(string message,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new message to the <see cref="Stream" /> of severity <see cref="LogSeverity.Error" />
    /// </summary>
    /// <param name="message">The actual log-message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Error(string message,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new <see cref="Exception" /> tree (up to most
    ///     inner <see cref="Exception" />) to the <see cref="Stream" /> of severity <see cref="LogSeverity.Error" />
    /// </summary>
    /// <param name="exception">An occured <see cref="Exception" /></param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Error(Exception exception,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new message to the <see cref="Stream" /> of severity <see cref="LogSeverity.Critical" />
    /// </summary>
    /// <param name="message">The actual log-message</param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Critical(string message,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);

    /// <summary>
    ///     Log a new <see cref="Exception" /> tree (up to most
    ///     inner <see cref="Exception" />) to the <see cref="Stream" /> of severity <see cref="LogSeverity.Critical" />
    /// </summary>
    /// <param name="exception">An occured <see cref="Exception" /></param>
    /// <param name="callerMember">The calling member for this Log message</param>
    /// <param name="callerFile">The calling source file for this Log message</param>
    /// <param name="callerLine">The line number in the calling file for this Log message</param>
    void Critical(Exception exception,
        [CallerFilePath] string callerFile = null,
        [CallerMemberName] string callerMember = null,
        [CallerLineNumber] int callerLine = 0);
}
