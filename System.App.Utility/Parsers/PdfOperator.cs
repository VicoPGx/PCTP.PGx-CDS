using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.App.Utility.Parsers
{
    public class PdfOperator
    {
        /// <summary>
        /// Get file size
        /// </summary>
        public static long GetFileSize(string filepath)
        {
            var info = new FileInfo(filepath);
            return info == null ? -1 : info.Length;
        }

        /// <summary>
        /// Get MD5 check sum in Base64
        /// </summary>
        public static string GetMD5CheckSum(string filepath)
        {
            if (File.Exists(filepath) == false)
                return string.Empty;

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filepath))
                {
                    var bytes = md5.ComputeHash(stream);
                    return Convert.ToBase64String(bytes);                    
                }
            }
        }

        /// <summary>
        /// Get plain text content from PDF file
        /// </summary>
        public static string GetText(string filepath)
        {
            return FilterReader.GetText(filepath);
        }

        /// <summary>
        /// Get leading plain text content from PDF file
        /// </summary>
        public static string GetLeadingText(string filepath, int size)
        {
            if (File.Exists(filepath) == false)
                return string.Empty;

            if (size <=0)
                return string.Empty;

            var reader = new FilterReader(filepath);
            if (reader == null)
                return string.Empty;

            var s = reader.ReadToEnd();
            if (s == null || s.Length <= 0)
                return string.Empty;

            string content = s.Substring(0, Math.Min(s.Length, size));
            reader.Close();
            reader.Dispose();
            return content;
        }

        public static void ComparePdfFolders(string folder1, string folder2)
        {
            var md5 = MD5.Create();

            var dict1 = new Dictionary<string, FileAttr>();
            var dict2 = new Dictionary<string, FileAttr>();
            System.IO.Directory.EnumerateFiles(folder1).ToList().ForEach(x =>
            {
                try
                {
                    dict1[x] = new FileAttr(PdfOperator.GetFileSize(x), PdfOperator.GetMD5CheckSum(x), PdfOperator.GetLeadingText(x, 500));
                }
                catch
                {
                    System.Console.WriteLine(x);
                }
            });

            System.IO.Directory.EnumerateFiles(folder2).ToList().ForEach(x =>
            {
                try
                {
                    dict2[x] = new FileAttr(PdfOperator.GetFileSize(x), PdfOperator.GetMD5CheckSum(x), PdfOperator.GetLeadingText(x, 500));
                }
                catch
                {
                    System.Console.WriteLine(x);
                }
            });

            var only1 = new Dictionary<string, FileAttr>();
            var only2 = new Dictionary<string, FileAttr>();
            var common = new Dictionary<string, string>();
            var suspect = new List<KeyValuePair<string, string>>();
            foreach (var x in dict1)
            {
                foreach (var y in dict2)
                {
                    if (x.Value.CheckSum == y.Value.CheckSum || x.Value.Content == y.Value.Content)
                    {
                        common.Add(x.Key, y.Key);
                        break;
                    }
                    if (x.Value.Length == y.Value.Length)
                    {
                        suspect.Add(new KeyValuePair<string, string>(x.Key, y.Key));
                    }
                }
                only1.Add(x.Key, x.Value);
            }

            foreach (var x in dict2)
            {
                foreach (var y in dict1)
                {
                    if (x.Value.CheckSum == y.Value.CheckSum || x.Value.Content == y.Value.Content)
                    {
                        break;
                    }
                }
                only2.Add(x.Key, x.Value);
            }

            suspect = suspect.Distinct().ToList();

            System.Console.WriteLine("=== Summary ===");
            System.Console.WriteLine("common " + common.Count); // common files
            System.Console.WriteLine("only 1 " + only1.Count); // files only in folder 1
            System.Console.WriteLine("only 2 " + only2.Count); // files only in folder 2
            System.Console.WriteLine("suspect " + suspect.Count); // suspected files that have same size
        }

        public class FileAttr
        {
            public long Length { get; set; }
            public string CheckSum { get; set; }
            public string Content { get; set; }

            public FileAttr(long length, string checksum, string content)
            {
                this.Length = length;
                this.CheckSum = checksum;
                this.Content = content;
            }
        }
    }
}
