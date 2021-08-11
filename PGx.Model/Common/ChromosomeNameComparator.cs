using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGx.Model.Common
{
    public class ChromosomeNameComparator : IComparer<String>
    {
        private static  IComparer<String> sf_comparator = new ChromosomeNameComparator();

        /**
         * Gets an instance of this comparator.
         *
         * @return an instance of this comparator
         */
        public static IComparer<String> Comparator
        {
            get { return sf_comparator; }
        }



        public int Compare(String name1, String name2)
        {


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

            if (name1.StartsWith("chr"))
            {
                name1 = name1.Substring(3);
            }
            int name1Int, name2Int;
            Boolean isName1Numeric = int.TryParse(name1, out name1Int);
            if (name2.StartsWith("chr"))
            {
                name2 = name2.Substring(3);
            }
            Boolean isName2Numeric = int.TryParse(name2, out name2Int);

            // assume non-numeric is either X or Y
            if (isName1Numeric)
            {
                if (isName2Numeric)
                {
                     return (name1Int < name2Int) ? -1 : ((name1Int == name2Int) ? 0 : 1);
                     //return name1Int.CompareTo(name2Int);

                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (isName2Numeric)
                {
                    return 1;
                }
                else
                {
                    if (name1 == name2)
                    {
                        return 0;
                    }
                    else
                        return -1;
                    //return ObjectUtils.compare(name1, name2);
                }
            }
        }
    }
}
