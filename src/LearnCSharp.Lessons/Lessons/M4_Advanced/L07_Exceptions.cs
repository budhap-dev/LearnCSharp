using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.7.md
public sealed class L07_Exceptions : LessonBase
{
    public override string Id => "4.7";
    public override string Title => "Exceptions in depth and custom exception types";

    public override void Run()
    {
        Section("The exception hierarchy");

        Out("Exception", "the root of everything");
        Out("  SystemException", "thrown by the runtime: NullReference, IndexOutOfRange...");
        Out("  ArgumentException", "a caller passed something wrong");
        Out("    ArgumentNullException", "...specifically, null");
        Out("    ArgumentOutOfRangeException", "...specifically, out of range");
        Out("  InvalidOperationException", "right arguments, wrong time");
        Out("  IOException", "files, streams, network");
        Out("  your own : Exception", "domain problems specific to your program");

        Section("Catch order: most specific first");

        foreach (string input in new[] { "5", "0", "abc" })
        {
            try
            {
                Out($"100 / {input}", 100 / int.Parse(input));
            }
            catch (DivideByZeroException)          // most specific
            {
                Out($"100 / {input}", "DivideByZeroException");
            }
            catch (FormatException)
            {
                Out($"100 / {input}", "FormatException");
            }
            catch (Exception ex)                   // the catch-all goes LAST
            {
                Out($"100 / {input}", $"something else: {ex.GetType().Name}");
            }
        }

        Warn("Put catch (Exception) first and the compiler stops you - later catches would be "
           + "unreachable. That rule saves you from a common mistake.");

        Section("Exception filters with 'when'");

        foreach (int code in new[] { 404, 500 })
        {
            try
            {
                throw new HttpLikeException(code);
            }
            catch (HttpLikeException ex) when (ex.StatusCode == 404)
            {
                Out($"status {code}", "handled as 'not found'");
            }
            catch (HttpLikeException)
            {
                Out($"status {code}", "handled as a general failure");
            }
        }

        Note("'when' filters WITHOUT unwinding the stack, so the original stack trace survives. "
           + "That makes it better than catching, testing, and re-throwing.");

        Section("throw vs throw ex - this matters");

        try { CallThatRethrowsCorrectly(); }
        catch (Exception ex)
        {
            Out("using 'throw;' - frames kept", CountFrames(ex));
        }

        try { CallThatRethrowsBadly(); }
        catch (Exception ex)
        {
            Out("using 'throw ex;' - frames lost", CountFrames(ex));
        }

        Warn("'throw ex;' RESETS the stack trace to this line, hiding where the problem really "
           + "started. Write a bare 'throw;' to re-throw.");

        Section("Wrapping an exception without losing the cause");

        try
        {
            try
            {
                int[] tiny = [1];
                _ = tiny[5];
            }
            catch (IndexOutOfRangeException inner)
            {
                throw new DataLoadException("Could not load the save file.", inner);
            }
        }
        catch (DataLoadException ex)
        {
            Out("outer message", ex.Message);
            Out("ex.InnerException", ex.InnerException?.GetType().Name);
            Out("inner message", ex.InnerException?.Message);
        }

        Section("Custom exceptions carry your own data");

        try
        {
            Withdraw(100m, 250m);
        }
        catch (InsufficientFundsException ex)
        {
            Out("message", ex.Message);
            Out("ex.Balance", ex.Balance);
            Out("ex.Requested", ex.Requested);
            Out("ex.Shortfall", ex.Shortfall);
        }

        Section("finally and using");

        Out("Cleanup()", Cleanup());
        Note("finally runs on every path out of the try - normal end, return, or exception. "
           + "For anything disposable, 'using' does this for you (lesson 4.8).");

        Section("Guard helpers built into .NET");

        try { object? player = null; ArgumentNullException.ThrowIfNull(player, "player"); }
        catch (ArgumentNullException ex) { Out("ThrowIfNull", ex.ParamName); }

        try { ArgumentOutOfRangeException.ThrowIfNegative(-1, "lives"); }
        catch (ArgumentOutOfRangeException ex) { Out("ThrowIfNegative", ex.ParamName); }

        try { ArgumentException.ThrowIfNullOrWhiteSpace("  ", "name"); }
        catch (ArgumentException ex) { Out("ThrowIfNullOrWhiteSpace", ex.ParamName); }

        Section("When to throw and when not to");

        Out("throw", "the caller broke the contract, or something genuinely unexpected happened");
        Out("return false / TryX", "failure is a normal, expected outcome (bad user input)");
        Out("return null or a default", "'not found' is ordinary - use a nullable return type");
        Out("never", "empty catch blocks, or exceptions used as control flow in a loop");
    }

    private static void CallThatRethrowsCorrectly()
    {
        try { DeepFailure(); }
        catch (InvalidOperationException) { throw; }        // keeps the whole trace
    }

    private static void CallThatRethrowsBadly()
    {
        try { DeepFailure(); }
#pragma warning disable CA2200 // deliberately wrong, so you can see the damage it does
        catch (InvalidOperationException ex) { throw ex; }   // trace restarts here
#pragma warning restore CA2200
    }

    private static void DeepFailure() => Level2();
    private static void Level2() => Level3();
    private static void Level3() => throw new InvalidOperationException("deep failure");

    private static int CountFrames(Exception ex) =>
        (ex.StackTrace ?? "").Split('\n', StringSplitOptions.RemoveEmptyEntries).Length;

    private static void Withdraw(decimal balance, decimal amount)
    {
        if (amount > balance) throw new InsufficientFundsException(balance, amount);
    }

    private static string Cleanup()
    {
        try
        {
            return "returned from try";
        }
        finally
        {
            // Runs even though we already returned.
        }
    }
}

/// <summary>A custom exception: inherit Exception and offer the three standard constructors.</summary>
public class DataLoadException : Exception
{
    public DataLoadException() { }
    public DataLoadException(string message) : base(message) { }
    public DataLoadException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>A custom exception that carries extra, useful data.</summary>
public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(decimal balance, decimal requested)
        : base($"Cannot withdraw {requested:F2}: the balance is only {balance:F2}.")
    {
        Balance = balance;
        Requested = requested;
    }

    public decimal Balance { get; }
    public decimal Requested { get; }
    public decimal Shortfall => Requested - Balance;
}

public class HttpLikeException : Exception
{
    public HttpLikeException(int statusCode) : base($"Request failed with status {statusCode}.")
        => StatusCode = statusCode;

    public int StatusCode { get; }
}
