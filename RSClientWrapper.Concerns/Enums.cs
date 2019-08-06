using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSClientWrapper.Concerns
{
    public enum LogSeverity
    {
        /// <summary>
        ///     Indicating the log-message is for debugging
        /// </summary>
        Debug,

        /// <summary>
        ///     Indicating the log-message is some kind of
        ///     information or progress update
        /// </summary>
        Info,

        /// <summary>
        ///     Indicating the log-message is an ignorable
        ///     warning or usual exception
        /// </summary>
        Warning,

        /// <summary>
        ///     Indicating the log-message is a runtime-affecting
        ///     error or unexpected exception
        /// </summary>
        Error,

        /// <summary>
        ///     Indicating the log-message is an unexpected exception
        ///     which may prevent the application from continuing
        /// </summary>
        Critical
    }
}
