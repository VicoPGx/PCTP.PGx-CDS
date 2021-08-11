using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGx.Model.Common.Util
{
    public class CompareUtils
    {
        public static int Compare(int ? x, int ? y)
        {
            return (x < y) ? -1 : ((x == y) ? 0 : 1);
        }
    }
}
