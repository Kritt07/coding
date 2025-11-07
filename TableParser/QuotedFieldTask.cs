using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace TableParser;

[TestFixture]
public class QuotedFieldTaskTests
{
    [TestCase("''", 0, "", 2)]
    [TestCase("'a'", 0, "a", 3)]
    [TestCase("\"a\"", 0, "a", 3)]
    [TestCase("'a b c'", 0, "a b c", 7)]
    [TestCase("\"a b c\"", 0, "a b c", 7)]
    [TestCase("'\"'", 0, "\"", 3)]
    [TestCase("\"'\"", 0, "'", 3)]
    [TestCase("'\\\\'", 0, "\\", 4)]
    [TestCase("\"\\\\\"", 0, "\\", 4)]
    [TestCase("'a\\\\b'", 0, "a\\b", 6)]
    [TestCase("\"a\\\\b\"", 0, "a\\b", 6)]
    [TestCase("'a\\\"b'", 0, "a\"b", 6)]
    [TestCase("\"a\\\'b\"", 0, "a\'b", 6)]
    [TestCase("'a'", 0, "a", 3)]
    [TestCase("\"a\"", 0, "a", 3)]
    [TestCase("'a", 0, "a", 2)]
    [TestCase("\"a", 0, "a", 2)]
    [TestCase("'a b c", 0, "a b c", 6)]
    [TestCase("\"a b c", 0, "a b c", 6)]
    [TestCase("abc 'def' ghi", 4, "def", 5)]
    [TestCase("abc \"def\" ghi", 4, "def", 5)]
    [TestCase("'a''b'", 0, "a", 3)]
    [TestCase("\"a\"\"b\"", 0, "a", 3)]
    [TestCase("'a\\\\'", 0, "a\\", 5)]
    [TestCase("\"a\\\\\"", 0, "a\\", 5)]
    [TestCase(@"some_text ""QF \"""" other_text", 10, "QF \"", 7)]
    public void Test(string line, int startIndex, string expectedValue, int expectedLength)
    {
        var actualToken = QuotedFieldTask.ReadQuotedField(line, startIndex);
        ClassicAssert.AreEqual(new Token(expectedValue, startIndex, expectedLength), actualToken);
    }
}

class QuotedFieldTask
{
    public static Token ReadQuotedField(string line, int startIndex)
    {
        var quoteChar = line[startIndex];
        var (value, length) = ParseQuotedContent(line, startIndex, quoteChar);
        return new Token(value, startIndex, length);
    }

    private static (string value, int length) ParseQuotedContent(string line, int startIndex, char quoteChar)
    {
        var parsingState = new ParsingState();
        var currentIndex = startIndex + 1;

        while (currentIndex < line.Length && !parsingState.IsFinished)
        {
            currentIndex = ProcessNextCharacter(line, currentIndex, quoteChar, parsingState);
        }

        return (parsingState.ValueBuilder.ToString(), parsingState.Length);
    }

    private static int ProcessNextCharacter(string line, int currentIndex, char quoteChar, ParsingState state)
    {
        var currentChar = line[currentIndex];
        
        if (currentChar == '\\')
        {
            return ProcessEscapeCharacter(line, currentIndex, state);
        }
        else if (currentChar == quoteChar)
        {
            return ProcessClosingQuote(currentIndex, state);
        }
        else
        {
            return ProcessRegularCharacter(currentChar, currentIndex, state);
        }
    }

    private static int ProcessEscapeCharacter(string line, int currentIndex, ParsingState state)
    {
        if (currentIndex + 1 < line.Length)
        {
            var nextChar = line[currentIndex + 1];
            state.ValueBuilder.Append(nextChar);
            state.IncrementLength(2);
            return currentIndex + 2;
        }
        else
        {
            state.ValueBuilder.Append('\\');
            state.IncrementLength(1);
            return currentIndex + 1;
        }
    }

    private static int ProcessClosingQuote(int currentIndex, ParsingState state)
    {
        state.IncrementLength(1);
        state.FinishParsing();
        return currentIndex + 1;
    }

    private static int ProcessRegularCharacter(char character, int currentIndex, ParsingState state)
    {
        state.ValueBuilder.Append(character);
        state.IncrementLength(1);
        return currentIndex + 1;
    }

    private class ParsingState
    {
        public System.Text.StringBuilder ValueBuilder { get; } = new System.Text.StringBuilder();
        public int Length { get; private set; } = 1;
        public bool IsFinished { get; private set; }

        public void IncrementLength(int increment)
        {
            Length += increment;
        }

        public void FinishParsing()
        {
            IsFinished = true;
        }
    }
}