using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    class GameLogger
    {
        public static void SimpleConsoleWriteLine(string message,
                                           [CallerMemberName] string callingMethod = "",
                                           [CallerFilePath] string callingFilePath = "",
                                           [CallerLineNumber] int callingFileLineNumber = 0)
        {
            string msg = string.Format("[HMW_LOG] message : {0}, Method : {1}, FilePath : {2}, LineNum : {3}", message, callingMethod, callingFilePath, callingFileLineNumber);
            Console.WriteLine(msg);
        }

        public static void SimpleConsoleWriteLineNoFileInfo(string message)
        {
            string msg = string.Format("[HMW_LOG] message : {0}", message);
            Console.WriteLine(msg);
        }
    }
}
