using System.Diagnostics;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Advanced;

// Notes: docs/module-4/4.10.md
public sealed class L10_Async : LessonBase
{
    public override string Id => "4.10";
    public override string Title => "Asynchronous programming: async, await and Task";

    public override string Summary =>
        "await frees the thread while waiting instead of blocking it. The difference between "
        + "awaiting one at a time and starting tasks concurrently is dramatic, and measured "
        + "here.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Write async methods returning Task and Task<T>",
        "Use WhenAll to run independent work concurrently",
        "Explain why calling .Result can deadlock",
    ];

    public override void Run()
    {
        // Run is synchronous, so we bridge into async code once, here, with GetAwaiter().GetResult().
        // In a real app you would write 'static async Task Main()' and await all the way down.
        RunAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        Section("The problem: blocking wastes time");

        Stopwatch watch = Stopwatch.StartNew();
        int a = FetchBlocking("page A", 150);
        int b = FetchBlocking("page B", 150);
        int c = FetchBlocking("page C", 150);
        Out("three blocking calls of 150ms", $"{watch.ElapsedMilliseconds} ms, total {a + b + c}");

        Section("await one at a time: same total, but the thread is free to work");

        watch.Restart();
        int x = await FetchAsync("page A", 150);
        int y = await FetchAsync("page B", 150);
        int z = await FetchAsync("page C", 150);
        Out("three awaited calls, one by one", $"{watch.ElapsedMilliseconds} ms, total {x + y + z}");

        Note("Awaiting in sequence is still sequential. The gain here is that the thread is not "
           + "held hostage - in a UI it stays responsive, on a server it serves other requests.");

        Section("Task.WhenAll: start them all, then wait once");

        watch.Restart();
        Task<int> taskA = FetchAsync("page A", 150);   // starts immediately
        Task<int> taskB = FetchAsync("page B", 150);
        Task<int> taskC = FetchAsync("page C", 150);

        int[] results = await Task.WhenAll(taskA, taskB, taskC);
        Out("three concurrent calls", $"{watch.ElapsedMilliseconds} ms, total {results.Sum()}");

        Warn("The speed-up comes from starting the tasks BEFORE awaiting them. "
           + "'await FetchAsync(..)' three times in a row does not overlap anything.");

        Section("Task.WhenAny: react to the first one to finish");

        Task<string> fast = SlowLabel("fast", 50);
        Task<string> slow = SlowLabel("slow", 300);

        Task<string> winner = await Task.WhenAny(fast, slow);
        Out("first to finish", await winner);

        await slow;    // still tidy up the other one

        Section("Returning values: Task<T> versus Task");

        Out("async Task<int>", "an async method that returns an int");
        Out("async Task", "an async method that returns nothing, but is still awaitable");
        Out("async void", "ONLY for event handlers - you cannot await it or catch its exceptions");

        Section("Exceptions inside async code");

        try
        {
            await FailingAsync();
        }
        catch (InvalidOperationException ex)
        {
            Out("caught from an async method", ex.Message);
        }

        // With WhenAll, the first exception surfaces; the Task holds them all.
        Task both = Task.WhenAll(FailingAsync(), FailingAsync());
        try
        {
            await both;
        }
        catch (InvalidOperationException)
        {
            Out("WhenAll with two failures", $"{both.Exception?.InnerExceptions.Count} exceptions collected");
        }

        Section("Cancellation");

        using CancellationTokenSource cancellation = new();
        cancellation.CancelAfter(80);

        try
        {
            await LongJobAsync(500, cancellation.Token);
        }
        catch (OperationCanceledException)
        {
            Out("job cancelled after 80ms", "OperationCanceledException");
        }

        Section("Progress reporting");

        Progress<int> progress = new(percent => Console.WriteLine($"      progress: {percent}%"));
        Line();
        await WorkWithProgressAsync(progress);

        Section("What NOT to do");

        Out(".Result or .Wait()", "blocks the thread and can DEADLOCK in UI apps");
        Out("async void", "exceptions escape and crash the process");
        Out("Thread.Sleep in async code", "blocks - use await Task.Delay instead");
        Out("async with no await", "the compiler warns; it runs synchronously");

        Section("Where async genuinely helps");

        Out("file and network I/O", "the CPU is idle while waiting - the big win");
        Out("database queries", "same reason");
        Out("UI applications", "keeps the window responsive");
        Out("pure calculation", "async does NOT help - use Task.Run or Parallel for that");

        Section("CPU work: Task.Run moves it off the current thread");

        watch.Restart();
        int[] sums = await Task.WhenAll(
            Task.Run(() => CountPrimes(1, 25000)),
            Task.Run(() => CountPrimes(25000, 50000)));
        Out("primes below 50,000 on 2 threads", $"{sums.Sum()} found in {watch.ElapsedMilliseconds} ms");
    }

    private static int FetchBlocking(string name, int milliseconds)
    {
        Thread.Sleep(milliseconds);                // blocks this thread completely
        return name.Length;
    }

    private static async Task<int> FetchAsync(string name, int milliseconds)
    {
        await Task.Delay(milliseconds);            // releases the thread while waiting
        return name.Length;
    }

    private static async Task<string> SlowLabel(string label, int milliseconds)
    {
        await Task.Delay(milliseconds);
        return label;
    }

    private static async Task FailingAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException("the async operation failed");
    }

    private static async Task LongJobAsync(int milliseconds, CancellationToken token)
    {
        // Pass the token down so the wait itself can be interrupted.
        await Task.Delay(milliseconds, token);
    }

    private static async Task WorkWithProgressAsync(IProgress<int> progress)
    {
        for (int percent = 25; percent <= 100; percent += 25)
        {
            await Task.Delay(20);
            progress.Report(percent);
        }
    }

    private static int CountPrimes(int from, int to)
    {
        int count = 0;
        for (int n = Math.Max(2, from); n < to; n++)
        {
            bool prime = true;
            for (int i = 2; i * i <= n; i++)
            {
                if (n % i == 0) { prime = false; break; }
            }
            if (prime) count++;
        }
        return count;
    }
}
