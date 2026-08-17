using System.Collections.Concurrent;
using System.Diagnostics;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Production;

// Notes: docs/module-6/6.1.md
public sealed class L01_Threading : LessonBase
{
    public override string Id => "6.1";
    public override string Title => "Threading and concurrency";

    public override string Summary =>
        "Two threads touching the same variable can silently lose updates - a race condition, "
        + "demonstrated live here. Learn lock, Interlocked and the concurrent collections that "
        + "make shared state safe, and when concurrency is not worth its cost.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Demonstrate a race condition and explain why it loses updates",
        "Make shared state safe with lock, Interlocked or a concurrent collection",
        "Explain what a deadlock is and the ordering rule that prevents it",
    ];

    public override void Run()
    {
        Section("Threads: several workers in one process");

        Out("a process", "your running program: one heap, shared by all its threads");
        Out("a thread", "one worker executing code; every program starts with one");
        Out("why more", "use several CPU cores, or stay responsive while waiting");
        Out("the danger", "threads SHARE memory - and sharing mutable state is where bugs live");

        Section("A race condition, live");

        // One counter, four threads, each incrementing 100,000 times.
        // counter++ is secretly THREE steps: read, add one, write back.
        // Two threads can read the same value, both add one, both write - one update lost.
        int unsafeCounter = 0;

        RunOnFourThreads(() =>
        {
            for (int i = 0; i < 100_000; i++)
                unsafeCounter++;                    // NOT atomic
        });

        Out("expected", 400_000);
        Out("unsafeCounter actually reached", unsafeCounter);
        Warn("Updates were lost - and a different number vanishes every run. Race conditions "
           + "are the worst kind of bug: rare, unrepeatable, and absent from your tests.");

        Section("Fix 1: lock - one thread at a time");

        int lockedCounter = 0;
        object gate = new();

        RunOnFourThreads(() =>
        {
            for (int i = 0; i < 100_000; i++)
            {
                lock (gate)                          // only one thread may hold the gate
                {
                    lockedCounter++;
                }
            }
        });

        Out("lockedCounter", lockedCounter);
        Note("lock (gate) { ... } means: wait until nobody else holds the gate, take it, run "
           + "the block, release it. The three steps of ++ can no longer interleave.");

        Section("Fix 2: Interlocked - atomic hardware operations");

        int atomicCounter = 0;

        RunOnFourThreads(() =>
        {
            for (int i = 0; i < 100_000; i++)
                Interlocked.Increment(ref atomicCounter);   // one indivisible CPU instruction
        });

        Out("atomicCounter", atomicCounter);
        Note("For simple counters, Interlocked is both correct AND faster than lock - the CPU "
           + "provides the atomicity directly.");

        Section("Fix 3: concurrent collections");

        // Dictionary<K,V> corrupts under concurrent writes. ConcurrentDictionary does not.
        ConcurrentDictionary<int, int> tallies = new();

        RunOnFourThreads(() =>
        {
            for (int i = 0; i < 50_000; i++)
                tallies.AddOrUpdate(i % 10, 1, (_, count) => count + 1);
        });

        Out("buckets", tallies.Count);
        Out("total tallied", tallies.Values.Sum());
        Out("also available", "ConcurrentQueue, ConcurrentBag, BlockingCollection");

        Section("Deadlock: the mutual standoff");

        Out("the recipe", "thread A holds lock 1, wants lock 2; thread B holds 2, wants 1");
        Out("the result", "both wait forever - no exception, no crash, just silence");
        Out("the rule that prevents it", "every thread acquires locks in the SAME ORDER");
        Warn("This lesson does not run a real deadlock - it would hang the course forever, "
           + "which is precisely the point.");

        Section("Parallel.For: splitting CPU work across cores");

        const int limit = 2_000_000;

        Stopwatch watch = Stopwatch.StartNew();
        int sequential = CountPrimes(2, limit);
        long sequentialMs = watch.ElapsedMilliseconds;

        watch.Restart();
        int parallelCount = 0;
        Parallel.For(2, limit, n =>
        {
            if (IsPrime(n)) Interlocked.Increment(ref parallelCount);
        });
        long parallelMs = watch.ElapsedMilliseconds;

        Out($"primes below {limit:N0}, one thread", $"{sequential:N0} in {sequentialMs} ms");
        Out($"same on all cores (Parallel.For)", $"{parallelCount:N0} in {parallelMs} ms");
        Out("cores on this machine", Environment.ProcessorCount);

        Note("Note the Interlocked inside the parallel loop - parallelism does not excuse you "
           + "from the race rules; it makes them mandatory.");

        Section("Thread vs Task - which do you actually use?");

        Out("new Thread(...)", "raw OS thread - almost never used directly today");
        Out("Task.Run(...)", "borrows a pooled thread for CPU work - the normal choice (4.10)");
        Out("async/await", "I/O waiting without holding any thread at all (4.10)");
        Out("Parallel.For/ForEach", "data parallelism over a collection");

        Section("When concurrency is NOT worth it");

        watch.Restart();
        int tinySequential = 0;
        for (int i = 0; i < 10_000; i++) tinySequential += i % 7;
        long tinySeqTicks = watch.ElapsedTicks;

        watch.Restart();
        int tinyParallel = 0;
        Parallel.For(0, 10_000, i => Interlocked.Add(ref tinyParallel, i % 7));
        long tinyParTicks = watch.ElapsedTicks;

        Out("tiny job, one thread", $"{tinySeqTicks} ticks");
        Out("tiny job, Parallel.For", $"{tinyParTicks} ticks");
        Warn("For small work the coordination costs MORE than the work. Parallelise big, "
           + "independent chunks - and always measure (lesson 6.2).");

        Section("The rules that keep you safe");

        Out("1", "prefer no shared state: give each thread its own data, combine at the end");
        Out("2", "if you must share: immutable data needs no locks at all (2.9 records)");
        Out("3", "if you must mutate: lock, Interlocked, or a concurrent collection");
        Out("4", "acquire multiple locks in one agreed order, everywhere");
        Out("5", "never guess that parallel is faster - time it");
    }

    private static void RunOnFourThreads(Action work)
    {
        Task[] tasks = new Task[4];
        for (int t = 0; t < 4; t++) tasks[t] = Task.Run(work);
        Task.WaitAll(tasks);
    }

    private static int CountPrimes(int from, int to)
    {
        int count = 0;
        for (int n = from; n < to; n++)
            if (IsPrime(n)) count++;
        return count;
    }

    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        for (int i = 2; i * i <= n; i++)
            if (n % i == 0) return false;
        return true;
    }
}
