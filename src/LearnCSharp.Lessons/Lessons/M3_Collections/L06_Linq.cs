using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Collections;

// Notes: docs/module-3/3.6.md
public sealed class L06_Linq : LessonBase
{
    public override string Id => "3.6";
    public override string Title => "LINQ part 1: filtering, projecting, ordering";

    public override void Run()
    {
        List<Pupil> pupils =
        [
            new Pupil("Ada",   10, "Maths",   91),
            new Pupil("Ben",   10, "Maths",   64),
            new Pupil("Cara",  11, "Maths",   78),
            new Pupil("Dev",   10, "English", 55),
            new Pupil("Eve",   11, "English", 88),
            new Pupil("Femi",  11, "Science", 72),
            new Pupil("Gita",  10, "Science", 95),
        ];

        Section("Where - keep the ones that match");

        Out("mark > 80", Names(pupils.Where(p => p.Mark > 80)));
        Out("year 10 AND mark > 70", Names(pupils.Where(p => p.Year == 10 && p.Mark > 70)));

        Section("Select - turn each item into something else");

        Out("just the names", string.Join(", ", pupils.Select(p => p.Name)));
        Out("names in capitals", string.Join(", ", pupils.Select(p => p.Name.ToUpperInvariant())));
        Out("name + mark", string.Join(", ", pupils.Take(3).Select(p => $"{p.Name}:{p.Mark}")));

        // Select into an anonymous type - a throwaway shape with just the fields you need.
        var summaries = pupils.Take(2).Select(p => new { p.Name, Passed = p.Mark >= 60 });
        foreach (var s in summaries) Out(s.Name, s.Passed);

        Section("OrderBy, ThenBy, OrderByDescending");

        Out("by mark", Names(pupils.OrderBy(p => p.Mark)));
        Out("by mark, highest first", Names(pupils.OrderByDescending(p => p.Mark)));
        Out("by subject then mark", string.Join(", ",
            pupils.OrderBy(p => p.Subject).ThenByDescending(p => p.Mark)
                  .Select(p => $"{p.Subject[..3]}-{p.Name}")));

        Section("Chaining is where LINQ shines");

        Out("year 10, passed, best first", string.Join(", ",
            pupils.Where(p => p.Year == 10)
                  .Where(p => p.Mark >= 60)
                  .OrderByDescending(p => p.Mark)
                  .Select(p => $"{p.Name} ({p.Mark})")));

        Section("Getting single items");

        Out("First(mark > 80)", pupils.First(p => p.Mark > 80).Name);
        Out("FirstOrDefault(mark > 99)", pupils.FirstOrDefault(p => p.Mark > 99)?.Name);
        Out("Last()", pupils.Last().Name);
        Out("Single(name == \"Ada\")", pupils.Single(p => p.Name == "Ada").Name);

        try { pupils.First(p => p.Mark > 99); }
        catch (InvalidOperationException) { Out("First with no match", "InvalidOperationException"); }

        Note("Use FirstOrDefault when 'nothing found' is a normal outcome. Single insists there is "
           + "EXACTLY one match and throws if there are two.");

        Section("Aggregation");

        Out("Count()", pupils.Count());
        Out("Count(mark >= 70)", pupils.Count(p => p.Mark >= 70));
        Out("Sum of marks", pupils.Sum(p => p.Mark));
        Out("Average mark", Math.Round(pupils.Average(p => p.Mark), 2));
        Out("Min mark", pupils.Min(p => p.Mark));
        Out("Max mark", pupils.Max(p => p.Mark));
        Out("MinBy - who scored lowest", pupils.MinBy(p => p.Mark)?.Name);
        Out("MaxBy - who scored highest", pupils.MaxBy(p => p.Mark)?.Name);

        Section("Any and All");

        Out("Any(mark > 90)", pupils.Any(p => p.Mark > 90));
        Out("Any(mark > 99)", pupils.Any(p => p.Mark > 99));
        Out("All(mark >= 50)", pupils.All(p => p.Mark >= 50));
        Out("Any() - is there anything at all", pupils.Any());

        Note("Prefer Any() to Count() > 0. Any() stops at the first match; Count() walks the lot.");

        Section("Paging: Skip and Take");

        Out("Take(3)", Names(pupils.Take(3)));
        Out("Skip(2).Take(2)", Names(pupils.Skip(2).Take(2)));
        Out("TakeLast(2)", Names(pupils.TakeLast(2)));
        Out("page 2, size 3", Names(pupils.Skip(3).Take(3)));

        Section("Distinct and set operations");

        Out("Distinct subjects", string.Join(", ", pupils.Select(p => p.Subject).Distinct()));
        Out("DistinctBy year", string.Join(", ", pupils.DistinctBy(p => p.Year).Select(p => p.Name)));

        int[] a = [1, 2, 3, 4];
        int[] b = [3, 4, 5];
        Out("a.Union(b)", string.Join(", ", a.Union(b)));
        Out("a.Intersect(b)", string.Join(", ", a.Intersect(b)));
        Out("a.Except(b)", string.Join(", ", a.Except(b)));

        Section("Query syntax - the SQL-looking alternative");

        // Identical meaning to the method chain; pick whichever reads better.
        IEnumerable<string> topInYear10 =
            from pupil in pupils
            where pupil.Year == 10
            orderby pupil.Mark descending
            select $"{pupil.Name} ({pupil.Mark})";

        Out("query syntax", string.Join(", ", topInYear10));
        Out("same in method syntax", string.Join(", ",
            pupils.Where(p => p.Year == 10).OrderByDescending(p => p.Mark)
                  .Select(p => $"{p.Name} ({p.Mark})")));

        Section("Materialise the result when you need it fixed");

        Out("ToList()", pupils.Where(p => p.Mark > 80).ToList().Count);
        Out("ToArray()", pupils.Select(p => p.Name).ToArray().Length);
        Out("ToDictionary(name -> mark)", pupils.ToDictionary(p => p.Name, p => p.Mark)["Gita"]);
        Out("ToHashSet() of years", string.Join(", ", pupils.Select(p => p.Year).ToHashSet()));
    }

    private static string Names(IEnumerable<Pupil> pupils) => string.Join(", ", pupils.Select(p => p.Name));
}

public record Pupil(string Name, int Year, string Subject, int Mark);
