using System.Text;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.8.md
public sealed class L08_Strings : LessonBase
{
    public override string Id => "1.8";
    public override string Title => "Strings and text handling";

    public override void Run()
    {
        Section("Strings are immutable");

        string original = "hello";
        string shouted = original.ToUpper();     // returns a NEW string

        Out("original", original);
        Out("original.ToUpper() returned", shouted);
        Out("original is still", original);

        Warn("Calling original.ToUpper(); on its own line changes nothing. Assign the result.");

        Section("Indexing and slicing");

        string word = "Computer";
        Out("word.Length", word.Length);
        Out("word[0]", word[0]);
        Out("word[^1]", word[^1]);
        Out("word[0..4]", word[0..4]);
        // word[2] = 'x';  <- will not compile: strings are read-only

        Section("Searching");

        string sentence = "The quick brown fox jumps over the lazy dog";
        Out("Contains(\"brown\")", sentence.Contains("brown"));
        Out("StartsWith(\"The\")", sentence.StartsWith("The"));
        Out("IndexOf(\"fox\")", sentence.IndexOf("fox"));
        Out("IndexOf(\"cat\")  not found", sentence.IndexOf("cat"));

        Note("IndexOf returns -1 when it finds nothing. Check for -1 before using it as an index.");

        Section("Transforming (each returns a new string)");

        Out("Replace(\"lazy\", \"sleepy\")", sentence.Replace("lazy", "sleepy"));
        Out("Substring(4, 5)", sentence.Substring(4, 5));
        Out("\"42\".PadLeft(8, '.')", "42".PadLeft(8, '.'));
        Out("\"  hi  \".Trim() + |", "  hi  ".Trim() + "|");

        Section("Split and Join");

        string[] parts = "Ada,Alan,Grace".Split(',');
        Out("Split(',')", string.Join(" | ", parts));
        Out("string.Join(\" and \", parts)", string.Join(" and ", parts));

        string messy = "one,,two,  three ,";
        Out("messy input", messy);
        Out("with RemoveEmptyEntries | TrimEntries", string.Join(" | ",
            messy.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)));

        string[] words = sentence.Split(' ');
        Out("word count", words.Length);

        Section("Comparing");

        Out("\"abc\" == \"abc\"", "abc" == "abc");
        Out("\"ABC\" == \"abc\"", "ABC" == "abc");
        Out("Equals with OrdinalIgnoreCase", "ABC".Equals("abc", StringComparison.OrdinalIgnoreCase));

        Section("Empty, null and whitespace");

        Out("IsNullOrEmpty(\"\")", string.IsNullOrEmpty(""));
        Out("IsNullOrEmpty(\"   \")", string.IsNullOrEmpty("   "));
        Out("IsNullOrWhiteSpace(\"   \")", string.IsNullOrWhiteSpace("   "));

        Section("Escapes, verbatim and raw strings");

        Line("Tab\there, and a \"quoted\" word.");
        Out("@\"C:\\Users\\Ada\"", @"C:\Users\Ada");

        string json = """
            { "name": "Ada", "age": 36 }
            """;
        Out("raw string literal", json);

        Section("Why StringBuilder exists");

        const int iterations = 20000;
        System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();

        string slow = "";
        for (int i = 0; i < iterations; i++) slow += "x";     // builds 20,000 throwaway strings
        long slowMs = watch.ElapsedMilliseconds;

        watch.Restart();
        StringBuilder builder = new();
        for (int i = 0; i < iterations; i++) builder.Append('x');   // one growable buffer
        string fast = builder.ToString();

        Out($"string += x{iterations}", $"{slowMs} ms");
        Out($"StringBuilder x{iterations}", $"{watch.ElapsedMilliseconds} ms");
        Out("same result?", slow == fast);

        Section("char helpers");

        Out("char.IsDigit('7')", char.IsDigit('7'));
        Out("char.IsLetter('a')", char.IsLetter('a'));
        Out("char.ToUpper('a')", char.ToUpper('a'));
        Out("(int)'A'", (int)'A');

        int vowels = 0;
        foreach (char c in sentence) if ("aeiouAEIOU".Contains(c)) vowels++;
        Out("vowels in the sentence", vowels);

        Section("Palindrome check");

        foreach (string candidate in new[] { "racecar", "A man, a plan, a canal: Panama", "hello" })
            Out($"IsPalindrome(\"{candidate}\")", IsPalindrome(candidate));
    }

    private static bool IsPalindrome(string text)
    {
        string clean = new string(text.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        for (int i = 0, j = clean.Length - 1; i < j; i++, j--)
            if (clean[i] != clean[j]) return false;
        return true;
    }
}
