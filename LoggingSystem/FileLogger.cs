using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSystem
{
    public class FileLogger : ILogger
    {
        protected readonly string LogFilePath;
        protected readonly FileLoggerQueue FileLoggerQueue;

        public FileLogger(string logFilePath)
        {
            LogFilePath = logFilePath;
            FileLoggerQueue = FileLoggerQueue.GetInstance(logFilePath);
        }


        public virtual void Log(ELogType logType, string message, Exception exception = null)
        {
            FileLoggerQueue.Enqueue(new LogEntry(TransformMessage(message, exception), LogFilePath, logType));
        }



        protected static string TransformMessage(string message, Exception exception)
        {
            if (exception != null) { return string.Format("{0}\n{1}", message, exception); }
            return message;
        }

        public void LogInfo(string message, Exception exception = null)
        {
            Log(ELogType.INFO, message, exception);
        }
        public void LogWarning(string message, Exception exception)
        {
            Log(ELogType.WARNING, message, exception);
        }
        public void LogError(string message, Exception exception)
        {
            Log(ELogType.ERROR, message, exception);
        }
    }
}
