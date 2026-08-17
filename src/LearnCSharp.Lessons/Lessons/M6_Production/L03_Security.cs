using System.Security.Cryptography;
using System.Text;
using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Production;

// Notes: docs/module-6/6.3.md
public sealed class L03_Security : LessonBase
{
    public override string Id => "6.3";
    public override string Title => "Security essentials";

    public override string Summary =>
        "Attackers do not break in - they walk through doors you left open. See SQL injection "
        + "work against naive code, learn why passwords are hashed slowly with a salt, and "
        + "which Random is safe for secrets.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Explain SQL injection and why parameterised queries defeat it",
        "Hash a password properly: salted, slow, and never reversible",
        "Choose RandomNumberGenerator over Random for anything secret",
    ];

    public override void Run()
    {
        Section("The mindset: every input is hostile until proven otherwise");

        Out("users mistype", "validation is UX (1.3)");
        Out("attackers craft", "input DESIGNED to be misread as code - that is security");
        Out("the pattern", "the same trick every time: data crossing into code");

        Section("SQL injection - the classic, demonstrated");

        // Naive code builds the query by gluing strings:
        string username = "ada";
        Out("query for 'ada'", BuildNaiveQuery(username));

        // The attacker types this as their "username":
        string attack = "' OR '1'='1";
        Out("attacker types", attack);
        Out("query becomes", BuildNaiveQuery(attack));
        Warn("WHERE Name = '' OR '1'='1' is TRUE for every row - the attacker just logged in "
           + "as everyone. With '; DROP TABLE Users; --' they delete the table instead.");

        Section("The fix: parameterised queries");

        Out("naive", "sql = \"... WHERE Name = '\" + input + \"'\"      data GLUED INTO code");
        Out("parameterised", "sql = \"... WHERE Name = @name\" + AddWithValue(\"@name\", input)");
        Out("why it works", "the input travels as pure DATA - the database never parses it as SQL");
        Out("simulated here", RunParameterised(attack));
        Note("Every real database library supports this. There is no input so weird that it "
           + "escapes a parameter - the fix is total, and it is LESS code than escaping.");

        Section("Storing passwords: never plaintext, never 'encrypted'");

        Out("plaintext", "one database leak = every account gone. Indefensible.");
        Out("encrypted", "the key sits next to the data - same leak, one extra step");
        Out("hashed", "one-way: you can CHECK a password without being able to RECOVER it");

        Section("But a fast hash is still wrong");

        string password = "hunter2";
        string sha = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        Out("SHA-256(\"hunter2\")", sha[..32] + "...");
        Warn("SHA-256 is built for SPEED - a GPU tries billions per second, so common "
           + "passwords fall in moments to a precomputed 'rainbow table'. Fast hashes are for "
           + "file integrity (5.9), not passwords.");

        Section("Right: a SALT plus a deliberately SLOW hash");

        // Salt: random bytes per user, stored beside the hash.
        // Slow: PBKDF2 (built into .NET) repeats the hash 100,000+ times.
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        var watch = System.Diagnostics.Stopwatch.StartNew();
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        watch.Stop();

        Out("salt (random, per user)", Convert.ToHexString(salt));
        Out("PBKDF2, 100,000 rounds", Convert.ToHexString(hash)[..32] + "...");
        Out("time for ONE attempt", $"{watch.ElapsedMilliseconds} ms");

        Note("Tune the rounds so one attempt costs ~100ms on your hardware: nothing to a "
           + "user logging in, ruinous to an attacker making billions of guesses. The salt kills rainbow tables: identical passwords get "
           + "different hashes. Same idea, stronger: bcrypt and Argon2.");

        // Verifying = re-run with the stored salt and compare.
        byte[] again = Rfc2898DeriveBytes.Pbkdf2("hunter2", salt, 100_000, HashAlgorithmName.SHA256, 32);
        byte[] wrong = Rfc2898DeriveBytes.Pbkdf2("hunter3", salt, 100_000, HashAlgorithmName.SHA256, 32);
        Out("verify \"hunter2\"", CryptographicOperations.FixedTimeEquals(hash, again));
        Out("verify \"hunter3\"", CryptographicOperations.FixedTimeEquals(hash, wrong));
        Note("FixedTimeEquals compares in constant time, so an attacker cannot learn bytes "
           + "from how QUICKLY the comparison fails - a timing attack.");

        Section("Random vs RandomNumberGenerator");

        Out("Random / Random.Shared", "statistical randomness - games, shuffles, dice (1.10)");
        Out("its weakness", "seeded and predictable: observe outputs, predict the rest");
        Out("RandomNumberGenerator", "cryptographic - OS entropy, unpredictable");

        Out("session token", Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)));
        Out("6-digit reset code", RandomNumberGenerator.GetInt32(100_000, 1_000_000));
        Warn("A password-reset code from new Random() is guessable. Anything an attacker "
           + "would WANT to guess - tokens, codes, keys, salts - comes from "
           + "RandomNumberGenerator, never Random.");

        Section("Secrets do not live in source code");

        Out("the mistake", "const string ApiKey = \"sk-live-...\" - now in git, forever");
        Out("why forever", "history survives deletion; leaked keys get scraped in minutes");
        Out("instead", "environment variables, user-secrets in development, a vault in production");
        Out("read like", "Environment.GetEnvironmentVariable(\"API_KEY\")");
        Note("This is why the deploy token for this very course lives in a GitHub secret, "
           + "not in the workflow file.");

        Section("The rest of the essentials, briefly");

        Out("least privilege", "the app's DB account cannot DROP TABLE = injection does less harm");
        Out("dependencies", "your NuGet packages are code you run - keep them updated");
        Out("error messages", "stack traces to users leak your internals; log privately instead");
        Out("do not invent crypto", "use the platform's reviewed primitives, as this lesson does");

        Section("The one-line summaries");

        Out("injection", "parameterise - data must never travel as code");
        Out("passwords", "salt + slow hash (PBKDF2/bcrypt/Argon2), compare in fixed time");
        Out("secrets", "environment or vault, never source");
        Out("randomness", "RandomNumberGenerator when an attacker would want to guess it");
    }

    private static string BuildNaiveQuery(string input) =>
        $"SELECT * FROM Users WHERE Name = '{input}'";

    /// <summary>
    /// Simulates what a parameterised query does: the input is handled as a VALUE,
    /// compared against data - never spliced into the SQL text.
    /// </summary>
    private static string RunParameterised(string input)
    {
        string[] realUsers = ["ada", "alan", "grace"];
        bool found = realUsers.Contains(input);      // the attack string is just... a string
        return found ? "user found" : $"no user called \"{input}\" - attack inert";
    }
}
