using RSClientWrapper.Concerns;
using RSClientWrapper.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSClientWrapper
{
    public class DefaultLogger : ILoggerContract
    {
        public IAppLogger AppLogger { get; set; }
        public string FolderPath { get; set; }
        public DefaultLogger(string folderPath, string name
            //, IApplicationContext applicatonContext
            )
        {
            try
            {
                //if (!(string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(name) || applicatonContext == null))
                //{
                //    if (!Directory.Exists($"{folderPath}/{applicatonContext.CodeName}/{applicatonContext.PortalCodeName}"))
                //    {
                //        if (!Directory.Exists($"{folderPath}/{applicatonContext.CodeName}"))
                //        {
                //            if (!Directory.Exists($"{folderPath}"))
                //            {
                //                Directory.CreateDirectory(folderPath);
                //            }
                //            Directory.CreateDirectory($"{folderPath}/{applicatonContext.CodeName}");
                //        }
                //        Directory.CreateDirectory($"{folderPath}/{applicatonContext.CodeName}/{applicatonContext.PortalCodeName}");
                //    }
                //    this.FolderPath = $"{folderPath}/{applicatonContext.CodeName}/{applicatonContext.PortalCodeName}/{name}_{DateTime.Now.ToString(Constants.LOGGERPOSTFIXDATEFORMAT)}/";
                //    this.AppLogger = LoggerFactory.New($"{folderPath}/{applicatonContext.CodeName}/{applicatonContext.PortalCodeName}/{name}_{DateTime.Now.ToString(Constants.LOGGERPOSTFIXDATEFORMAT)}.txt");
                //}
                if (!(string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(name)))
                {
                    if (!Directory.Exists($"{folderPath}"))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    this.FolderPath = $"{folderPath}/{name}_{DateTime.Now.ToString(Constants.LOGGERPOSTFIXDATEFORMAT)}/";
                    this.AppLogger = LoggerFactory.New($"{folderPath}/{name}_{DateTime.Now.ToString(Constants.LOGGERPOSTFIXDATEFORMAT)}.txt");
                }
            }
            catch { }

        }
        public void Log(string message)
        {
            if (this.AppLogger == null)
                return;
            Task.Run(() => { this.AppLogger.Log(Concerns.LogSeverity.Info, message); });
        }


        public void Log(Exception ex)
        {
            if (this.AppLogger == null)
                return;
            Task.Run(() => { this.AppLogger.Log(Concerns.LogSeverity.Error, ex); });
        }

        public void Log(Exception ex, string infoMessage)
        {
            if (this.AppLogger == null)
                return;
            Task.Run(() => { this.AppLogger.Log(Concerns.LogSeverity.Error, ex); });
        }
        public void LogCritical(Exception ex)
        {
            if (this.AppLogger == null)
                return;
            this.AppLogger.Critical(ex);
        }

        public void LogDebug(string message)
        {
            if (this.AppLogger == null)
                return;
            this.AppLogger.Debug(message);
        }

        public void LogWarning(string message)
        {
            if (this.AppLogger == null)
                return;
            this.AppLogger.Warning(message);
        }

        public void LogApi(string title, string content)
        {
            try
            {
                if (string.IsNullOrEmpty(this.FolderPath))
                    return;
                if (!Directory.Exists(this.FolderPath))
                {
                    Directory.CreateDirectory(this.FolderPath);
                }
                string logId = DateTime.Now.Ticks.ToString();
                this.Log($"LogId: {logId} , {title} ");
                using (StreamWriter sw = new StreamWriter($"{this.FolderPath}{logId}.json", true))
                {
                    sw.Write(content);
                }
            }
            catch
            {

            }
        }

        public T LogCritical<T>(Func<T> func)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                this.LogCritical(ex);
            }
            return default(T);
        }
    }
}
