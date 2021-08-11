using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.App.Utility.Helpers
{

    /// <summary>
    /// 日志类
    /// </summary>
    public class Log
    {
 
 
        /// <summary>
        /// 输出日志记录（参数：记录消息）
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string file = "Log" + date + ".log";
            string path = folder + "\\" + file;
 
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
 
            string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
            sw.WriteLine();
 
            sw.WriteLine("- - " + NowTime + " - - - - - - - - - - -");
            sw.WriteLine("-");
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1); //1代表上级，2代表上上级，以此类推
            MethodBase method = frame.GetMethod();
            string methodName = method.Name;
            string className = method.ReflectedType.FullName;
            sw.WriteLine(className +"."+ methodName + "() 方法被执行。");
            sw.WriteLine("-");
            sw.WriteLine("记录消息：");
            sw.WriteLine(message);
            sw.WriteLine("-");
            sw.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - -");
 
            sw.WriteLine();
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
 
 
        /// <summary>
        /// 输出日志记录（参数：记录消息，错误异常信息）
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public static void WriteLog(string message, string error)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string file = "Log" + date + ".log";
            string path = folder + "\\" + file;
 
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
 
            string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
            sw.WriteLine();
 
            sw.WriteLine("- - " + NowTime + " - - - - - - - - - - -");
            sw.WriteLine("-");
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1); //1代表上级，2代表上上级，以此类推
            MethodBase method = frame.GetMethod();
            string methodName = method.Name;
            string className = method.ReflectedType.FullName;
            sw.WriteLine(className + "." + methodName + "() 方法被执行。");
            sw.WriteLine("-");
            sw.WriteLine("记录消息：");
            sw.WriteLine(message);
            sw.WriteLine("-");
            sw.WriteLine("错误异常：");
            sw.WriteLine(error);
            sw.WriteLine("-");
            sw.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - -");
 
            sw.WriteLine();
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
 
 
        /// <summary>
        /// 输出日志记录（参数：记录消息，错误异常信息，执行的SQL语句）
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        /// <param name="strSql"></param>
        public static void WriteLog(string message, string error, string strSql)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string file = "Log" + date + ".log";
            string path = folder + "\\" + file;
 
            FileStream fs = new FileStream(path, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
 
            string NowTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffff");
            sw.WriteLine();
 
            sw.WriteLine("- - " + NowTime + " - - - - - - - - - - -");
            sw.WriteLine("-");
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1); //1代表上级，2代表上上级，以此类推
            MethodBase method = frame.GetMethod();
            string methodName = method.Name;
            string className = method.ReflectedType.FullName;
            sw.WriteLine(className + "." + methodName + "() 方法被执行。");
            sw.WriteLine("-");
            sw.WriteLine("记录消息：");
            sw.WriteLine(message);
            sw.WriteLine("-");
            sw.WriteLine("错误异常：");
            sw.WriteLine(error);
            sw.WriteLine("-");
            sw.WriteLine("执行SQL：");
            sw.WriteLine(strSql);
            sw.WriteLine("-");
            sw.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - -");
 
            sw.WriteLine();
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}
