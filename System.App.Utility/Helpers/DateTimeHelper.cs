using System.Drawing;
using System.IO;

namespace System.App.Utility.Helpers
{
    public static class DateTimeHelper
    {
        /// <summary>
        /// Get 1st day of month
        /// </summary>
        public static DateTime GetMonthStart(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        /// <summary>
        /// Get 1st day of next month
        /// </summary>
        public static DateTime GetNextMonthStart(DateTime dt)
        {
            return GetMonthStart(dt).AddMonths(1);
            //return GetMonthEnd(dt).AddDays(1);
        }

        /// <summary>
        /// Get last day of month
        /// </summary>
        public static DateTime GetMonthEnd(DateTime dt)
        {
            return GetNextMonthStart(dt).AddDays(-1);
        }

        /// <summary>
        /// Get month differences between two datetimes
        /// </summary>
        public static int GetMonthDiff(DateTime start,DateTime end)
        {
            return (end.Year - start.Year) * 12 + end.Month - start.Month;
        }


        /// <summary>
        /// Get the correct LocalDateTime of files no matter which date settings your computer has
        /// </summary>
        /// <param name="filename">full file path name</param>
        /// <returns>last modified time</returns>
        public static DateTime GetExplorerFileDate(string filename)
        {
            DateTime now = DateTime.Now;
            TimeSpan localOffset = now - now.ToUniversalTime();
            return File.GetLastWriteTimeUtc(filename) + localOffset;
        }
    }
}