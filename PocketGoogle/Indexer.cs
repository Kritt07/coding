using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Metadata;

namespace PocketGoogle;

public class Indexer : IIndexer
{
	private HashSet<char> separators = new HashSet<char> {' ', '.', ',', '!', '?', ':', '-','\r','\n' };
	private Dictionary<string, Dictionary<int, List<int>>> wordToDocPositions = new();
	private Dictionary<int, string> docIdToText = new();

	public void Add(int id, string documentText)
	{
		docIdToText[id] = documentText;
		ParseDocument(documentText, (word, position) => AddWordToIndex(word, id, position));
	}

	private void AddWordToIndex(string word, int id, int position)
	{
		if (string.IsNullOrEmpty(word))
			return;

		if (!wordToDocPositions.ContainsKey(word))
			wordToDocPositions[word] = new Dictionary<int, List<int>>();
		
		if (!wordToDocPositions[word].ContainsKey(id))
			wordToDocPositions[word][id] = new List<int>();
		
		wordToDocPositions[word][id].Add(position);
	}

	private void ParseDocument(string documentText, Action<string, int> onWordFound)
	{
		int wordStart = -1;
		
		for (int i = 0; i <= documentText.Length; i++)
		{
			bool isSeparator = i == documentText.Length || separators.Contains(documentText[i]);
			
			if (wordStart != -1 && isSeparator)
			{
				string word = documentText.Substring(wordStart, i - wordStart);
				onWordFound(word, wordStart);
				wordStart = -1;
			}
			else if (wordStart == -1 && !isSeparator)
				wordStart = i;
		}
	}

	public List<int> GetIds(string word)
	{
		if (!wordToDocPositions.ContainsKey(word))
			return new List<int>();
		return new List<int>(wordToDocPositions[word].Keys);
	}

	public List<int> GetPositions(int id, string word)
	{
		if (!wordToDocPositions.ContainsKey(word))
			return new List<int>();
		
		if (!wordToDocPositions[word].ContainsKey(id))
			return new List<int>();
		
		return new List<int>(wordToDocPositions[word][id]);
	}

	public void Remove(int id)
	{
		if (!docIdToText.ContainsKey(id))
			return;
		
		string documentText = docIdToText[id];
		docIdToText.Remove(id);
		ParseDocument(documentText, (word, position) => RemoveWordFromIndex(word, id));
	}

	private void RemoveWordFromIndex(string word, int id)
	{
		if (string.IsNullOrEmpty(word) || !wordToDocPositions.ContainsKey(word))
			return;

		wordToDocPositions[word].Remove(id);
		
		if (wordToDocPositions[word].Count == 0)
			wordToDocPositions.Remove(word);
	}
}