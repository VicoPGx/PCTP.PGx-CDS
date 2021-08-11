using System;

namespace System.App.Utility.Helpers
{
    public sealed class RandomNumberHelper
    {
        private static Random gen = new Random();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">The exclusive upper bound of the random number returned</param>
        /// <returns></returns>
        public static int GetRandomNumber(int min, int max)
        {
            return gen.Next(min,max);
        } 
    }
}
