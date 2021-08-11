using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
namespace PGx.Model.Common
{

    public class ChromosomePositionComparator : IComparer<String>
    {
        public static IComparer<String> sf_comparator = new ChromosomePositionComparator();
        private static String sf_pattern = "(?:chr)?(\\w{1,2}):(\\d+)";

        /**
         * Gets an instance of this comparator.
         *
         * @return an instance of this comparator
         */
        public static IComparer<String> getComparator()
        {
            return sf_comparator;
        }



        public int Compare(String o1, String o2)
        {

            //noinspection StringEquality
            if (o1 == o2)
            {
                return 0;
            }
            if (o1 == null)
            {
                return -1;
            }
            else if (o2 == null)
            {
                return 1;
            }
            var reg = new Regex(sf_pattern);
            var m1 = reg.Match(o1);
            //Matcher m1 = sf_pattern.matcher(o1);
            //if (!m1.matches()) {
            if (!reg.IsMatch(o1))
            {
                throw new ArgumentException("'" + o1 + "' is not in the expected chromosomal position format");
            }
            var m2 = reg.Match(o2);
            //Matcher m2 = sf_pattern.matcher(o2);
            if (!reg.IsMatch(o2))
            {
                throw new ArgumentException("'" + o2 + "' is not in the expected chromosomal position format");
            }

            int rez = new ChromosomeNameComparator().Compare(m1.Groups[1].Value, m2.Groups[1].Value);
            if (rez != 0)
            {
                return rez;
            }
            int n1 = int.Parse(m1.Groups[2].Value);
            int n2 = int.Parse(m2.Groups[2].Value);
            return IntCompare(n1, n2);
        }
        int IntCompare(int x, int y)
        {
            return (x < y) ? -1 : ((x == y) ? 0 : 1);
        }
    }
}
