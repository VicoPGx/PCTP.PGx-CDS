using System.Drawing;

namespace System.App.Utility.Helpers
{
    public static class TextEncodingHelper
    {
        /// <summary>
        /// Judge whether a string is encoded in Chinese
        /// </summary>
        public static bool IsChineseString(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            for (int i = 0; i < s.Length; i++)
            {
                var seg = s.Substring(i, 1);

                byte[] sarr = System.Text.Encoding.GetEncoding("gb2312").GetBytes(seg);

                if (sarr.Length == 2)
                {
                    return true;
                }
            }
            return false;
        }        
    }
}