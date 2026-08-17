using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Projects;

// Notes: docs/module-7/7.1.md
public sealed class L01_NumberGuessing : LessonBase
{
    public override string Id => "7.1";
    public override string Title => "Project: number guessing game";

    public override void Run()
    {
        Section("What this project uses");

        Out("module 1", "loops, selection, methods, input validation, Random");
        Out("module 2", "a class to hold the game state");
        Out("module 5", "binary search - the optimal guessing strategy");

        Section("A game, played by a human (simulated input)");

        Line();
        GuessingGame game = new(secret: 42, maximum: 100);
        foreach (int guess in new[] { 50, 25, 37, 43, 41, 42 })
        {
            GuessResult result = game.Guess(guess);
            Line($"guess {guess,3} -> {result}");
            if (result == GuessResult.Correct) break;
        }

        Out("attempts used", game.Attempts);
        Out("game over", game.IsOver);

        Section("The computer playing perfectly: binary search");

        Line();
        int attempts = PlayOptimally(secret: 73, maximum: 100, verbose: true);
        Out("attempts needed", attempts);

        Section("It never needs more than ceil(log2(n)) guesses");

        int worst = 0;
        for (int secret = 1; secret <= 100; secret++)
            worst = Math.Max(worst, PlayOptimally(secret, 100, verbose: false));

        Out("worst case over all 1-100", worst);
        Out("ceil(log2(100))", Math.Ceiling(Math.Log2(100)));

        foreach (int range in new[] { 100, 1_000, 1_000_000 })
            Out($"range 1-{range:N0}", $"at most {Math.Ceiling(Math.Log2(range))} guesses");

        Section("Compare with guessing one at a time");

        int linearWorst = 0, linearTotal = 0;
        for (int secret = 1; secret <= 100; secret++)
        {
            int used = PlayLinearly(secret, 100);
            linearWorst = Math.Max(linearWorst, used);
            linearTotal += used;
        }

        Out("linear: worst case", linearWorst);
        Out("linear: average", linearTotal / 100.0);
        Out("binary: worst case", worst);

        Section("The real interactive loop");

        // This is the version to type into a project of your own. It is not run here
        // because the course must complete without a human at the keyboard.
        Line();
        Line("static void Main()");
        Line("{");
        Line("    int secret = Random.Shared.Next(1, 101);");
        Line("    GuessingGame game = new(secret, 100);");
        Line();
        Line("    while (!game.IsOver)");
        Line("    {");
        Line("        Console.Write($\"Guess 1-100 (attempt {game.Attempts + 1}): \");");
        Line();
        Line("        if (!int.TryParse(Console.ReadLine(), out int guess))");
        Line("        {");
        Line("            Console.WriteLine(\"Numbers only, please.\");");
        Line("            continue;");
        Line("        }");
        Line();
        Line("        Console.WriteLine(game.Guess(guess) switch");
        Line("        {");
        Line("            GuessResult.TooLow  => \"Too low!\",");
        Line("            GuessResult.TooHigh => \"Too high!\",");
        Line("            GuessResult.Correct => $\"Got it in {game.Attempts} attempts!\",");
        Line("            _                   => \"Out of range.\",");
        Line("        });");
        Line("    }");
        Line("}");

        Section("Extend it yourself");

        Out("1", "add difficulty levels that change the range and the attempt limit");
        Out("2", "keep a high score table in a file (lesson 4.9)");
        Out("3", "swap the roles: you think of a number and the computer guesses");
        Out("4", "add 'warmer / colder' feedback based on the distance from the last guess");
    }

    private static int PlayOptimally(int secret, int maximum, bool verbose)
    {
        GuessingGame game = new(secret, maximum);
        int low = 1, high = maximum;

        while (!game.IsOver)
        {
            int guess = low + (high - low) / 2;         // always split the range in half
            GuessResult result = game.Guess(guess);

            if (verbose)
                Console.WriteLine($"      range {low}-{high}, guess {guess} -> {result}");

            if (result == GuessResult.TooLow) low = guess + 1;
            else if (result == GuessResult.TooHigh) high = guess - 1;
        }

        return game.Attempts;
    }

    private static int PlayLinearly(int secret, int maximum)
    {
        GuessingGame game = new(secret, maximum);

        for (int guess = 1; guess <= maximum; guess++)
            if (game.Guess(guess) == GuessResult.Correct) break;

        return game.Attempts;
    }
}

public enum GuessResult
{
    TooLow,
    TooHigh,
    Correct,
    OutOfRange,
}

/// <summary>
/// The game RULES live here, with no Console calls at all. That separation means the same class
/// works in a console app, a web page or a test - and it makes the logic easy to test.
/// </summary>
public class GuessingGame
{
    private readonly int _secret;
    private readonly int _maximum;

    public GuessingGame(int secret, int maximum)
    {
        if (secret < 1 || secret > maximum)
            throw new ArgumentOutOfRangeException(nameof(secret), "The secret must be inside the range.");

        _secret = secret;
        _maximum = maximum;
    }

    public int Attempts { get; private set; }

    public bool IsOver { get; private set; }

    public GuessResult Guess(int guess)
    {
        if (IsOver) throw new InvalidOperationException("The game has already finished.");

        if (guess < 1 || guess > _maximum) return GuessResult.OutOfRange;   // does not cost an attempt

        Attempts++;

        if (guess == _secret)
        {
            IsOver = true;
            return GuessResult.Correct;
        }

        return guess < _secret ? GuessResult.TooLow : GuessResult.TooHigh;
    }
}
