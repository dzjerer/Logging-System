using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSystem
{
    public class FileLoggerQueue
    {
        private static readonly Dictionary<string, FileLoggerQueue> Instances = new Dictionary<string, FileLoggerQueue>();
        private static readonly object LogEntryQueueLock = new object();

        private readonly Queue<LogEntry> logEntryQueue = new Queue<LogEntry>();
        private readonly BackgroundWorker logger = new BackgroundWorker();


        public static FileLoggerQueue GetInstance(string filePath)
        {
            checked
            {
                if (!Instances.ContainsKey(filePath))
                {
                    Instances[filePath] = new FileLoggerQueue();

                    FileInfo file = new FileInfo(filePath);
                    if (file.Exists)
                    {
                        int logId = (int)(DateTime.UtcNow - new DateTime(2000, 1, 1)).TotalSeconds;
                        bool can = true;
                        int moreThan = 0;

                        while (can)
                        {
                            string text = string.Format("{0}{1}", file.Name, logId);
                            if (moreThan > 0) { text = string.Format("{0}_{1}", text, moreThan); }
                            FileInfo newFile = new FileInfo(Path.Combine(file.DirectoryName, text + file.Extension));
                            if (newFile.Exists) { moreThan++; }
                            else
                            {
                                File.Move(filePath, newFile.FullName);
                                can = false;
                            }
                        }
                    }
                }
                return Instances[filePath];
            }
        }


        private FileLoggerQueue()
        {
            logger.WorkerSupportsCancellation = false;
            logger.DoWork += LoggerDoWork;
        }


        internal void Enqueue(LogEntry entry)
        {
            object locker = LogEntryQueueLock;
            lock (locker)
            {
                logEntryQueue.Enqueue(entry);
                if (!logger.IsBusy) { logger.RunWorkerAsync(); }
            }
        }


        private void LoggerDoWork(object sender, DoWorkEventArgs args)
        {
            while (true)
            {
                bool existsWork = false;
                object locker = LogEntryQueueLock;
                LogEntry entry;

                lock (locker)
                {
                    if (logEntryQueue.Count <= 0) { break; }
                    if (logEntryQueue.Count > 1) { existsWork = true; }
                    entry = logEntryQueue.Dequeue();
                }
                ProcessLog(entry);
                if (existsWork)
                {
                    object locker2 = LogEntryQueueLock;
                    lock (locker2) { if (logEntryQueue.Count > 0) { continue; } }
                    break;
                }
            }
        }


        private static void ProcessLog(LogEntry entry)
        {
            StreamWriter writer = File.AppendText(entry.TargetFile);
            writer.WriteLine(string.Format("[{0:MM/dd/yyyy HH:mm:ss}] [{1}] {2}", entry.Timestamp, entry.Severity, entry.Message));
            writer.Close();
        }
    }
}
