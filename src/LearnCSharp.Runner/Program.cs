using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LearnCSharp.Runner;

public static class Entry
{
    // The runtime is driven through the [JSExport] methods below; there is nothing to do at startup.
    public static void Main() { }
}

/// <summary>
/// Compiles and runs a student's program. These are two calls so that the caller can start its
/// "this program is taking too long" timer after compiling, which can be slow on a phone.
/// </summary>
public static partial class Runner
{
    const int MaxErrors = 20;
    const int MaxOutputChars = 200_000;

    // The same set that `dotnet new console` turns on through ImplicitUsings.
    const string GlobalUsings = """
        global using System;
        global using System.IO;
        global using System.Linq;
        global using System.Net.Http;
        global using System.Threading;
        global using System.Threading.Tasks;
        global using System.Collections.Generic;
        """;

    static readonly List<MetadataReference> References = new();
    static byte[]? _compiled;
    static string? _entryPointName;
    static int _entryPointParameters;

    [JSExport]
    public static void Initialise(byte[] referencesZip)
    {
        UseBritishFormats();
        using var archive = new ZipArchive(new MemoryStream(referencesZip));
        foreach (var entry in archive.Entries)
        {
            using var source = entry.Open();
            using var copy = new MemoryStream();
            source.CopyTo(copy);
            References.Add(MetadataReference.CreateFromImage(ImmutableArray.Create(copy.ToArray()), filePath: entry.Name));
        }
    }

    [JSExport]
    public static string Compile(string code)
    {
        var timer = Stopwatch.StartNew();
        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);
        var compilation = CSharpCompilation.Create(
            "student",
            [CSharpSyntaxTree.ParseText(code, parseOptions), CSharpSyntaxTree.ParseText(GlobalUsings, parseOptions)],
            References,
            new CSharpCompilationOptions(
                OutputKind.ConsoleApplication,
                nullableContextOptions: NullableContextOptions.Enable,
                optimizationLevel: OptimizationLevel.Release,
                // There is only one thread here.
                concurrentBuild: false));

        using var image = new MemoryStream();
        var emit = compilation.Emit(image);

        var errors = new JsonArray();
        foreach (var error in emit.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Take(MaxErrors))
            errors.Add(Describe(error));

        _compiled = null;
        if (emit.Success)
        {
            var entryPoint = compilation.GetEntryPoint(default)!;
            _compiled = image.ToArray();
            _entryPointName = entryPoint.MetadataName;
            _entryPointParameters = entryPoint.Parameters.Length;
        }

        return new JsonObject
        {
            ["ok"] = emit.Success,
            ["errors"] = errors,
            ["ms"] = timer.ElapsedMilliseconds,
        }.ToJsonString();
    }

    [JSExport]
    public static async Task<string> Execute(string input)
    {
        var result = new JsonObject();
        var timer = Stopwatch.StartNew();
        var output = new CappedWriter(MaxOutputChars);
        var (previousOut, previousError) = (Console.Out, Console.Error);
        Console.SetOut(output);
        Console.SetError(output);
        Console.SetIn(new StringReader(input));
        try
        {
            var returned = FindEntryPoint(Assembly.Load(_compiled!)).Invoke(
                null, _entryPointParameters == 0 ? null : [Array.Empty<string>()]);
            if (returned is Task task) await task;
        }
        catch (Exception thrown)
        {
            var exception = thrown is TargetInvocationException { InnerException: { } inner } ? inner : thrown;
            if (exception is OutputLimitException)
                result["truncated"] = true;
            else
                result["exception"] = new JsonObject { ["type"] = exception.GetType().FullName, ["message"] = exception.Message };
        }
        finally
        {
            Console.SetOut(previousOut);
            Console.SetError(previousError);
        }
        result["output"] = output.ToString();
        result["ms"] = timer.ElapsedMilliseconds;
        return result.ToJsonString();
    }

    /// <summary>
    /// Finds the method the student wrote rather than Assembly.EntryPoint. An async program's
    /// EntryPoint is a compiler-made wrapper that blocks on the task, and blocking the only thread
    /// throws here. Calling the real method lets us await it instead.
    /// </summary>
    static MethodInfo FindEntryPoint(Assembly assembly) =>
        assembly.EntryPoint!.DeclaringType!
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .First(m => m.Name == _entryPointName && m.GetParameters().Length == _entryPointParameters);

    static string Describe(Diagnostic error)
    {
        var message = $"{error.GetMessage(CultureInfo.InvariantCulture)} ({error.Id})";
        return error.Location.IsInSource
            ? $"Line {error.Location.GetLineSpan().StartLinePosition.Line + 1}: {message}"
            : message;
    }

    /// <summary>
    /// The browser build carries no regional data, so the default formats are American-flavoured
    /// with "¤" for currency. Students on this course are in the UK, so make those match.
    /// </summary>
    static void UseBritishFormats()
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.CurrencySymbol = "£";
        culture.NumberFormat.CurrencyPositivePattern = 0;
        culture.NumberFormat.CurrencyNegativePattern = 1;
        culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
        culture.DateTimeFormat.LongDatePattern = "dddd, d MMMM yyyy";
        culture.DateTimeFormat.ShortTimePattern = "HH:mm";
        culture.DateTimeFormat.LongTimePattern = "HH:mm:ss";
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    sealed class OutputLimitException : Exception;

    /// <summary>Stops a program that prints forever before it can use up all the memory.</summary>
    sealed class CappedWriter(int limit) : TextWriter
    {
        readonly StringBuilder _text = new();

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            if (_text.Length >= limit) throw new OutputLimitException();
            _text.Append(value);
        }

        public override void Write(string? value)
        {
            if (value is null) return;
            if (_text.Length + value.Length > limit)
            {
                _text.Append(value, 0, limit - _text.Length);
                throw new OutputLimitException();
            }
            _text.Append(value);
        }

        public override void Write(char[] buffer, int index, int count) => Write(new string(buffer, index, count));

        public override string ToString() => _text.ToString();
    }
}
