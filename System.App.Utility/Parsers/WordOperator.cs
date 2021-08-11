using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.App.Utility.Parsers
{
    public class WordOperator
    {
        /// <summary>
        /// Extract Text From a word document
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <remarks>This method is not tested yet</remarks>
        public static string ExtractTextFromFile(string path)
        {
            Microsoft.Office.Interop.Word._Application word = new Microsoft.Office.Interop.Word.Application();
            object missing = System.Reflection.Missing.Value;
            object readOnly = true;
            object obj = path;
            Microsoft.Office.Interop.Word._Document docs = word.Documents.Open(obj, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
            string totaltext = "";
            for (int i = 0; i < docs.Paragraphs.Count; i++)
            {
                totaltext += " \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString();
            }
            Console.WriteLine(totaltext);
            docs.Close();
            word.Quit();
            return totaltext;
        }
    }
}
