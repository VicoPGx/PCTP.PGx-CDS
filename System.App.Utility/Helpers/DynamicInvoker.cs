using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.App.Utility.Helpers
{
    public static class DynamicInvoker
    {
        /// <summary>
        /// Invoke a method of a class from a assembly
        /// </summary>
        /// <param name="path">full path name of the assembly file</param>
        /// <param name="type">class name</param>
        /// <param name="func">method name</param>
        /// <param name="args">arguments</param>
        /// <returns>return from invoking the method</returns>
        public static object Invoke(string path, string type, string func, object[] args)
        {
            var assembly = Assembly.LoadFile(path);
            if (assembly == null)
                return null;

            return Invoke(assembly, type, func, args);
        }

        public static object Invoke(Assembly assembly, string type, string func, object[] args)
        {
            try
            {
                var t = assembly.GetType(type);
                var obj = DynamicInitializer.NewInstance(t); // or Activator.CreateInstance(t);

                if (obj == null)
                {
                    return null;
                }

                // Alternately you could get the MethodInfo for the TestRunner.Run method
                return t.InvokeMember(func,
                    BindingFlags.Default | BindingFlags.InvokeMethod,
                    null,
                    obj,
                    args);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message + "  " + e.StackTrace + "   " + e.TargetSite);
                return null;
            }
        }
    }
}
