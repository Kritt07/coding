using System.Text;

namespace TextAnalysis;

static class SentencesParserTask
{
    public static List<List<string>> ParseSentences(string text)
    {
        var sentencesList = new List<List<string>>();
        var separators = new[] { '.', '!', '?', ';', ':', '(', ')' };
        
        // Разделяем по разделителям, но НЕ удаляем пустые — потому что они могут быть между разделителями
        var sentences = text.Split(separators, StringSplitOptions.None);

        foreach (var sentence in sentences)
        {
            var words = ParseWordList(sentence);
            if (words.Count > 0) // Только если есть хотя бы одно слово
                sentencesList.Add(words);
        }

        return sentencesList;
    }

    public static List<string> ParseWordList(string text)
    {
        if (string.IsNullOrEmpty(text))
            return new List<string>();

        var sb = new StringBuilder();

        foreach (char c in text)
        {
            if (char.IsLetter(c) || c == '\'')
                sb.Append(c);
            else
                sb.Append(' ');
        }

        string processedText = sb.ToString().ToLower().Trim();

        return processedText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}