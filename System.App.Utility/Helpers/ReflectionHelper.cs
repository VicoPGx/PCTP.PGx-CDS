using System;
using System.Linq.Expressions;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.IO;

namespace System.App.Utility.Helpers
{
    public static class ReflectionHelper
    {
        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }

        /// <summary>
        /// Find assembly from loaded assemblies by matching name, e.g. "System.App.NLP.dll".
        /// </summary>
        /// <param name="name">assembly name</param>
        /// <returns>null if not found</returns>
        public static Assembly FindAssembly(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var a = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => CultureInfo.CurrentCulture.CompareInfo.IndexOf(x.ManifestModule.FullyQualifiedName, name, CompareOptions.IgnoreCase) >= 0) ;

            // if assembly is not loaded, load it
            if(a==null)
            {
                var fullname = name;

                if (Path.IsPathRooted(name) == false)
                {
                    fullname = Path.Combine(WebServerUtility.ServerBinPath, name);
                }

                // first load by full path name
                try
                {
                    a = Assembly.LoadFrom(fullname);
                }
                catch 
                {
                    a = null;
                }

                if (a == null)
                {
                    // then load by only file name
                    try
                    {
                        a = Assembly.LoadFrom(name);
                    }
                    catch
                    {
                        a = null;
                    }
                }
            }
            
            return a;
        }

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.CurrentCultureIgnoreCase)).ToArray();
        }

        public static Type GetTypeInNamespace(Assembly assembly, string type, string nameSpace)
        {
            var types = ReflectionHelper.GetTypesInNamespace(assembly, nameSpace);
            return types.ToList().FirstOrDefault(x => x.FullName == type);            
        }

        public static Type GetTypeInAssembly(Assembly assembly, string type)
        {
            return assembly == null ? null : assembly.GetTypes().FirstOrDefault(t => t.FullName == type);
        }

        public static bool IsTypeImplementsInterface(string implementerName, string interfaceName)
        {
            var t1 = Type.GetType(implementerName);
            var t2 = Type.GetType(interfaceName);
            return t1 != null && t2 != null && t2.IsAssignableFrom(t1);
        }

        /// <summary>
        /// Judge whether a type implements an interface by Reflection
        /// </summary>
        /// <param name="implementerName">Derived class; The type that implements interface, e.g. "System.App.NLP.Algorithms.ConceptValueExtractor"</param>
        /// <param name="assemblyName1">The assembly where implementer type resides, e.g. "System.App.NLP.Algorithms.ConceptValueExtractor.dll"</param>
        /// <param name="interfaceName">The interface / base type, e.g. "System.App.NLP.Infrastructure.Interfaces.IExtractConcept"</param>
        /// <param name="assemblyName2">The assembly where interface / base type resides, e.g. "System.App.NLP.Infrastructure.dll"</param>
        /// <param name="log">whether log message</param>
        public static bool IsTypeImplementsInterface(string implementerName, string assemblyName1, string interfaceName, string assemblyName2, bool log = true)
        {
            var assembly1 = FindAssembly(assemblyName1);

            if (assembly1 == null)
            {
                if(log) Logger.Log("Assembly Not Found:" + assemblyName1 + Environment.NewLine);
            }

            var assembly2 = FindAssembly(assemblyName2);

            if (assembly2 == null)
            {
                if (log) Logger.Log("Assembly Not Found:" + assemblyName2 + Environment.NewLine);
            }

            if (assembly1 != null && assembly2 != null)
            {
                var t1 = assembly1.GetType(implementerName);
                var t2 = assembly2.GetType(interfaceName);

                if (t1 == null)
                {
                    if (log) Logger.Log("Type Not Found:" + implementerName + Environment.NewLine);
                    return false;
                }
                if (t2 == null)
                {
                    if (log) Logger.Log("Type Not Found:" + interfaceName + Environment.NewLine);
                    return false;
                }
                if (t2.IsAssignableFrom(t1) == false)
                {
                    if (log) Logger.Log("Assign failed " + implementerName + "," + assemblyName1 + "," + interfaceName + "," + assemblyName2 + Environment.NewLine);
                    return false;
                }

                return true;
                // return t1 != null && t2 != null && t2.IsAssignableFrom(t1);
            }
            return false;
        }

        public static dynamic CreateInstance(string assemblyPath, string typeName)
        {
            var assembly = FindAssembly(assemblyPath);
            if(assembly != null)
            {
                var type = GetTypeInAssembly(assembly, typeName);
                if(type!=null)
                {
                    dynamic obj = Activator.CreateInstance(type);
                    return obj;
                }
            }
            return null;
        }
    }
}