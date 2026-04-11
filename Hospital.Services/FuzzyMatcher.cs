using System;
using System.Linq;

namespace Hospital.Services
{
    public static class FuzzyMatcher
    {
        // Computes the Levenshtein distance representing syntactic similarity
        public static int ComputeLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source)) return string.IsNullOrEmpty(target) ? 0 : target.Length;
            if (string.IsNullOrEmpty(target)) return source.Length;

            int sourceLength = source.Length;
            int targetLength = target.Length;
            int[,] distance = new int[sourceLength + 1, targetLength + 1];

            for (int i = 0; i <= sourceLength; distance[i, 0] = i++) { }
            for (int j = 0; j <= targetLength; distance[0, j] = j++) { }

            for (int i = 1; i <= sourceLength; i++)
            {
                for (int j = 1; j <= targetLength; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost);
                }
            }
            return distance[sourceLength, targetLength];
        }

        // Checks if a query string fuzzily matches a target database entity string
        public static bool IsMatch(string query, string targetEntity, int maxDistance = 2)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(targetEntity)) return false;

            query = query.ToLowerInvariant();
            targetEntity = targetEntity.ToLowerInvariant();
            
            // Fast clear for direct matches
            if (targetEntity.Contains(query) || query.Contains(targetEntity)) return true;

            var queryWords = query.Split(new[] { ' ', ',', '.', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var targetWords = targetEntity.Split(new[] { ' ', ',', '.', '-' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var qw in queryWords)
            {
                if (qw.Length < 4) continue; // Ignore tiny connective words to prevent false positives

                foreach (var tw in targetWords)
                {
                    if (tw.Length < 4) continue;

                    if (ComputeLevenshteinDistance(qw, tw) <= maxDistance)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
