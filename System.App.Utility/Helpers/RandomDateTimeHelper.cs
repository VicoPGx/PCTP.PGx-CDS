using System;

namespace System.App.Utility.Helpers
{
    public sealed class RandomDateTimeHelper
    {
        private static DateTime start = new DateTime(1986, 3, 28);
        private static Random gen = new Random();

        /// <summary>
        /// Get random date time between start and now
        /// </summary>
        /// <returns></returns>
        public static DateTime GetRandomDate()
        {
            int range = ((TimeSpan)(DateTime.Today - start)).Days;
            return start.AddDays(gen.Next(range));
        }

        /// <summary>
        /// Get random date time between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static DateTime GetRandomDate(DateTime min, DateTime max)
        {
            int range = ((TimeSpan)(max - min)).Days;
            return min.AddDays(gen.Next(range));
        } 
    }
}
