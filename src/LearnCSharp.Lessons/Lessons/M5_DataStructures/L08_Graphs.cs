using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.8.md
public sealed class L08_Graphs : LessonBase
{
    public override string Id => "5.8";
    public override string Title => "Graphs: breadth-first, depth-first and Dijkstra";

    public override string Summary =>
        "Nodes joined by edges: maps, networks, social connections. Breadth-first finds the "
        + "fewest steps, while Dijkstra finds the cheapest route.";

    public override IReadOnlyList<string> Objectives =>
    [
        "Represent a graph as an adjacency list or matrix",
        "Implement breadth-first and depth-first search",
        "Explain when Dijkstra gives a different answer from BFS",
    ];

    public override void Run()
    {
        Section("A graph is nodes joined by edges");

        Out("vertex / node", "a thing: a city, a person, a web page");
        Out("edge", "a connection between two nodes");
        Out("directed", "the edge goes one way (a one-way street, 'follows' on social media)");
        Out("undirected", "the edge goes both ways (a friendship, a two-way road)");
        Out("weighted", "each edge has a cost: distance, time, price");
        Out("a tree", "is just a graph with no cycles and one route between any two nodes");

        Section("The map we will use");

        Line();
        Line("        A ---- B");
        Line("        |    / |");
        Line("        |  /   |");
        Line("        C ---- D ---- E");
        Line("                      |");
        Line("                      F");

        Graph graph = new();
        graph.AddEdge("A", "B");
        graph.AddEdge("A", "C");
        graph.AddEdge("B", "C");
        graph.AddEdge("B", "D");
        graph.AddEdge("C", "D");
        graph.AddEdge("D", "E");
        graph.AddEdge("E", "F");

        Out("nodes", string.Join(", ", graph.Nodes));
        Out("neighbours of B", string.Join(", ", graph.Neighbours("B")));
        Out("neighbours of F", string.Join(", ", graph.Neighbours("F")));

        Section("Two ways to store a graph");

        Out("adjacency list", "a dictionary: node -> its neighbours. Best for sparse graphs.");
        Out("adjacency matrix", "a 2D grid of true/false. Best when almost everything connects.");

        Line();
        graph.PrintMatrix();

        Section("Breadth-first search - uses a QUEUE");

        Line();
        Out("BFS visit order from A", string.Join(" -> ", graph.BreadthFirst("A")));
        Line();
        graph.BreadthFirstVerbose("A");

        Note("BFS explores in rings: everything one step away, then everything two steps away. "
           + "That is why it finds the SHORTEST path in an unweighted graph.");

        Section("Depth-first search - uses a STACK (or recursion)");

        Out("DFS visit order from A (recursive)", string.Join(" -> ", graph.DepthFirstRecursive("A")));
        Out("DFS visit order from A (with a stack)", string.Join(" -> ", graph.DepthFirstIterative("A")));

        Note("DFS charges down one path as far as it can, then backtracks. It uses less memory than "
           + "BFS but does NOT find the shortest route.");

        Section("Shortest path with BFS");

        Out("A to F", string.Join(" -> ", graph.ShortestPath("A", "F")));
        Out("A to D", string.Join(" -> ", graph.ShortestPath("A", "D")));
        Out("hops from A to F", graph.ShortestPath("A", "F").Count - 1);

        Section("Connectivity");

        Out("is F reachable from A?", graph.CanReach("A", "F"));

        graph.AddNode("Z");                                   // an island with no edges
        Out("is Z reachable from A?", graph.CanReach("A", "Z"));
        Out("connected components", graph.ComponentCount());

        Section("Weighted graphs: Dijkstra's algorithm");

        WeightedGraph roads = new();
        roads.AddEdge("A", "B", 4);
        roads.AddEdge("A", "C", 2);
        roads.AddEdge("B", "C", 1);
        roads.AddEdge("B", "D", 5);
        roads.AddEdge("C", "D", 8);
        roads.AddEdge("C", "E", 10);
        roads.AddEdge("D", "E", 2);
        roads.AddEdge("D", "F", 6);
        roads.AddEdge("E", "F", 3);

        Line();
        Line("        A --4-- B --5-- D --6-- F");
        Line("        |     /         |     /");
        Line("        2   1           2   3");
        Line("        |  /            |  /");
        Line("        C -----10------ E");
        Line("         \\____8_________/   (C-D)");

        (List<string> path, int cost) = roads.Dijkstra("A", "F");
        Out("cheapest A to F", string.Join(" -> ", path));
        Out("total cost", cost);

        Out("fewest HOPS A to F", string.Join(" -> ", graph.ShortestPath("A", "F")));
        Note("Fewest hops is not the same as cheapest. BFS counts steps; Dijkstra adds up weights.");

        Line();
        Out("all costs from A", string.Join(", ",
            roads.AllDistances("A").OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}")));

        Section("Where graphs are used");

        Out("satnav and maps", "Dijkstra and A* over a road network");
        Out("social networks", "friends-of-friends, suggestions, degrees of separation");
        Out("the web", "pages and links; PageRank is a graph algorithm");
        Out("dependencies", "build order, module loading - a topological sort");
        Out("games", "pathfinding, state machines, quest trees");
        Out("networks", "routing packets across the internet");

        Section("Complexity");

        Out("BFS and DFS", "O(V + E) - each node and edge is looked at once");
        Out("Dijkstra with a priority queue", "O((V + E) log V)");
        Out("adjacency list memory", "O(V + E)");
        Out("adjacency matrix memory", "O(V^2) - wasteful unless the graph is dense");
    }
}

/// <summary>An unweighted, undirected graph stored as an adjacency list.</summary>
public class Graph
{
    private readonly Dictionary<string, List<string>> _adjacency = new();

    public IEnumerable<string> Nodes => _adjacency.Keys.OrderBy(n => n);

    public void AddNode(string node)
    {
        if (!_adjacency.ContainsKey(node)) _adjacency[node] = new List<string>();
    }

    public void AddEdge(string from, string to)
    {
        AddNode(from);
        AddNode(to);
        _adjacency[from].Add(to);
        _adjacency[to].Add(from);                 // undirected, so record it both ways
    }

    public IEnumerable<string> Neighbours(string node) =>
        _adjacency.TryGetValue(node, out List<string>? list) ? list : [];

    /// <summary>BFS: a QUEUE gives you level-by-level exploration.</summary>
    public List<string> BreadthFirst(string start)
    {
        List<string> order = new();
        HashSet<string> seen = [start];           // mark as seen when you ENQUEUE, not when you visit
        Queue<string> pending = new([start]);

        while (pending.Count > 0)
        {
            string node = pending.Dequeue();
            order.Add(node);

            foreach (string neighbour in Neighbours(node))
            {
                if (seen.Add(neighbour))          // Add returns false if it was already there
                    pending.Enqueue(neighbour);
            }
        }

        return order;
    }

    public void BreadthFirstVerbose(string start)
    {
        HashSet<string> seen = [start];
        Queue<string> pending = new([start]);
        int step = 0;

        while (pending.Count > 0)
        {
            string node = pending.Dequeue();
            step++;
            List<string> added = new();

            foreach (string neighbour in Neighbours(node))
                if (seen.Add(neighbour)) { pending.Enqueue(neighbour); added.Add(neighbour); }

            Console.WriteLine($"      step {step}: visit {node}, queue now [{string.Join(", ", pending)}]"
                            + (added.Count > 0 ? $", added {string.Join(", ", added)}" : ""));
        }
    }

    /// <summary>DFS by recursion - the call stack IS the stack.</summary>
    public List<string> DepthFirstRecursive(string start)
    {
        List<string> order = new();
        HashSet<string> seen = new();
        Visit(start, seen, order);
        return order;
    }

    private void Visit(string node, HashSet<string> seen, List<string> order)
    {
        if (!seen.Add(node)) return;              // already been here

        order.Add(node);

        foreach (string neighbour in Neighbours(node))
            Visit(neighbour, seen, order);
    }

    /// <summary>The same search with an explicit Stack - identical logic, no recursion.</summary>
    public List<string> DepthFirstIterative(string start)
    {
        List<string> order = new();
        HashSet<string> seen = new();
        Stack<string> pending = new([start]);

        while (pending.Count > 0)
        {
            string node = pending.Pop();
            if (!seen.Add(node)) continue;

            order.Add(node);

            // Push in reverse so the first neighbour is explored first.
            foreach (string neighbour in Neighbours(node).AsEnumerable().Reverse())
                if (!seen.Contains(neighbour)) pending.Push(neighbour);
        }

        return order;
    }

    /// <summary>BFS, but remembering how we reached each node so the route can be rebuilt.</summary>
    public List<string> ShortestPath(string start, string end)
    {
        Dictionary<string, string?> cameFrom = new() { [start] = null };
        Queue<string> pending = new([start]);

        while (pending.Count > 0)
        {
            string node = pending.Dequeue();
            if (node == end) break;

            foreach (string neighbour in Neighbours(node))
            {
                if (cameFrom.ContainsKey(neighbour)) continue;
                cameFrom[neighbour] = node;
                pending.Enqueue(neighbour);
            }
        }

        if (!cameFrom.ContainsKey(end)) return [];

        // Walk the breadcrumbs backwards, then flip the result.
        List<string> path = new();
        for (string? at = end; at is not null; at = cameFrom[at]) path.Add(at);
        path.Reverse();
        return path;
    }

    public bool CanReach(string start, string end) => BreadthFirst(start).Contains(end);

    public int ComponentCount()
    {
        HashSet<string> seen = new();
        int components = 0;

        foreach (string node in Nodes)
        {
            if (seen.Contains(node)) continue;
            components++;
            foreach (string reached in BreadthFirst(node)) seen.Add(reached);
        }

        return components;
    }

    public void PrintMatrix()
    {
        List<string> nodes = Nodes.ToList();

        Console.WriteLine("      adjacency matrix:");
        Console.WriteLine("         " + string.Join(" ", nodes.Select(n => n.PadLeft(2))));

        foreach (string row in nodes)
        {
            string cells = string.Join(" ", nodes.Select(col => (Neighbours(row).Contains(col) ? "1" : "0").PadLeft(2)));
            Console.WriteLine($"      {row,2} {cells}");
        }
    }
}

/// <summary>A weighted graph, plus Dijkstra's shortest-path algorithm.</summary>
public class WeightedGraph
{
    private readonly Dictionary<string, List<(string To, int Weight)>> _adjacency = new();

    public void AddEdge(string from, string to, int weight)
    {
        if (!_adjacency.ContainsKey(from)) _adjacency[from] = new();
        if (!_adjacency.ContainsKey(to)) _adjacency[to] = new();

        _adjacency[from].Add((to, weight));
        _adjacency[to].Add((from, weight));
    }

    public Dictionary<string, int> AllDistances(string start)
    {
        Dictionary<string, int> distance = _adjacency.Keys.ToDictionary(n => n, _ => int.MaxValue);
        distance[start] = 0;

        // The priority queue always hands back the cheapest unexplored node.
        PriorityQueue<string, int> pending = new();
        pending.Enqueue(start, 0);
        HashSet<string> settled = new();

        while (pending.Count > 0)
        {
            string node = pending.Dequeue();
            if (!settled.Add(node)) continue;

            foreach ((string to, int weight) in _adjacency[node])
            {
                int candidate = distance[node] + weight;

                if (candidate < distance[to])      // found a cheaper way in
                {
                    distance[to] = candidate;
                    pending.Enqueue(to, candidate);
                }
            }
        }

        return distance;
    }

    public (List<string> Path, int Cost) Dijkstra(string start, string end)
    {
        Dictionary<string, int> distance = _adjacency.Keys.ToDictionary(n => n, _ => int.MaxValue);
        Dictionary<string, string?> cameFrom = new() { [start] = null };
        distance[start] = 0;

        PriorityQueue<string, int> pending = new();
        pending.Enqueue(start, 0);
        HashSet<string> settled = new();

        while (pending.Count > 0)
        {
            string node = pending.Dequeue();
            if (!settled.Add(node)) continue;
            if (node == end) break;

            foreach ((string to, int weight) in _adjacency[node])
            {
                int candidate = distance[node] + weight;

                if (candidate < distance[to])
                {
                    distance[to] = candidate;
                    cameFrom[to] = node;
                    pending.Enqueue(to, candidate);
                }
            }
        }

        if (distance[end] == int.MaxValue) return ([], -1);

        List<string> path = new();
        for (string? at = end; at is not null; at = cameFrom[at]) path.Add(at);
        path.Reverse();

        return (path, distance[end]);
    }
}
