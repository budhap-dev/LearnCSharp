using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.4.md
public sealed class L04_Operators : LessonBase
{
    public override string Id => "1.4";
    public override string Title => "Operators and expressions";

    public override string Summary =>
        "Arithmetic, comparison and logic - including the integer-division trap that silently "
        + "produces wrong answers with no error at all.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Predict the result of integer versus floating-point division",
        "Use % for wrap-around and for splitting durations",
        "Explain short-circuit evaluation and why it makes null checks safe",
    ];

    public override void Run()
    {
        Section("Arithmetic - watch the division");

        int x = 17, y = 5;
        Out("x + y", x + y);
        Out("x - y", x - y);
        Out("x * y", x * y);
        Out("x / y   INTEGER division", x / y);
        Out("x % y   remainder", x % y);

        Warn("17 / 5 is 3, not 3.4. Two ints always divide to an int. Cast one side to fix it:");
        Out("(double)x / y", (double)x / y);

        Section("Modulus in practice");

        int totalSeconds = 3725;
        Out("3725 s -> hours", totalSeconds / 3600);
        Out("3725 s -> minutes", totalSeconds % 3600 / 60);
        Out("3725 s -> seconds", totalSeconds % 60);
        Out("is 42 even?", 42 % 2 == 0);

        Section("Divide by zero: ints throw, doubles do not");

        try { int zero = 0; Out("17 / 0", 17 / zero); }
        catch (DivideByZeroException) { Out("int 17 / 0", "DivideByZeroException"); }

        Out("double 17.0 / 0", 17.0 / 0);
        Out("double 0.0 / 0.0", 0.0 / 0.0);
        Out("double.IsNaN(0.0 / 0.0)", double.IsNaN(0.0 / 0.0));

        Section("Increment and compound assignment");

        int n = 10;
        Out("n++ returns the OLD value", n++);
        Out("n is now", n);
        Out("++n returns the NEW value", ++n);

        n = 10;
        n += 5; Out("n += 5", n);
        n *= 2; Out("n *= 2", n);
        n /= 4; Out("n /= 4", n);

        Section("Comparison and logic");

        bool sunny = true, warm = false;
        Out("sunny && warm  AND", sunny && warm);
        Out("sunny || warm  OR", sunny || warm);
        Out("!sunny         NOT", !sunny);
        Out("sunny ^ warm   XOR", sunny ^ warm);

        // && short-circuits: if the left side settles it, the right side never runs.
        // That is what makes this null check safe.
        string? maybeNull = null;
        Out("maybeNull != null && maybeNull.Length > 3", maybeNull != null && maybeNull.Length > 3);

        Section("Ternary ?: and the null operators");

        int mark = 72;
        Out("mark >= 50 ? \"Pass\" : \"Fail\"", mark >= 50 ? "Pass" : "Fail");

        string? nothing = null;
        Out("nothing ?? \"fallback\"", nothing ?? "fallback");
        Out("nothing?.Length", nothing?.Length);
        Out("\"hello\"?.Length", "hello"?.Length);

        string? user = null;
        user ??= "guest";                       // assign only if currently null
        Out("user ??= \"guest\"", user);

        Section("Bitwise operators - GCSE binary in code");

        int p = 0b1100, q = 0b1010;
        Out("p = 1100, q = 1010 -> p & q", Convert.ToString(p & q, 2).PadLeft(4, '0'));
        Out("p | q", Convert.ToString(p | q, 2).PadLeft(4, '0'));
        Out("p ^ q", Convert.ToString(p ^ q, 2).PadLeft(4, '0'));
        Out("p << 1  left shift doubles", $"{Convert.ToString(p << 1, 2)} = {p << 1}");
        Out("p >> 1  right shift halves", $"{Convert.ToString(p >> 1, 2).PadLeft(4, '0')} = {p >> 1}");

        Section("Precedence");

        Out("2 + 3 * 4", 2 + 3 * 4);
        Out("(2 + 3) * 4", (2 + 3) * 4);
        Note("When in doubt, add brackets. Brackets are free; a misread expression is not.");
    }
}
