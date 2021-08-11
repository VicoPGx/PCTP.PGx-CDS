using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace System.App.Utility.Helpers
{
    public static class Logger
    {
        public static void Log(string filePath, string content)
        {
            File.AppendAllText(filePath, content);
        }

        public static void Log(string content)
        {
            File.AppendAllText(WebServerUtility.ServerBinPath + "\\" + GenerateLogFileName() + ".log", DateTime.Now.ToString("yyyyMMddHHmmss") + " - " + content);
        }

        public static string GenerateLogFileName()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }
    }
}