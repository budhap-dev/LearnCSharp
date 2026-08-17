using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.2.md
public sealed class L02_Variables : LessonBase
{
    public override string Id => "1.2";
    public override string Title => "Variables, data types and constants";

    public override void Run()
    {
        Section("Declaring variables: type name = value;");

        int score = 0;
        string playerName = "Ada";
        bool isGameOver = false;
        char grade = 'A';           // single quotes = char
        double height = 1.75;

        Out("score", score);
        Out("playerName", playerName);
        Out("isGameOver", isGameOver);
        Out("grade", grade);
        Out("height", height);

        Section("How big is each whole-number type?");

        Out("byte  (1 byte)", $"{byte.MinValue} .. {byte.MaxValue}");
        Out("short (2 bytes)", $"{short.MinValue} .. {short.MaxValue}");
        Out("int   (4 bytes)", $"{int.MinValue} .. {int.MaxValue}");
        Out("long  (8 bytes)", $"{long.MinValue} .. {long.MaxValue}");

        Section("Overflow: what happens past the maximum");

        int big = int.MaxValue;
        Out("int.MaxValue + 1 (wraps round)", unchecked(big + 1));

        try
        {
            checked { big += 1; }               // checked turns overflow into an exception
        }
        catch (OverflowException)
        {
            Out("same sum inside checked { }", "OverflowException");
        }

        Section("double is approximate, decimal is exact");

        double a = 0.1, b = 0.2;
        Out("0.1 + 0.2 as double", a + b);
        Out("(0.1 + 0.2) == 0.3", (a + b) == 0.3);

        decimal da = 0.1m, db = 0.2m;           // the m suffix makes a decimal literal
        Out("0.1m + 0.2m as decimal", da + db);
        Out("(0.1m + 0.2m) == 0.3m", (da + db) == 0.3m);

        Warn("Use decimal for money. Never compare doubles with == - compare the difference instead.");
        Out("Math.Abs((a+b) - 0.3) < 1e-9", Math.Abs((a + b) - 0.3) < 1e-9);

        Section("char is really a number");

        Out("(int)'A'", (int)'A');
        Out("(char)('A' + 2)", (char)('A' + 2));

        Section("var infers the type - it is still static typing");

        var count = 10;                          // compiler decides: int
        var name = "Ada";                        // compiler decides: string
        Out("var count = 10 is really a", count.GetType().Name);
        Out("var name = \"Ada\" is really a", name.GetType().Name);
        // count = "hello";  <- would not compile: count is an int, permanently.

        Section("const and readonly");

        const int MaxLives = 3;                  // fixed at compile time
        Out("MaxLives", MaxLives);
        // MaxLives = 4;  <- will not compile

        Section("Default values");

        Out("default(int)", default(int));
        Out("default(bool)", default(bool));
        Out("default(double)", default(double));
        Out("default(string)", default(string));

        Note("Fields get these defaults automatically. Local variables do not - the compiler makes "
           + "you assign one before you read it.");
    }
}
