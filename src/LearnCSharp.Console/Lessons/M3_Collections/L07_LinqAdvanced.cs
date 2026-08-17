using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Collections;

// Notes: docs/module-3/3.7.md
public sealed class L07_LinqAdvanced : LessonBase
{
    public override string Id => "3.7";
    public override string Title => "LINQ part 2: grouping, joining, aggregating";

    public override void Run()
    {
        List<Sale> sales =
        [
            new Sale("Ada",  "Books", 12.50m, new DateOnly(2026, 1, 5)),
            new Sale("Ada",  "Games", 45.00m, new DateOnly(2026, 1, 20)),
            new Sale("Ben",  "Books",  8.99m, new DateOnly(2026, 2, 2)),
            new Sale("Ben",  "Books", 22.00m, new DateOnly(2026, 2, 14)),
            new Sale("Cara", "Games", 60.00m, new DateOnly(2026, 2, 28)),
            new Sale("Cara", "Music", 15.75m, new DateOnly(2026, 3, 1)),
        ];

        Section("GroupBy - buckets keyed by something");

        foreach (IGrouping<string, Sale> group in sales.GroupBy(s => s.Category))
            Out($"category {group.Key}", $"{group.Count()} sales, total {group.Sum(s => s.Amount):F2}");

        Section("Group then project into a tidy shape");

        var byCustomer = sales
            .GroupBy(s => s.Customer)
            .Select(g => new
            {
                Customer = g.Key,
                Orders = g.Count(),
                Total = g.Sum(s => s.Amount),
                Best = g.Max(s => s.Amount),
            })
            .OrderByDescending(x => x.Total);

        foreach (var row in byCustomer)
            Out(row.Customer, $"{row.Orders} orders, total {row.Total:F2}, biggest {row.Best:F2}");

        Section("Grouping by a computed key");

        foreach (IGrouping<string, Sale> group in sales.GroupBy(s => s.Date.Month switch
                 {
                     1 or 2 or 3 => "Q1",
                     _ => "later",
                 }))
            Out($"quarter {group.Key}", $"{group.Count()} sales");

        foreach (IGrouping<bool, Sale> group in sales.GroupBy(s => s.Amount >= 20))
            Out(group.Key ? "big sales" : "small sales", group.Count());

        Section("ToLookup - a reusable one-to-many index");

        ILookup<string, Sale> lookup = sales.ToLookup(s => s.Customer);
        Out("lookup[\"Ben\"].Count()", lookup["Ben"].Count());
        Out("lookup[\"Nobody\"] - empty, not an error", lookup["Nobody"].Count());

        Section("Join - match two sequences on a key");

        List<Customer> customers =
        [
            new Customer("Ada", "London"),
            new Customer("Ben", "Leeds"),
            new Customer("Cara", "Cardiff"),
            new Customer("Dev", "Dover"),
        ];

        var joined = customers.Join(
            sales,
            customer => customer.Name,             // key from the first sequence
            sale => sale.Customer,                 // key from the second
            (customer, sale) => new { customer.City, sale.Category, sale.Amount });

        foreach (var row in joined.Take(4))
            Out(row.City, $"{row.Category} {row.Amount:F2}");

        Note("Dev has no sales, so Join drops them. Join keeps only matches - like an SQL INNER JOIN.");

        Section("GroupJoin - keep everyone, even with no matches");

        var withTotals = customers.GroupJoin(
            sales,
            customer => customer.Name,
            sale => sale.Customer,
            (customer, theirSales) => new { customer.Name, Total = theirSales.Sum(s => s.Amount) });

        foreach (var row in withTotals)
            Out(row.Name, row.Total.ToString("F2"));

        Section("SelectMany - flatten nested collections");

        List<Basket> baskets =
        [
            new Basket("Ada", ["apple", "pear"]),
            new Basket("Ben", ["bread"]),
            new Basket("Cara", ["milk", "eggs", "jam"]),
        ];

        Out("Select gives lists of lists", baskets.Select(b => b.Items).Count());
        Out("SelectMany flattens them", string.Join(", ", baskets.SelectMany(b => b.Items)));
        Out("with the owner attached", string.Join(", ",
            baskets.SelectMany(b => b.Items, (b, item) => $"{b.Owner}:{item}")));

        Section("Aggregate - fold a sequence into one value");

        int[] numbers = [1, 2, 3, 4, 5];
        Out("Aggregate sum", numbers.Aggregate((total, n) => total + n));
        Out("Aggregate product", numbers.Aggregate((total, n) => total * n));
        Out("with a seed of 100", numbers.Aggregate(100, (total, n) => total + n));

        string[] words = ["never", "gonna", "give"];
        Out("build a sentence", words.Aggregate((a, b) => a + " " + b));

        Section("Zip - pair up two sequences");

        string[] names = ["Ada", "Ben", "Cara"];
        int[] scores = [91, 64, 78];
        Out("Zip", string.Join(", ", names.Zip(scores, (n, s) => $"{n}={s}")));

        Section("Chunk - fixed-size batches");

        foreach (int[] chunk in Enumerable.Range(1, 10).Chunk(4))
            Out("chunk", string.Join(", ", chunk));

        Section("Generating sequences");

        Out("Enumerable.Range(5, 4)", string.Join(", ", Enumerable.Range(5, 4)));
        Out("Enumerable.Repeat(\"ab\", 3)", string.Join(", ", Enumerable.Repeat("ab", 3)));
        Out("Empty<int>()", Enumerable.Empty<int>().Count());

        Section("A realistic report in one query");

        var report = sales
            .Where(s => s.Amount > 10)
            .GroupBy(s => s.Category)
            .Select(g => new { Category = g.Key, Revenue = g.Sum(s => s.Amount), Count = g.Count() })
            .Where(x => x.Count >= 1)
            .OrderByDescending(x => x.Revenue)
            .ToList();

        foreach (var row in report)
            Out(row.Category, $"revenue {row.Revenue:F2} from {row.Count} sales");

        Section("Performance notes");

        Out("do Where before Select", "filter first so you transform fewer items");
        Out("avoid ToList() mid-chain", "it forces the whole sequence into memory");
        Out("Any() beats Count() > 0", "it stops at the first match");
        Out("enumerating twice", "runs the query twice - store it with ToList() if you need it twice");
    }
}

public record Sale(string Customer, string Category, decimal Amount, DateOnly Date);
public record Customer(string Name, string City);
public record Basket(string Owner, List<string> Items);
