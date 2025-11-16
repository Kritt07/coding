using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Autocomplete;

internal class AutocompleteTask
{
	/// <returns>
	/// Возвращает первую фразу словаря, начинающуюся с prefix.
	/// </returns>
	/// <remarks>
	/// Эта функция уже реализована, она заработает, 
	/// как только вы выполните задачу в файле LeftBorderTask
	/// </remarks>
	public static string FindFirstByPrefix(IReadOnlyList<string> phrases, string prefix)
	{
		var index = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) + 1;
		if (index < phrases.Count && phrases[index].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
			return phrases[index];
            
		return null;
	}

	/// <returns>
	/// Возвращает первые в лексикографическом порядке count (или меньше, если их меньше count) 
	/// элементов словаря, начинающихся с prefix.
	/// </returns>
	/// <remarks>Эта функция должна работать за O(log(n) + count)</remarks>
	public static string[] GetTopByPrefix(IReadOnlyList<string> phrases, string prefix, int count)
	{
		// Находим начальный индекс фраз, начинающихся с prefix
		var startIndex = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) + 1;
		
		// Если индекс выходит за границы или нет подходящих фраз, возвращаем пустой массив
		if (startIndex >= phrases.Count || 
			!phrases[startIndex].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
			return new string[0];

		var result = new List<string>();
		
		// Проходим по фразам, начиная с startIndex, пока они начинаются с prefix
		for (int i = 0; i < count && startIndex + i < phrases.Count; i++)
		{
			var currentPhrase = phrases[startIndex + i];
			
			// Проверяем, что фраза начинается с prefix
			if (currentPhrase.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
				result.Add(currentPhrase);
			else
				break; // Фразы отсортированы, поэтому можно прерваться
		}
		
		return result.ToArray();
	}

	/// <returns>
	/// Возвращает количество фраз, начинающихся с заданного префикса
	/// </returns>
	public static int GetCountByPrefix(IReadOnlyList<string> phrases, string prefix)
	{
		var leftIndex = LeftBorderTask.GetLeftBorderIndex(phrases, prefix, -1, phrases.Count) + 1;
		var rightIndex = RightBorderTask.GetRightBorderIndex(phrases, prefix, -1, phrases.Count);
		
		return Math.Max(0, rightIndex - leftIndex);
	}
}

public class AutocompleteTests
{
    private IReadOnlyList<string> testPhrases;

    [SetUp]
    public void SetUp()
    {
        testPhrases = new List<string>
        {
            "apple",
            "application",
            "banana",
            "band",
            "bandage",
            "cat",
            "category",
            "dog",
            "elephant"
        };
    }

    [Test]
    public void TopByPrefix_IsEmpty_WhenNoPhrases()
    {
        var emptyPhrases = new List<string>();
        var result = AutocompleteTask.GetTopByPrefix(emptyPhrases, "a", 3);
        
        CollectionAssert.IsEmpty(result);
    }

    [Test]
    public void TopByPrefix_ReturnsCorrectCount_WhenEnoughPhrases()
    {
        var result = AutocompleteTask.GetTopByPrefix(testPhrases, "ban", 2);
        
        ClassicAssert.AreEqual(2, result.Length);
        ClassicAssert.AreEqual("banana", result[0]);
        ClassicAssert.AreEqual("band", result[1]);
    }

    [Test]
    public void TopByPrefix_ReturnsAllAvailable_WhenNotEnoughPhrases()
    {
        var result = AutocompleteTask.GetTopByPrefix(testPhrases, "ban", 10);
        
        ClassicAssert.AreEqual(3, result.Length);
        ClassicAssert.AreEqual("banana", result[0]);
        ClassicAssert.AreEqual("band", result[1]);
        ClassicAssert.AreEqual("bandage", result[2]);
    }

    [Test]
    public void TopByPrefix_ReturnsEmpty_WhenNoMatches()
    {
        var result = AutocompleteTask.GetTopByPrefix(testPhrases, "xyz", 3);
        
        CollectionAssert.IsEmpty(result);
    }

    [Test]
    public void TopByPrefix_IsCaseInsensitive()
    {
        var result = AutocompleteTask.GetTopByPrefix(testPhrases, "BAN", 2);
        
        ClassicAssert.AreEqual(2, result.Length);
        ClassicAssert.AreEqual("banana", result[0]);
        ClassicAssert.AreEqual("band", result[1]);
    }

    [Test]
    public void CountByPrefix_IsTotalCount_WhenEmptyPrefix()
    {
        var result = AutocompleteTask.GetCountByPrefix(testPhrases, "");
        
        ClassicAssert.AreEqual(testPhrases.Count, result);
    }

    [Test]
    public void CountByPrefix_ReturnsZero_WhenNoMatches()
    {
        var result = AutocompleteTask.GetCountByPrefix(testPhrases, "xyz");
        
        ClassicAssert.AreEqual(0, result);
    }

    [Test]
    public void CountByPrefix_ReturnsCorrectCount_ForExistingPrefix()
    {
        var result = AutocompleteTask.GetCountByPrefix(testPhrases, "ban");
        
        ClassicAssert.AreEqual(3, result);
    }

    [Test]
    public void CountByPrefix_ReturnsOne_ForSingleMatch()
    {
        var result = AutocompleteTask.GetCountByPrefix(testPhrases, "ele");
        
        ClassicAssert.AreEqual(1, result);
    }

    [Test]
    public void CountByPrefix_IsCaseInsensitive()
    {
        var result = AutocompleteTask.GetCountByPrefix(testPhrases, "BAN");
        
        ClassicAssert.AreEqual(3, result);
    }

    [Test]
    public void CountByPrefix_ReturnsZero_ForEmptyPhrases()
    {
        var emptyPhrases = new List<string>();
        var result = AutocompleteTask.GetCountByPrefix(emptyPhrases, "a");
        
        ClassicAssert.AreEqual(0, result);
    }

    [Test]
    public void FindFirstByPrefix_ReturnsFirstMatch()
    {
        var result = AutocompleteTask.FindFirstByPrefix(testPhrases, "ban");
        
        ClassicAssert.AreEqual("banana", result);
    }

    [Test]
    public void FindFirstByPrefix_ReturnsNull_WhenNoMatches()
    {
        var result = AutocompleteTask.FindFirstByPrefix(testPhrases, "xyz");
        
        ClassicAssert.IsNull(result);
    }

    [Test]
    public void FindFirstByPrefix_IsCaseInsensitive()
    {
        var result = AutocompleteTask.FindFirstByPrefix(testPhrases, "BAN");
        
        ClassicAssert.AreEqual("banana", result);
    }

    [Test]
    public void FindFirstByPrefix_ReturnsNull_ForEmptyPhrases()
    {
        var emptyPhrases = new List<string>();
        var result = AutocompleteTask.FindFirstByPrefix(emptyPhrases, "a");
        
        ClassicAssert.IsNull(result);
    }

    [Test]
    public void GetTopByPrefix_HandlesCountZero()
    {
        var result = AutocompleteTask.GetTopByPrefix(testPhrases, "ban", 0);
        
        CollectionAssert.IsEmpty(result);
    }

    [Test]
    public void GetTopByPrefix_ReturnsInLexicographicalOrder()
    {
        var phrases = new List<string> { "beta", "alpha", "gamma", "alpha2" };
        var sortedPhrases = phrases.OrderBy(p => p, StringComparer.InvariantCultureIgnoreCase).ToList();
        
        var result = AutocompleteTask.GetTopByPrefix(sortedPhrases, "a", 3);
        
        ClassicAssert.AreEqual("alpha", result[0]);
        ClassicAssert.AreEqual("alpha2", result[1]);
        ClassicAssert.AreEqual("beta", result[2]);
    }
}