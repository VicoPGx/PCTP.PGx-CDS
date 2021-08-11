using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace PGx.Model.Common
{
    public class HaplotypeNameComparator : IComparer<String>
    {
        private static String sf_starPattern = "(\\D*)(\\d+)(.*)";
        private static IComparer<String> sf_comparator = new HaplotypeNameComparator();
        private static List<String> sf_topTerms = new List<string>() { "Any", "All" };
        private static List<String> sf_bottomTerms = new List<string>() { "Other", "Unknown" };

        /**
         * Gets an instance of this comparator.
         *
         * @return an instance of this comparator
         */
        public static IComparer<String> getComparator()
        {
            return sf_comparator;
        }



        public int Compare(String name1, String name2)
        {

            //noinspection StringEquality
            if (name1 == name2)
            {
                return 0;
            }
            if (name1 == null)
            {
                return -1;
            }
            else if (name2 == null)
            {
                return 1;
            }

            if (sf_topTerms.Contains(name1) || sf_bottomTerms.Contains(name2))
            {
                return -1;
            }
            if (sf_topTerms.Contains(name2) || sf_bottomTerms.Contains(name1))
            {
                return 1;
            }
            Match matcher1 = Regex.Match(name1, sf_starPattern);
            Match matcher2 = Regex.Match(name2, sf_starPattern);
            //Matcher matcher1 = sf_starPattern.matcher(name1);
            //Matcher matcher2 = sf_starPattern.matcher(name2);
            if (Regex.IsMatch(name1, sf_starPattern) && Regex.IsMatch(name2, sf_starPattern))
            {
                String prePortion1 = matcher1.Groups[1].Value.Trim();
                String prePortion2 = matcher2.Groups[1].Value.Trim();
                int rez = string.Compare(prePortion1, prePortion2);
                if (rez != 0)
                {
                    return rez;
                }

                String starPortion1 = matcher1.Groups[2].Value;
                String starPortion2 = matcher2.Groups[2].Value;
                int star1 = Convert.ToInt32(starPortion1);
                int star2 = Convert.ToInt32(starPortion2);
                rez = (star1 < star2) ? -1 : ((star1 == star2) ? 0 : 1);
                if (rez != 0)
                {
                    return rez;
                }

                String restPortion1 = matcher1.Groups[3].Value.Trim();
                String restPortion2 = matcher2.Groups[3].Value.Trim();
                return string.Compare(restPortion1, restPortion2);
            }
            return string.Compare(name1, name2);
        }
    }
}
