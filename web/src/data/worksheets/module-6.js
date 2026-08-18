// Module 6 - Production C#: concurrency, performance, security, serialization and DI.
// Every task is written so its output is deterministic - parallel tasks only ever report a
// final total, never per-thread lines whose order could vary.

/** @type {import('./index').Worksheet} */
export default {
  module: 6,
  intro:
    'Thread-safe shared state, building strings the fast way, hashing a password, JSON round-trips, and wiring a class through an interface. Parallel tasks here print only their final result, so the output never depends on timing.',
  tasks: [
    {
      id: '6.1',
      lesson: '6.1',
      title: 'A thread-safe counter',
      level: 2,
      task: 'Run 1000 increments across many threads with `Parallel.For`, but make them add up correctly. A plain `count++` loses updates in a race; guard it with `lock` (or `Interlocked.Increment`). Print the final total, which must always be 1000.',
      starter: `int count = 0;
object gate = new();

Parallel.For(0, 1000, i =>
{
    // TODO: increment count safely - a bare count++ races and loses updates
    count++;
});

Console.WriteLine(count);`,
      expected: `1000`,
      hints: [
        'Wrap the increment: lock (gate) { count++; }',
        'Or drop the lock and use Interlocked.Increment(ref count); - a single atomic step.',
        'Without protection the answer is usually a bit under 1000 and changes every run - that is the race.',
      ],
      solution: `int count = 0;
object gate = new();

Parallel.For(0, 1000, i =>
{
    lock (gate)
    {
        count++;
    }
});

Console.WriteLine(count);`,
    },
    {
      id: '6.2',
      lesson: '6.1',
      title: 'Await many tasks',
      level: 2,
      task: 'Start three async jobs that each return a number after a tiny delay, wait for all of them with `Task.WhenAll`, and print their sum. Awaiting them together lets them overlap instead of running one after another.',
      starter: `int[] results = await Task.WhenAll(Job(2), Job(3), Job(5));
Console.WriteLine(results.Sum());

async Task<int> Job(int n)
{
    // TODO: await a small delay, then return n * 10
    return 0;
}`,
      expected: `100`,
      hints: [
        'await Task.Delay(1); stands in for slow work without blocking a thread.',
        'Return n * 10; the three results are 20, 30, 50.',
        'Task.WhenAll gives back an int[] once every task has finished.',
      ],
      solution: `int[] results = await Task.WhenAll(Job(2), Job(3), Job(5));
Console.WriteLine(results.Sum());

async Task<int> Job(int n)
{
    await Task.Delay(1);
    return n * 10;
}`,
    },
    {
      id: '6.3',
      lesson: '6.2',
      title: 'Build strings the fast way',
      level: 1,
      task: 'Join the numbers 1 to 100 into "1,2,3,...,100" using a `StringBuilder` rather than repeated `+` (which makes a new string every time). Print the total length of the finished string.',
      starter: `var sb = new System.Text.StringBuilder();

// TODO: append 1..100 separated by commas, no trailing comma

string result = sb.ToString();
Console.WriteLine(result.Length);`,
      expected: `291`,
      hints: [
        'Append the number, then a comma - but not after the last one.',
        'Easiest: append "," before every number except the first: if (i > 1) sb.Append(",");',
        'The digits (192) plus 99 commas plus... just trust the StringBuilder and print result.Length.',
      ],
      solution: `var sb = new System.Text.StringBuilder();

for (int i = 1; i <= 100; i++)
{
    if (i > 1) sb.Append(',');
    sb.Append(i);
}

string result = sb.ToString();
Console.WriteLine(result.Length);`,
    },
    {
      id: '6.4',
      lesson: '6.3',
      title: 'Hash a password',
      level: 2,
      task: 'Never store a password as plain text - store a hash. Compute the SHA-256 hash of "correct horse" and print it as uppercase hex. Then show that checking the same password hashes to the same value ("match: True"), and a wrong one does not.',
      starter: `using System.Security.Cryptography;
using System.Text;

string stored = Hash("correct horse");
Console.WriteLine(stored);
Console.WriteLine($"match: {Hash("correct horse") == stored}");
Console.WriteLine($"match: {Hash("wrong") == stored}");

string Hash(string password)
{
    // TODO: SHA-256 the UTF-8 bytes, return uppercase hex
    return "";
}`,
      expected: `4104D36F8DA2C254349F85836793EBE029E0C957063A34C91C2E9203187B5631
match: True
match: False`,
      hints: [
        'byte[] bytes = Encoding.UTF8.GetBytes(password);',
        'byte[] hash = SHA256.HashData(bytes);',
        'Convert.ToHexString(hash) gives the uppercase hex string.',
      ],
      solution: `using System.Security.Cryptography;
using System.Text;

string stored = Hash("correct horse");
Console.WriteLine(stored);
Console.WriteLine($"match: {Hash("correct horse") == stored}");
Console.WriteLine($"match: {Hash("wrong") == stored}");

string Hash(string password)
{
    byte[] bytes = Encoding.UTF8.GetBytes(password);
    byte[] hash = SHA256.HashData(bytes);
    return Convert.ToHexString(hash);
}`,
    },
    {
      id: '6.5',
      lesson: '6.3',
      title: 'Validate before you trust',
      level: 2,
      task: 'Write `bool IsValidUsername(string s)` that accepts a username only if it is 3 to 12 characters and every character is a letter or a digit. Print the verdict for each candidate.',
      starter: `string[] candidates = ["ada", "ab", "SuperLongUsername", "bad name", "grace99", "ok_"];

foreach (string c in candidates)
{
    Console.WriteLine($"{c,-18} {IsValidUsername(c)}");
}

bool IsValidUsername(string s)
{
    // TODO: length 3..12 and only letters or digits
    return true;
}`,
      expected: `ada                True
ab                 False
SuperLongUsername  False
bad name           False
grace99            True
ok_                False`,
      hints: [
        'First the length: if (s.Length < 3 || s.Length > 12) return false;',
        'Then every character: foreach (char ch in s) if (!char.IsLetterOrDigit(ch)) return false;',
        'A space or an underscore is not a letter or digit, so those fail.',
      ],
      solution: `string[] candidates = ["ada", "ab", "SuperLongUsername", "bad name", "grace99", "ok_"];

foreach (string c in candidates)
{
    Console.WriteLine($"{c,-18} {IsValidUsername(c)}");
}

bool IsValidUsername(string s)
{
    if (s.Length < 3 || s.Length > 12) return false;
    foreach (char ch in s)
    {
        if (!char.IsLetterOrDigit(ch)) return false;
    }
    return true;
}`,
    },
    {
      id: '6.6',
      lesson: '6.4',
      title: 'A JSON round-trip',
      level: 2,
      task: 'Serialize a `Person` to JSON, print it, then deserialize it back and print a greeting from the recovered object. Use `System.Text.Json`.',
      starter: `using System.Text.Json;

var ada = new Person("Ada", 36);

string json = ""; // TODO: serialize ada
Console.WriteLine(json);

Person back = ada; // TODO: deserialize json instead
Console.WriteLine($"Hello, {back.Name} ({back.Age})");

record Person(string Name, int Age);`,
      expected: `{"Name":"Ada","Age":36}
Hello, Ada (36)`,
      hints: [
        'JsonSerializer.Serialize(ada) gives the JSON string.',
        'JsonSerializer.Deserialize<Person>(json) rebuilds the object; it is non-null here so you can use ! or a null check.',
        'A positional record serializes its properties in declaration order.',
      ],
      solution: `using System.Text.Json;

var ada = new Person("Ada", 36);

string json = JsonSerializer.Serialize(ada);
Console.WriteLine(json);

Person back = JsonSerializer.Deserialize<Person>(json)!;
Console.WriteLine($"Hello, {back.Name} ({back.Age})");

record Person(string Name, int Age);`,
    },
    {
      id: '6.7',
      lesson: '6.5',
      title: 'Dependency injection',
      level: 3,
      task: 'A `Notifier` should not care how a message is delivered. Give it an `IMessageSink` through its constructor, then run it once with an `EmailSink` and once with an `SmsSink` - without changing `Notifier` itself. Each sink prefixes the message.',
      starter: `new Notifier(new EmailSink()).Send("Deployed");
new Notifier(new SmsSink()).Send("Deployed");

// TODO: an IMessageSink interface, two sinks, and a Notifier that is given one
interface IMessageSink
{
    void Deliver(string message);
}

class EmailSink : IMessageSink
{
    public void Deliver(string message) { }
}

class SmsSink : IMessageSink
{
    public void Deliver(string message) { }
}

class Notifier
{
    public Notifier(IMessageSink sink) { }
    public void Send(string message) { }
}`,
      expected: `EMAIL: Deployed
SMS: Deployed`,
      hints: [
        'EmailSink.Deliver prints $"EMAIL: {message}"; SmsSink prints $"SMS: {message}".',
        'Notifier stores the IMessageSink it is given and calls _sink.Deliver(message) in Send.',
        'The point: swapping email for SMS needs no change to Notifier - you just pass a different sink.',
      ],
      solution: `new Notifier(new EmailSink()).Send("Deployed");
new Notifier(new SmsSink()).Send("Deployed");

interface IMessageSink
{
    void Deliver(string message);
}

class EmailSink : IMessageSink
{
    public void Deliver(string message) => Console.WriteLine($"EMAIL: {message}");
}

class SmsSink : IMessageSink
{
    public void Deliver(string message) => Console.WriteLine($"SMS: {message}");
}

class Notifier
{
    private readonly IMessageSink _sink;
    public Notifier(IMessageSink sink) => _sink = sink;
    public void Send(string message) => _sink.Deliver(message);
}`,
    },
  ],
};
