using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TextAnalysis;

static class TextGeneratorTask
{
    public static string ContinuePhrase(
        Dictionary<string, string> nextWords,
        string phraseBeginning,
        int wordsCount)
    {
        var separators = new[] { '.', '!', '?' };
        var resultString = new StringBuilder(phraseBeginning);

        for (int i = 0; i < wordsCount; i++)
        {
            var phraseList = CreatePraseList(resultString.ToString().Split(' '));
            var lengthPhraseList = phraseList.Count;

            string newWord = GetElement(nextWords, phraseList, lengthPhraseList);

            if (newWord != null)
            {
                resultString.Append($" {newWord}");

                foreach (var separator in separators)
                    if (newWord.EndsWith(separator))
                        break;
            }
            else
                break;
        }
        return resultString.ToString();
    }

    public static List<string> CreatePraseList(string[] words)
    {
        var result = new List<string>();
        foreach (var word in words)
            result.Add(word);

        return result;
    }

    public static string GetElement(Dictionary<string, string> nextWords, List<string> phraseList, int lengthPhraseList)
    {
        if (lengthPhraseList > 1)
        {
            var lastWord = phraseList[lengthPhraseList - 1];
            var penultimateWord = phraseList[lengthPhraseList - 2];
            var lastTwoWords = $"{penultimateWord} {lastWord}";

            if (nextWords.ContainsKey(lastTwoWords))
                return nextWords[lastTwoWords];
            else if (nextWords.ContainsKey(lastWord))
                return nextWords[lastWord];

            return null;
        }
        else
        {
            var lastWord = phraseList[lengthPhraseList - 1];
            if (nextWords.ContainsKey(lastWord))
                return nextWords[lastWord];

            return null;
        }
    }
}