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
    public static class ChineseNameGenerator
    {
        private static string GetLastName()
        {
            return ChineseNameHelper.LastNames.ElementAt(RandomNumberHelper.GetRandomNumber(0, ChineseNameHelper.LastNames.Count()));
        }
        private static string GetFirstName()
        {
            bool twoCharacters = RandomNumberHelper.GetRandomNumber(0, 2) > 0;
            if (twoCharacters)
                return ChineseNameHelper.FirstNames.ElementAt(RandomNumberHelper.GetRandomNumber(0, ChineseNameHelper.FirstNames.Count())) + ChineseNameHelper.FirstNames.ElementAt(RandomNumberHelper.GetRandomNumber(0, ChineseNameHelper.FirstNames.Count()));
            else
                return ChineseNameHelper.FirstNames.ElementAt(RandomNumberHelper.GetRandomNumber(0, ChineseNameHelper.FirstNames.Count()));
        }

        public static string GetChineseName()
        {
            return GetLastName() + GetFirstName();
        }
    }
}
