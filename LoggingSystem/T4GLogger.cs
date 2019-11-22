using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingSystem
{
    public class T4GLogger : FileLogger
    {
        public readonly string Prefix;

        private static string GetLogFilePath(string subdirectory, string logFileName)
        {
            string text = Path.Combine("Logs", subdirectory);
            Directory.CreateDirectory(text);
            return Path.Combine(text, logFileName + ".log");
        }

        public T4GLogger(string subdirectory, string logFileName, string prefix = null) : base(GetLogFilePath(subdirectory, logFileName))
        {
            Prefix = prefix;
        }

        public void Log(string message)
        {
            Log(ELogType.INFO, PrefixMsg(message), null);
        }


        #region OldMethod
        private static int oldStartTopPos;
        private static int oldEndTopPos;

        internal static int SetStartTopPos()
        {
            return oldStartTopPos = Console.CursorTop;
        }
        internal static int SetEndTopPos()
        {
            return oldEndTopPos = Console.CursorTop;
        }

        internal static void CLogLoadPluginStart(string pluginName)
        {

        }
        internal static void CLogLoadPluginEnd(string pluginName)
        {

        }
        #endregion

        public void CLogLoadPlugin(string pluginName, Action baseLoadPlugin)
        {
            int oldStartTopPos = Console.CursorTop;
            baseLoadPlugin();
            int oldEndTopPos = Console.CursorTop;

            Console.SetCursorPosition(0, oldStartTopPos);

            CLogWithoutPrefix("\n[loading]", ConsoleColor.Cyan, false);
            CLogWithoutPrefix("-", ConsoleColor.DarkYellow, false);
            CLogOfInColor("|T4G|", new ConsoleColor[] { ConsoleColor.DarkGreen, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.DarkGreen }, false, false);
            CLogWithoutPrefix("-", ConsoleColor.DarkYellow, false);
            CLogWithoutPrefix("rocket", ConsoleColor.DarkCyan, false);
            CLogWithoutPrefix("-", ConsoleColor.DarkYellow, false);
            CLogWithoutPrefix("plugin", ConsoleColor.DarkCyan, false);
            CLogWithoutPrefix("-", ConsoleColor.DarkYellow, false);
            CLogWithoutPrefix("> ", ConsoleColor.DarkCyan, false);
            CLogWithoutPrefix(pluginName, ConsoleColor.Cyan, false);
            CLogWithoutPrefix(" <", ConsoleColor.DarkCyan, false);
            CLogWithoutPrefix("-------" + new string('-', 8), ConsoleColor.DarkYellow, false);
            CLogWithoutPrefix("<", ConsoleColor.Magenta, false);

            Console.SetCursorPosition(0, oldEndTopPos);

            CLogWithoutPrefix("--", ConsoleColor.DarkYellow, false);
            CLogWithoutPrefix("<end>", ConsoleColor.Magenta, false);
            CLogWithoutPrefix(new string('-', (32 + pluginName.Length) + 10), ConsoleColor.DarkYellow, false);
            CLogWithoutPrefix("<", ConsoleColor.Magenta);
        }




        public void CLog(string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
        {
            CLog(ELogType.INFO, message, color, newLine);
        }
        public void CLog(ELogType logType, string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
        {
            string text = PrefixMsg(message);
            Log(logType, text, null);
            T4GCLog(logType, text, color, newLine);
        }

        public void CLogWithoutPrefix(string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
        {
            CLogWithoutPrefix(ELogType.INFO, message, color, newLine);
        }
        public void CLogWithoutPrefix(ELogType logType, string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
        {
            Log(logType, message, null);
            T4GCLog(logType, message, color, newLine);
        }



        internal void LogWarning(string message)
        {
            Log(ELogType.WARNING, PrefixMsg(message), null);
        }
        public void CLogWarning(string message)
        {
            string text = PrefixMsg(message);
            Log(ELogType.WARNING, text, null);
            T4GCLog(ELogType.WARNING, text);
        }

        internal void LogError(string message)
        {
            Log(ELogType.ERROR, PrefixMsg(message), null);
        }
        public void CLogError(string message)
        {
            string text = PrefixMsg(message);
            Log(ELogType.ERROR, text, null);
            T4GCLog(ELogType.ERROR, text);
        }   

        internal void LogException(Exception ex, string message)
        {
            Log(ELogType.ERROR, TransformMessage(PrefixMsg(message), ex), null);
        }
        public void CLogException(Exception ex, string message)
        {
            string text = PrefixMsg(message);
            Log(ELogType.ERROR, text, ex);
            T4GCLog(ELogType.ERROR, TransformMessage(text, ex));
        }

        public void CLogOfInColor(string message, ConsoleColor[] colors, bool loopingColors = false, bool newLine = true)
        {
            if (colors != null && colors.Length != 0)
            {
                ConsoleColor oldColor = Console.ForegroundColor;

                int colorIndex = 0;
                char[] array = message.ToArray();

                if (loopingColors)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] == ' ') { Console.Write(" "); continue; }
                        if (colorIndex > colors.Length - 1) { colorIndex = 0; }
                        Console.ForegroundColor = colors[colorIndex++];
                        Console.Write(array[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] == ' ') { Console.Write(" "); continue; }
                        if (colorIndex > colors.Length - 1) { Console.ForegroundColor = oldColor; }
                        else { Console.ForegroundColor = colors[colorIndex++]; }
                        Console.Write(array[i]);
                    }
                }
                if (newLine) { Console.WriteLine(); }
                Console.ForegroundColor = oldColor;
                Log(message);
            }
        }

        public static ConsoleColor[] GetColorsFromColor(ConsoleColor color, int count)
        {
            ConsoleColor[] colors = new ConsoleColor[count];
            for (int i = 0; i < count; i++) { colors[i] = color; }
            return colors;
        }

        private static void T4GCLog(ELogType logType, string message, ConsoleColor color = ConsoleColor.White, bool newLine = true)
        {
            if (logType == ELogType.INFO)
            {
                WriteToConsole(message, color, newLine);
            }
            else if (logType == ELogType.WARNING)
            {
                WriteToConsole(message, ConsoleColor.Yellow, true);
            }
            else if (logType == ELogType.ERROR)
            {
                WriteToConsole(message, ConsoleColor.Red, true);
            }
        }

        private static void WriteToConsole(string message, ConsoleColor color, bool newLine = true)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            if (newLine) { Console.WriteLine(message); }
            else { Console.Write(message); }
            Console.ForegroundColor = oldColor;
        }

        private string PrefixMsg(string message)
        {
            if (!string.IsNullOrEmpty(Prefix)) { return Prefix + message; }
            return message;
        }
    }
}
