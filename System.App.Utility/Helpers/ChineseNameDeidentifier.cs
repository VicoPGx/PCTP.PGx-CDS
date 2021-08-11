using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.App.Utility.Helpers;

namespace System.App.Utility.Helpers
{
    /// <summary>
    /// Generate a random chinese name
    /// </summary>
    public static class ChineseNameDeidentifier
    {
        /// <summary>
        /// Deidentification or reverse-Deidentification of a Chinese name. 
        /// The process is reversible.
        /// </summary>        
        private static string Convert(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            string lastName = string.Empty;
            string firstName = string.Empty;
            ChineseNameHelper.Parse(s, out lastName, out firstName);

            int lastNamesCount = ChineseNameHelper.LastNames.Count();
            int firstNamesCount = ChineseNameHelper.FirstNames.Count();

            if (string.IsNullOrEmpty(lastName) == false)
            {
                int pos = Array.IndexOf(ChineseNameHelper.LastNames, lastName);
                if (pos > -1)
                {
                    // the array contains the string and the pos variable
                    lastName = ChineseNameHelper.LastNames.ElementAtOrDefault(lastNamesCount - pos - 1);
                }
            }

            if (string.IsNullOrEmpty(firstName) == false)
            {
                var reverseFirstName = string.Empty;
                foreach (char y in firstName)
                {
                    int pos = Array.IndexOf(ChineseNameHelper.FirstNames, y.ToString());
                    if (pos > -1)
                    {
                        reverseFirstName += ChineseNameHelper.FirstNames.ElementAtOrDefault(firstNamesCount - pos - 1);
                    }
                    else
                    {
                        reverseFirstName += y.ToString();
                    }
                }
                firstName = reverseFirstName;
            }

            var ret = lastName + firstName;
            //if (ret == s)
            //{
            //    ret = new string(s.Reverse().ToArray()); // this is not reversible
            //}
            return ret;
        }

        public static string Deidentify(string s)
        {
            return Convert(s);
        }

        public static string Identify(string s)
        {
            return Convert(s);
        }
    }
}
