namespace Passwords;

public class CaseAlternatorTask
{
    public static List<string> AlternateCharCases(string lowercaseWord)
    {
        var result = new List<string>();
        AlternateCharCases(lowercaseWord.ToCharArray(), 0, result);
        return result;
    }

    static void AlternateCharCases(char[] word, int index, List<string> result)
	{
		if (index == word.Length)
		{
			result.Add(new string(word));
			return;
		}

		char original = word[index];
		AlternateCharCases(word, index + 1, result);

		// Пробуем изменить регистр, только если это даст другой символ
		if (char.IsLetter(original))
		{
			char toggled = char.IsLower(original) ? char.ToUpper(original) : char.ToLower(original);

			if (toggled != original)
			{
				word[index] = toggled;
				AlternateCharCases(word, index + 1, result);
				word[index] = original;
			}
		}
	}
}