using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Foundations;

// Notes: docs/module-1/1.7.md
public sealed class L07_Arrays : LessonBase
{
    public override string Id => "1.7";
    public override string Title => "Arrays: 1D, 2D and jagged";

    public override void Run()
    {
        Section("Creating arrays");

        int[] empty = new int[5];               // 5 slots, all zero
        int[] primes = [2, 3, 5, 7, 11];        // C# 12 collection expression
        string[] names = ["Ada", "Alan", "Grace"];

        Out("new int[5]", string.Join(", ", empty));
        Out("primes", string.Join(", ", primes));
        Out("primes.Length", primes.Length);

        Section("Indexing starts at zero");

        Out("primes[0]  first", primes[0]);
        Out("primes[primes.Length - 1]  last", primes[primes.Length - 1]);
        Out("primes[^1]  last, from the end", primes[^1]);
        Out("primes[^2]  second from last", primes[^2]);

        try { Out("primes[5]", primes[5]); }
        catch (IndexOutOfRangeException) { Out("primes[5]", "IndexOutOfRangeException"); }

        Section("Ranges slice an array");

        Out("primes[1..4]", string.Join(", ", primes[1..4]));
        Out("primes[..3]", string.Join(", ", primes[..3]));
        Out("primes[2..]", string.Join(", ", primes[2..]));
        Out("primes[^2..]", string.Join(", ", primes[^2..]));

        Section("The classic array algorithms");

        int[] data = [23, 5, 91, 42, 17, 8];
        Out("data", string.Join(", ", data));

        int sum = 0;
        foreach (int v in data) sum += v;
        Out("sum", sum);
        Out("average", (double)sum / data.Length);

        // Start max at data[0], NOT 0 - otherwise all-negative data gives the wrong answer.
        int max = data[0], min = data[0], maxIndex = 0;
        for (int i = 1; i < data.Length; i++)
        {
            if (data[i] > max) { max = data[i]; maxIndex = i; }
            if (data[i] < min) min = data[i];
        }
        Out("max", max);
        Out("index of max", maxIndex);
        Out("min", min);

        int above20 = 0;
        foreach (int v in data) if (v > 20) above20++;
        Out("count above 20", above20);

        Section("Arrays are reference types");

        int[] alias = data;                     // copies the arrow, not the boxes
        alias[0] = 999;
        Out("after alias[0] = 999, data[0]", data[0]);

        int[] realCopy = data.ToArray();        // a genuine copy
        realCopy[0] = 1;
        Out("after realCopy[0] = 1, data[0]", data[0]);
        data[0] = 23;

        Section("Useful Array methods");

        int[] sorted = data.ToArray();
        Array.Sort(sorted);
        Out("Array.Sort", string.Join(", ", sorted));
        Array.Reverse(sorted);
        Out("Array.Reverse", string.Join(", ", sorted));
        Out("Array.IndexOf(data, 42)", Array.IndexOf(data, 42));

        int[] sevens = new int[5];
        Array.Fill(sevens, 7);
        Out("Array.Fill(new int[5], 7)", string.Join(", ", sevens));

        Section("2D arrays - grids");

        int[,] table = new int[4, 4];           // 4 rows, 4 columns, one memory block
        for (int row = 0; row < table.GetLength(0); row++)
            for (int col = 0; col < table.GetLength(1); col++)
                table[row, col] = (row + 1) * (col + 1);

        Out("GetLength(0) rows", table.GetLength(0));
        Out("GetLength(1) columns", table.GetLength(1));
        Out("Length is rows x columns", table.Length);
        Out("table[2, 3]", table[2, 3]);

        Line();
        for (int row = 0; row < 4; row++)
        {
            string line = "";
            for (int col = 0; col < 4; col++) line += $"{table[row, col],4}";
            Line(line);
        }

        char[,] board =
        {
            { 'X', 'O', 'X' },
            { 'O', 'X', 'O' },
            { 'X', 'O', 'X' },
        };

        Line();
        for (int row = 0; row < 3; row++)
            Line($" {board[row, 0]} | {board[row, 1]} | {board[row, 2]} ");

        Section("Jagged arrays - rows of different lengths");

        int[][] triangle =
        [
            [1],
            [1, 1],
            [1, 2, 1],
            [1, 3, 3, 1],
            [1, 4, 6, 4, 1],
        ];

        Line();
        foreach (int[] row in triangle)
            Line(string.Join(" ", row).PadLeft(6 + row.Length));

        Out("triangle.Length  (rows)", triangle.Length);
        Out("triangle[4].Length", triangle[4].Length);
        Out("triangle[4][2]", triangle[4][2]);

        Note("int[,] is one rectangular block. int[][] is an array of separate arrays, so rows can "
           + "have different lengths and you can foreach over whole rows.");
    }
}
