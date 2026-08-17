using System.Text;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.6.md
public sealed class L06_Iteration : LessonBase
{
    public override string Id => "1.6";
    public override string Title => "Iteration: for, while, do-while, foreach";

    public override string Summary =>
        "Count-controlled and condition-controlled loops, how to choose between them, and the "
        + "off-by-one error that catches absolutely everyone.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Choose correctly between for, while, do-while and foreach",
        "Avoid off-by-one errors",
        "Use break and continue deliberately",
    ];

    public override void Run()
    {
        Section("for - when you know how many times");

        StringBuilder text = new();
        for (int i = 1; i <= 5; i++) text.Append(i).Append(' ');
        Out("for (i = 1; i <= 5; i++)", text.ToString().Trim());

        text.Clear();
        for (int i = 10; i >= 0; i -= 2) text.Append(i).Append(' ');
        Out("for (i = 10; i >= 0; i -= 2)", text.ToString().Trim());

        Section("Off-by-one: < versus <=");

        int countA = 0; for (int i = 0; i < 5; i++) countA++;
        int countB = 0; for (int i = 0; i <= 5; i++) countB++;
        Out("i = 0; i <  5  runs", countA);
        Out("i = 0; i <= 5  runs", countB);

        Section("while - when you do not know how many times");

        int total = 0, n = 1;
        while (total < 50) { total += n; n++; }
        Out("smallest n where 1+2+...+n >= 50", n - 1);
        Out("that total", total);

        int value = 1000, halvings = 0;
        while (value > 1) { value /= 2; halvings++; }
        Out("halvings to get 1000 down to 1", halvings);

        Section("do-while - always runs at least once");

        int[] pretendInput = [42, -1, 7];
        int attempt = 0, chosen;
        do
        {
            chosen = pretendInput[attempt];
            attempt++;
        }
        while (chosen is < 1 or > 10 && attempt < pretendInput.Length);

        Out("first valid value found", chosen);
        Out("attempts taken", attempt);

        Section("foreach - every item, no counter to get wrong");

        string[] planets = ["Mercury", "Venus", "Earth", "Mars"];
        foreach (string planet in planets) Out("planet", planet);

        Warn("You cannot Add or Remove while a foreach is running - that throws "
           + "InvalidOperationException. Use a for loop when you need to change the collection.");

        Section("break and continue");

        text.Clear();
        for (int i = 1; i <= 10; i++)
        {
            if (i % 2 == 0) continue;      // skip evens, go to the next i
            if (i > 7) break;              // leave the loop entirely
            text.Append(i).Append(' ');
        }
        Out("odds up to 7", text.ToString().Trim());

        // Linear search: break out the moment we find it.
        int[] data = [4, 8, 15, 16, 23, 42];
        int foundAt = -1;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == 16) { foundAt = i; break; }
        }
        Out("index of 16", foundAt);

        Section("Nested loops");

        Line();
        for (int row = 1; row <= 4; row++)
        {
            StringBuilder rowText = new();
            for (int col = 1; col <= 4; col++) rowText.Append($"{row * col,4}");
            Line(rowText.ToString());
        }

        Line();
        for (int row = 1; row <= 5; row++) Line(new string('*', row));

        Note("A loop inside a loop does outer x inner work. Two nested loops over n items is "
           + "O(n squared) - see lesson 5.1.");
    }
}
