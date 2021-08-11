using System;

namespace System.App.Utility.Helpers
{
    public class StringSimilarityHelper
    {
        public static int ComputeLevenshteinDistance
(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public const float SimilarityThreshold = 0.8f;
        
        public static bool IsSimilar(String s1, String s2)
        {
            if (String.IsNullOrEmpty(s1) == true || 
                String.IsNullOrEmpty(s2) == true)
                return false;

            if (s1.Contains(s2) || s2.Contains(s1))
                return true;

            if (s1.Length >= 3 && s2.Contains(s1.Substring(0, s1.Length - 1)) || 
                s2.Length >= 3 && s1.Contains(s2.Substring(0, s2.Length - 1)))
                return true;

            return false;

            // MatchsMaker matcher = new MatchsMaker(s1, s2);
            // return matcher.Score > SimilarityThreshold;
        }
    }
}
