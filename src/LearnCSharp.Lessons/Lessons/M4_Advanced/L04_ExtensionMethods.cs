using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.4.md
public sealed class L04_ExtensionMethods : LessonBase
{
    public override string Id => "4.4";
    public override string Title => "Extension methods: adding methods to types you do not own";

    public override void Run()
    {
        Section("Calling an extension method looks like a normal method");

        string sentence = "the quick brown fox";

        Out("sentence.WordCount()", sentence.WordCount());
        Out("sentence.ToTitleCase()", sentence.ToTitleCase());
        Out("sentence.Truncate(9)", sentence.Truncate(9));
        Out("\"racecar\".IsPalindrome()", "racecar".IsPalindrome());

        Note("string is sealed and lives in the .NET framework - you cannot inherit it or edit it. "
           + "Extension methods let you add behaviour anyway.");

        Section("It is really just a static method");

        Out("sentence.WordCount()", sentence.WordCount());
        Out("StringExtensions.WordCount(sentence)", StringExtensions.WordCount(sentence));
        Out("identical?", sentence.WordCount() == StringExtensions.WordCount(sentence));

        Section("Extending numbers");

        Out("7.IsPrime()", 7.IsPrime());
        Out("9.IsPrime()", 9.IsPrime());
        Out("5.Times() runs a lambda 5 times", string.Join(" ", 5.Times(i => i * i)));
        Out("42.ToBinary()", 42.ToBinary());
        Out("(-5).Clamp(0, 10)", (-5).Clamp(0, 10));

        Section("Extending your own interfaces");

        List<int> numbers = [4, 8, 15, 16, 23, 42];
        Out("numbers.Median()", numbers.Median());
        Out("numbers.SecondLargest()", numbers.SecondLargest());
        Out("numbers.Shuffle(seed: 1)", string.Join(", ", numbers.Shuffle(1)));

        Section("This is exactly how LINQ works");

        // Where, Select, OrderBy and friends are all extension methods on IEnumerable<T>,
        // defined in the static class System.Linq.Enumerable.
        Out("numbers.Where(...)", string.Join(", ", numbers.Where(n => n > 10)));
        Out("Enumerable.Where(numbers, ...)", string.Join(", ", Enumerable.Where(numbers, n => n > 10)));

        Section("Extensions can be chained");

        Out("chained", "  hello world  ".Trim().ToTitleCase().Truncate(8));

        Section("Rules");

        Out("must be in a static class", "and the method itself must be static");
        Out("first parameter takes 'this'", "public static int WordCount(this string text)");
        Out("needs the right 'using'", "extensions are found via the namespace they live in");
        Out("cannot access private state", "only the public surface of the type");
        Out("a real method always wins", "if the type has its own method with that name, it is used");

        Section("null and extension methods");

        string? nothing = null;
        // A real method call on null throws. An extension method receives the null and can cope.
        Out("nothing.IsNullOrBlank()", nothing.IsNullOrBlank());
        Out("\"   \".IsNullOrBlank()", "   ".IsNullOrBlank());
        Out("\"hi\".IsNullOrBlank()", "hi".IsNullOrBlank());
    }
}

public static class StringExtensions
{
    // 'this string text' is what makes it an extension of string.
    public static int WordCount(this string text) =>
        text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    public static string ToTitleCase(this string text) =>
        string.Join(' ', text.Split(' ').Select(w => w.Length == 0
            ? w
            : char.ToUpperInvariant(w[0]) + w[1..].ToLowerInvariant()));

    public static string Truncate(this string text, int maxLength) =>
        text.Length <= maxLength ? text : text[..maxLength] + "...";

    public static bool IsPalindrome(this string text)
    {
        string clean = new string(text.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        return clean.SequenceEqual(clean.Reverse());
    }

    // The parameter is nullable, so this can be called ON a null string safely.
    public static bool IsNullOrBlank(this string? text) => string.IsNullOrWhiteSpace(text);
}

public static class NumberExtensions
{
    public static bool IsPrime(this int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }

    public static IEnumerable<int> Times(this int count, Func<int, int> action)
    {
        for (int i = 1; i <= count; i++) yield return action(i);
    }

    public static string ToBinary(this int n) => Convert.ToString(n, 2);

    public static int Clamp(this int n, int min, int max) => Math.Clamp(n, min, max);
}

public static class SequenceExtensions
{
    public static double Median(this IEnumerable<int> source)
    {
        int[] sorted = source.OrderBy(n => n).ToArray();
        if (sorted.Length == 0) throw new InvalidOperationException("Empty sequence.");

        int middle = sorted.Length / 2;
        return sorted.Length % 2 == 1
            ? sorted[middle]
            : (sorted[middle - 1] + sorted[middle]) / 2.0;
    }

    public static int SecondLargest(this IEnumerable<int> source) =>
        source.Distinct().OrderByDescending(n => n).Skip(1).First();

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, int seed)
    {
        Random random = new(seed);
        return source.OrderBy(_ => random.Next());
    }
}
