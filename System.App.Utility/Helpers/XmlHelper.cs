using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;

namespace System.App.Utility.Helpers
{
    public static class XmlHelper
    {
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string FormatXml(string unformatted)
        {
            if(string.IsNullOrEmpty(unformatted))
                return string.Empty;

            var sb = new StringBuilder();

            // We will use stringWriter to push the formated xml into our StringBuilder 
            using (StringWriter stringWriter = new StringWriter(sb))
            {
                // We will use the Formatting of our xmlTextWriter to provide our indentation. 
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(unformatted);
                        doc.WriteTo(xmlTextWriter);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return string.Empty;
                    }
                }
            }
            
            return sb.ToString();
        }

#if false
        public static void ParseLargeFile(string filepath)
        {
            using (XmlReader reader = new XmlTextReader(filepath))
            {
                // Initialize a list for storage of the items
                var items = new List<Dictionary<string, string>>();

                var item = new Dictionary<string, string>();

                // Loop the reader, till it cant read anymore
                while (reader.Read())
                {
                    // An object with the type Element was found.
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        // do sth.
                    }
                    // EndElement was found, check if it is named item, if it is, store the object in the list and set openItem to false.
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                    }
                }
            }
        }
#endif
    }
}