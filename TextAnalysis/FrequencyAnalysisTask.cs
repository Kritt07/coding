namespace TextAnalysis
{
    static class FrequencyAnalysisTask
    {
        public static Dictionary<string, string> GetMostFrequentNextWords(List<List<string>> text)
        {
            var ngramCounts = CollectNgramFrequencies(text);
            var result = BuildResultDictionary(ngramCounts);

            return BuildResultDictionary(ngramCounts);
        }

        private static Dictionary<string, Dictionary<string, int>> CollectNgramFrequencies(List<List<string>> text)
        {
            var ngramCounts = new Dictionary<string, Dictionary<string, int>>();

            foreach (var sentence in text)
            {
                ProcessBigrams(sentence, ngramCounts);
                ProcessTrigrams(sentence, ngramCounts);
            }

            return ngramCounts;
        }

        private static void ProcessBigrams(List<string> sentence, Dictionary<string, Dictionary<string, int>> ngramCounts)
        {
            for (int i = 0; i < sentence.Count - 1; i++)
            {
                string key = sentence[i];
                string next = sentence[i + 1];
                AddContinuation(ngramCounts, key, next);
            }
        }

        private static void ProcessTrigrams(List<string> sentence, Dictionary<string, Dictionary<string, int>> ngramCounts)
        {
            for (int i = 0; i < sentence.Count - 2; i++)
            {
                string key = $"{sentence[i]} {sentence[i + 1]}";
                string next = sentence[i + 2];
                AddContinuation(ngramCounts, key, next);
            }
        }

        private static void AddContinuation(
            Dictionary<string, Dictionary<string, int>> ngramCounts,
            string key,
            string next)
        {
            if (!ngramCounts.TryGetValue(key, out var continuations))
            {
                continuations = new Dictionary<string, int>();
                ngramCounts[key] = continuations;
            }

            continuations[next] = continuations.GetValueOrDefault(next, 0) + 1;
        }

        private static Dictionary<string, string> BuildResultDictionary(
            Dictionary<string, Dictionary<string, int>> ngramCounts)
        {
            var Random = new Random();
            var result = new Dictionary<string, string>();

            foreach (var kvp in ngramCounts)
            {
                string ngram = kvp.Key;
                var continuations = kvp.Value;

                int maxFreq = continuations.Values.Max();
                var bestCandidates = continuations
                    .Where(x => x.Value == maxFreq)
                    .Select(x => x.Key)
                    .ToList();

                var random = Random.Next(bestCandidates.Count);
                result[ngram] = bestCandidates[random];
            }

            return result;
        }
    }
}
// harry and ron were still in my opinion asked for the
// harry and ron were delighted when she asked shrilly