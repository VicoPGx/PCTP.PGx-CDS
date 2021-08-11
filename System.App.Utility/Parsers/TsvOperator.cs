using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.App.Utility.Helpers;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace System.App.Utility.Parsers
{
    /// <summary>
    /// Parse Tab Separated Value file and get a list of object.
    /// Require the tsv has a header row, and the header names are the same with object properties
    /// </summary>
    public static class TsvOperator
    {
        public static IList<T> GetParsedObjects<T>(string filepath, out string result) where T : new()
        {
            if (File.Exists(filepath) == false)
            {
                result = "File Not Exist";
                return null;
            }

            var list = new List<T>();
            result = string.Empty;
            using (CsvReader csv = new CsvReader(new StreamReader(filepath), true, '\t'))
            {
                int fieldCount = csv.FieldCount;
                var headers = csv.GetFieldHeaders();
                
                while (csv.ReadNextRecord())
                {
                    try
                    {
                        var obj = new T();

                        foreach (var h in headers)
                        {
                            TypeHelper.SetPropertyStringValue(obj, h, csv[h]);
                        }

                        list.Add(obj);
                    }
                    catch
                    {
                        result += csv[0] + ";";
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Load a tsv file from embeded resource and parse its content into a dictionary object(require the tsv have 2 columns)
        /// </summary>
        /// <returns></returns>
        /// <remarks>Requires tsv has two columns</remarks>
        public static IDictionary<string, string> GetDict(string file, Assembly assembly)
        {
            var dict = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(file))
                return dict;

            // This class is usually called by others, so use CallingAssembly
            if (assembly == null)
                assembly = Assembly.GetCallingAssembly();

            string[] names = assembly.GetManifestResourceNames();
            foreach (string name in names)
            {
                if (name.Contains(file))
                {
                    String str = ResourceHelper.GetEmbededResourceFileAsString(name,assembly);

                    string[] lines = str.Replace("\r", "").Split('\n');
                    int lineCount = lines.Count();
                    char[] spliter = { '\t' };
                    for (int i = 0; i < lineCount; i++)
                    {
                        String str_Line = lines[i];
                        str_Line = str_Line.Replace(" ", "");
                        if (!str_Line.Equals(String.Empty))
                        {
                            string[] strResult = str_Line.Split(spliter);
                            dict.Add(strResult[0].Replace(" ", "").Trim(), strResult[1].Replace(" ", "").Trim());
                        }
                    }
                }
            }
            return dict;
        }
    }
}
