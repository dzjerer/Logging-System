using LoggingSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggingTesterCmd
{
    public class Program
    {
        private static T4GLogger Logger;

        private static void Main(string[] args)
        {
            Logger = new T4GLogger("TestLogs", "TestLogFile", "[TestLog] ");
            Logger.CLog("Loaded!", ConsoleColor.Cyan);

            while (true)
            {
                Console.Write("\nEnter a any text: ");
                string param = Console.ReadLine();
                if (param == "00") { break; }

                Console.WriteLine("> 1 - [INFO]");
                Console.WriteLine("> 2 - [WARING]");
                Console.WriteLine("> 3 - [ERROR]\n");

                Console.Write("Enter a type loging: ");

                int pc = int.MaxValue;
                try { pc = int.Parse(Console.ReadLine()); }
                catch {  }
                

                switch (pc)
                {
                    case 1:
                        Logger.CLog(param);
                        break;
                    case 2:
                        Logger.CLogWarning(param);
                        break;
                    case 3:
                        Logger.CLogError(param);
                        break;
                    default:
                        Logger.CLogOfInColor("Wrong format!", new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue }, true);
                        break;
                }
            }


            Logger.CLogOfInColor("Hello World", new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue }, true);

            Console.ReadLine();
        }
    }
}
