using System;
using System.IO;
using System.Reflection;
using System.Windows.Resources;
using System.Windows;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace System.App.Utility.Helpers
{
    public sealed partial class ResourceHelper
    {
        /// <summary>
        /// Get all keys in a resx
        /// </summary>
        /// <param name="targetType">A resx auto-generated resource class</param>
        /// <returns></returns>
        public static List<string> GetKeys(Type targetType)
        {
            List<string> keys = new List<string>();

            if (targetType != null)
            {
                try
                {
                    PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    foreach (PropertyInfo property in properties)
                    {
                        if(property.PropertyType== typeof(string))
                        keys.Add(property.Name);
                    }
                    keys.Sort(); 
                }
                catch { }
            }

            return keys;
        }

        /// <summary>
        /// Get all keys and values in a resx
        /// </summary>
        /// <param name="targetType">A resx auto-generated resource class</param>
        /// <returns></returns>
        public static Dictionary<string,string> GetKeyValuePairs(Type targetType)
        {
            var dict = new Dictionary<string, string>();

            if (targetType != null)
            {
                try
                {
                    PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    foreach (PropertyInfo property in properties)
                    {
                        if (property.PropertyType == typeof(string))
                            dict.Add(property.Name, property.GetValue(null, null) as string);
                    }
                }
                catch { }
            }

            return dict;
        }

        /// <summary>
        /// Get a string value from resx by key
        /// </summary>
        /// <param name="targetType">A resx auto-generated resource class</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        public static string GetValue(Type targetType, string key) 
        {
            if (targetType == null || string.IsNullOrEmpty(key))
                return string.Empty;

            try
            {
                PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string) && property.Name == key)
                        return property.GetValue(null, null) as string;
                }
            }
            catch {  }
            return string.Empty;
        }

        public static string[] GetKeys(Type targetType, string value)
        {
            var keys = new List<string>();
            if (targetType == null || string.IsNullOrEmpty(value))
                return keys.ToArray();

            try
            {
                PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo property in properties)
                {
                    if (property.PropertyType == typeof(string) && property.GetValue(null, null) as string == value)
                        keys.Add(property.Name);
                }
            }
            catch { }
            return keys.ToArray();
        }

     //   public static String GetResourceFileAsString(String relativeUri)
     //   {
     //       if (String.IsNullOrEmpty(relativeUri) == true)
     //           return String.Empty;

     //       StreamResourceInfo sr = Application.GetResourceStream(
     //new Uri(relativeUri, UriKind.Relative));
     //       Byte[] buffer = new Byte[sr.Stream.Length];
     //       sr.Stream.Read(buffer, 0, (int)sr.Stream.Length);
     //       return Encoding.UTF8.GetString(buffer, 0, buffer.GetLength(0));
     //   }

        public static String GetEmbededResourceFileAsString(String uri,Assembly assembly=null)
        {
            if (String.IsNullOrEmpty(uri) == true)
                return String.Empty;

            if (assembly == null)
                assembly = Assembly.GetExecutingAssembly();

            // After moving this class to System.App.Utility,  Assembly.GetExecutingAssembly() no longer worked as expected. 
            Stream stream = assembly.GetManifestResourceStream(uri);
            if (stream == null)
            {
                assembly = Assembly.GetCallingAssembly();
                stream = assembly.GetManifestResourceStream(uri);
            }
            if(stream != null)
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }

            return String.Empty;
        }
    }
}
