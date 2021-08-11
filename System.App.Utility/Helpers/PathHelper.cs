using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.App.Utility.Helpers
{
    public static class PathHelper
    {
        /// <remarks>
        /// 3 possible methods can be considered:
        /// AppDomain.CurrentDomain.BaseDirectory ->
        /// D:\VicoSvc\WebRoot\KB\kms6\
        /// Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) ->
        /// file:\C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files\root\04e9da4e\dfa35f69
        /// Server.MapPath("~")
        /// D:\VicoSvc\WebRoot\KB\kms6\
        /// </remarks>
        public static string GetAspNetBinDir()
        { 
            return AppDomain.CurrentDomain.BaseDirectory + @"bin\";
        }
    }
}
