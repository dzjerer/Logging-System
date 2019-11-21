using System;

namespace LoggingSystem
{
    public class LogEntry
    {
        public ELogType Severity { get; }
        public string Message { get; }
        public DateTime Timestamp { get; }
        public string TargetFile { get; }


        public LogEntry(string message, string targetFile, ELogType severity = ELogType.INFO)
        {
            Severity = severity;
            Message = message;
            TargetFile = targetFile;
            Timestamp = DateTime.Now;
        }
    }
}
