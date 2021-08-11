using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace System.App.Utility.Helpers
{
    /// <summary>
    /// System.Collections.Generic.KeyValuePair cannot be serialized to xml.
    /// Define this struct to replace KeyValuePair.
    /// </summary>    
    [Serializable]
    public struct Pair<T1,T2>
    {
        public T1 Object1 { get; set; }
        public T2 Object2 { get; set; }

        public Pair(T1 o1, T2 o2)
            : this()
        {
            Object1 = o1;
            Object2 = o2;
        }
    }

    public static class SerializationHelper
    {
        /// <summary>
        /// Deserialize an xml string into a typed object
        /// </summary>
        public static Object Deserialize(Type type, String serializedString)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (string.IsNullOrEmpty(serializedString))
                throw new ArgumentNullException("serializedString");
            using(MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedString)))
            {
                return new DataContractSerializer(type).ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize a typed object into an xml string
        /// </summary>
        public static String Serialize(ref Object obj)
        {
            DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                String serializedString = Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Position);
                return serializedString;
            }
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void Serialize<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            var xmlDocument = new XmlDocument();
            var serializer = new XmlSerializer(serializableObject.GetType());
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, serializableObject);
                stream.Position = 0;
                xmlDocument.Load(stream);
                xmlDocument.Save(fileName);
                stream.Close();
            }
        }

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="fileName">xml file path name</param>
        /// <param name="result">
        /// returned message. can be any of:
        /// "file name is empty", 
        /// "file does not exist",
        /// "parsing file failed",
        /// "parsing file success".
        /// </param>
        /// <returns>an object of type T</returns>        
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static T Deserialize<T>(string fileName, out string result)
        {
            if (string.IsNullOrEmpty(fileName)) 
            {
                result = "file name is empty";
                return default(T); 
            }

            if (File.Exists(fileName) == false)
            {
                result = string.Format("file \"{0}\" does not exist",fileName);
                return default(T); 
            }

            T objectOut = default(T);
            string attributeXml = string.Empty;
            try
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (var read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);
                   
                    var serializer = new XmlSerializer(outType);
                    using (var reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                    }
                }
            }
            catch
            {
                result = string.Format("parsing file \"{0}\" failed", fileName);
                return default(T); 
            }

            result = string.Format("parsing file \"{0}\" success", fileName);
            return objectOut;
        }
    }
}
