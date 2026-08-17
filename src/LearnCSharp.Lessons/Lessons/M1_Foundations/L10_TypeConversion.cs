using System.Globalization;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.10.md
public sealed class L10_TypeConversion : LessonBase
{
    public override string Id => "1.10";
    public override string Title => "Type conversion, casting, Math and Random";

    public override void Run()
    {
        Section("Implicit conversion - always safe, automatic");

        byte small = 42;
        int medium = small;                 // byte  -> int
        long large = medium;                // int   -> long
        double wide = large;                // long  -> double

        Out("byte -> int", medium);
        Out("int -> long", large);
        Out("long -> double", wide);

        Section("Explicit cast - you accept the risk");

        Out("(int)9.99", (int)9.99);
        Out("(int)-9.99", (int)-9.99);
        Warn("A cast TRUNCATES towards zero. It does not round.");

        int tooBig = 300;
        Out("(byte)300  wraps around", (byte)tooBig);
        Out("300 in binary", Convert.ToString(tooBig, 2));
        Out("only the bottom 8 bits survive", Convert.ToString((byte)tooBig, 2).PadLeft(8, '0'));

        Section("Rounding properly");

        Out("Math.Round(9.5)", Math.Round(9.5));
        Out("Math.Round(8.5)", Math.Round(8.5));
        Note("Exact halves go to the nearest EVEN number by default (banker's rounding).");
        Out("Math.Round(8.5, AwayFromZero)", Math.Round(8.5, MidpointRounding.AwayFromZero));
        Out("Math.Round(3.14159, 2)", Math.Round(3.14159, 2));
        Out("Math.Floor(9.99)", Math.Floor(9.99));
        Out("Math.Ceiling(9.01)", Math.Ceiling(9.01));
        Out("Math.Floor(-9.99)", Math.Floor(-9.99));
        Out("Math.Truncate(-9.99)", Math.Truncate(-9.99));

        Section("Cast vs Parse vs Convert");

        Out("(int)9.99  truncates", (int)9.99);
        Out("Convert.ToInt32(9.99)  rounds", Convert.ToInt32(9.99));
        Out("int.Parse(\"42\")", int.Parse("42"));
        Out("Convert.ToInt32((string?)null)", Convert.ToInt32((string?)null));

        Section("Number bases");

        int value = 202;
        Out("202 to binary", Convert.ToString(value, 2));
        Out("202 to hex", Convert.ToString(value, 16).ToUpperInvariant());
        Out("\"11001010\" from binary", Convert.ToInt32("11001010", 2));
        Out("\"CA\" from hex", Convert.ToInt32("CA", 16));
        Out("hex literal 0xCA", 0xCA);
        Out("binary literal 0b1100_1010", 0b1100_1010);
        Out("digit separators: 1_000_000", 1_000_000);

        Section("The Math class");

        Out("Math.Abs(-7)", Math.Abs(-7));
        Out("Math.Max(3, 9)", Math.Max(3, 9));
        Out("Math.Pow(2, 10)", Math.Pow(2, 10));
        Out("Math.Sqrt(144)", Math.Sqrt(144));
        Out("Math.Log2(1024)", Math.Log2(1024));
        Out("Math.PI", Math.PI);
        Out("Math.Clamp(150, 0, 100)", Math.Clamp(150, 0, 100));
        Out("Math.Sin(30 degrees)", Math.Round(Math.Sin(30 * Math.PI / 180), 4));
        Out("1 << 10  (2^10, integer and fast)", 1 << 10);

        Section("Random numbers");

        // A fixed seed gives the SAME sequence every run - essential for testing.
        Random random = new Random(42);
        Out("new Random(42), ten dice rolls",
            string.Join(" ", Enumerable.Range(0, 10).Select(_ => random.Next(1, 7))));
        Out("random.NextDouble()", Math.Round(random.NextDouble(), 4));
        Out("Random.Shared.Next(100)", Random.Shared.Next(100));

        Warn("Next(1, 7) gives 1-6. The upper bound is EXCLUSIVE, so Next(1, 6) never rolls a six.");

        Random dice = new Random(7);
        int[] tally = new int[7];
        for (int i = 0; i < 6000; i++) tally[dice.Next(1, 7)]++;
        for (int face = 1; face <= 6; face++) Out($"6000 rolls, face {face}", tally[face]);

        Section("Everything can become a string");

        Out("42.ToString()", 42.ToString());
        Out("3.14159.ToString(\"F2\")", 3.14159.ToString("F2", CultureInfo.InvariantCulture));
        Out("DateTime.ToString(\"dd/MM/yyyy\")",
            new DateTime(2026, 8, 16).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
    }
}
