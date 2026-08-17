using LearnCSharp.Core;

namespace LearnCSharp.Lessons.DataStructures;

// Notes: docs/module-5/5.7.md
public sealed class L07_Trees : LessonBase
{
    public override string Id => "5.7";
    public override string Title => "Binary search trees and traversals";

    public override void Run()
    {
        Section("Tree vocabulary");

        Out("root", "the single node at the top");
        Out("child / parent", "the nodes directly below / above");
        Out("leaf", "a node with no children");
        Out("subtree", "any node, taken together with everything below it");
        Out("height", "the longest path from the root down to a leaf");
        Out("binary tree", "every node has at most two children");

        Section("The binary search tree rule");

        Out("left subtree", "everything SMALLER than this node");
        Out("right subtree", "everything LARGER than this node");
        Out("consequence", "an in-order walk visits the values in sorted order, for free");

        Section("Building one");

        BinarySearchTree tree = new();
        foreach (int value in new[] { 50, 30, 70, 20, 40, 60, 80, 35 })
            tree.Insert(value);

        Line();
        tree.Print();

        Out("Count", tree.Count);
        Out("Height", tree.Height);
        Out("Min", tree.Min());
        Out("Max", tree.Max());

        Section("Searching is O(height)");

        Line();
        tree.SearchVerbose(35);
        Line();
        tree.SearchVerbose(45);

        Out("Contains(60)", tree.Contains(60));
        Out("Contains(99)", tree.Contains(99));

        Section("The four traversals");

        Out("in-order   (left, node, right)", string.Join(", ", tree.InOrder()));
        Out("pre-order  (node, left, right)", string.Join(", ", tree.PreOrder()));
        Out("post-order (left, right, node)", string.Join(", ", tree.PostOrder()));
        Out("level-order (breadth first)", string.Join(", ", tree.LevelOrder()));

        Line();
        Line("in-order   -> sorted output; used to list a tree in order");
        Line("pre-order  -> copies a tree, or writes it out to a file");
        Line("post-order -> deletes a tree safely (children before parents); evaluates expressions");
        Line("level-order-> shortest path in an unweighted tree; printing it row by row");

        Section("Deleting a node - three cases");

        BinarySearchTree deleteDemo = new();
        foreach (int v in new[] { 50, 30, 70, 20, 40, 60, 80 }) deleteDemo.Insert(v);

        Out("start", string.Join(", ", deleteDemo.InOrder()));

        deleteDemo.Delete(20);
        Out("delete 20 (a leaf - just remove it)", string.Join(", ", deleteDemo.InOrder()));

        deleteDemo.Delete(30);
        Out("delete 30 (one child - promote it)", string.Join(", ", deleteDemo.InOrder()));

        deleteDemo.Delete(50);
        Out("delete 50 (two children - use the successor)", string.Join(", ", deleteDemo.InOrder()));

        Note("With two children you replace the node with its in-order SUCCESSOR: the smallest value "
           + "in the right subtree. That keeps the ordering rule intact.");

        Section("Balance is everything");

        BinarySearchTree balanced = new();
        foreach (int v in new[] { 50, 30, 70, 20, 40, 60, 80 }) balanced.Insert(v);

        BinarySearchTree degenerate = new();
        foreach (int v in new[] { 10, 20, 30, 40, 50, 60, 70 }) degenerate.Insert(v);

        Out("balanced tree, 7 nodes, height", balanced.Height);
        Out("sorted input, 7 nodes, height", degenerate.Height);

        Line();
        Line("inserting already-sorted data gives you this:");
        degenerate.Print();

        Warn("A degenerate tree is just a linked list, so search falls from O(log n) back to O(n). "
           + "Real systems use self-balancing trees (AVL, red-black) to prevent it.");

        Out("balanced search on 1,000,000 nodes", $"~{Math.Ceiling(Math.Log2(1_000_000))} comparisons");
        Out("degenerate search on 1,000,000", "up to 1,000,000 comparisons");

        Section("Expression trees");

        // (3 + 4) * 5
        ExpressionNode expression = new("*",
            new ExpressionNode("+", new ExpressionNode("3"), new ExpressionNode("4")),
            new ExpressionNode("5"));

        Out("in-order (infix)", expression.InOrder());
        Out("pre-order (prefix / Polish)", expression.PreOrder());
        Out("post-order (postfix / RPN)", expression.PostOrder());
        Out("evaluated", expression.Evaluate());

        Section("Where trees show up");

        Out("file systems", "folders containing folders");
        Out("HTML and XML", "the DOM is a tree");
        Out("databases", "B-trees power almost every index");
        Out("compilers", "your source code becomes an abstract syntax tree");
        Out("games", "decision trees, scene graphs, spatial partitioning");

        Section(".NET's balanced-tree collections");

        SortedSet<int> sortedSet = [50, 30, 70, 20];
        Out("SortedSet (a red-black tree)", string.Join(", ", sortedSet));

        SortedDictionary<string, int> sortedDictionary = new() { ["b"] = 2, ["a"] = 1, ["c"] = 3 };
        Out("SortedDictionary keys", string.Join(", ", sortedDictionary.Keys));
        Note("Both stay sorted and guarantee O(log n). A Dictionary is O(1) but unordered - pick "
           + "based on whether you need the order.");
    }
}

public class TreeNode
{
    public TreeNode(int value) => Value = value;

    public int Value { get; set; }
    public TreeNode? Left { get; set; }
    public TreeNode? Right { get; set; }
}

public class BinarySearchTree
{
    private TreeNode? _root;

    public int Count { get; private set; }

    public int Height => HeightOf(_root);

    public void Insert(int value)
    {
        _root = Insert(_root, value);
        Count++;
    }

    private static TreeNode Insert(TreeNode? node, int value)
    {
        if (node is null) return new TreeNode(value);        // found the empty spot

        if (value < node.Value) node.Left = Insert(node.Left, value);
        else node.Right = Insert(node.Right, value);

        return node;
    }

    public bool Contains(int value)
    {
        TreeNode? current = _root;

        while (current is not null)
        {
            if (value == current.Value) return true;
            current = value < current.Value ? current.Left : current.Right;   // halve the search
        }

        return false;
    }

    public void SearchVerbose(int value)
    {
        TreeNode? current = _root;
        int step = 0;
        Console.WriteLine($"      searching for {value}");

        while (current is not null)
        {
            step++;
            if (value == current.Value)
            {
                Console.WriteLine($"      step {step}: at {current.Value} -> FOUND");
                return;
            }

            string direction = value < current.Value ? "smaller, go left" : "bigger, go right";
            Console.WriteLine($"      step {step}: at {current.Value} -> {value} is {direction}");
            current = value < current.Value ? current.Left : current.Right;
        }

        Console.WriteLine($"      step {step + 1}: nothing here - not found");
    }

    public int Min()
    {
        TreeNode current = _root ?? throw new InvalidOperationException("The tree is empty.");
        while (current.Left is not null) current = current.Left;      // smallest = leftmost
        return current.Value;
    }

    public int Max()
    {
        TreeNode current = _root ?? throw new InvalidOperationException("The tree is empty.");
        while (current.Right is not null) current = current.Right;    // largest = rightmost
        return current.Value;
    }

    public void Delete(int value)
    {
        _root = Delete(_root, value);
    }

    private static TreeNode? Delete(TreeNode? node, int value)
    {
        if (node is null) return null;

        if (value < node.Value) { node.Left = Delete(node.Left, value); return node; }
        if (value > node.Value) { node.Right = Delete(node.Right, value); return node; }

        // Found it. Case 1 and 2: no children, or one child - promote whatever is there.
        if (node.Left is null) return node.Right;
        if (node.Right is null) return node.Left;

        // Case 3: two children. Copy in the in-order successor, then delete that successor.
        TreeNode successor = node.Right;
        while (successor.Left is not null) successor = successor.Left;

        node.Value = successor.Value;
        node.Right = Delete(node.Right, successor.Value);
        return node;
    }

    public List<int> InOrder()
    {
        List<int> result = new();
        InOrder(_root, result);
        return result;
    }

    private static void InOrder(TreeNode? node, List<int> result)
    {
        if (node is null) return;

        InOrder(node.Left, result);      // 1. everything smaller
        result.Add(node.Value);          // 2. this node
        InOrder(node.Right, result);     // 3. everything larger
    }

    public List<int> PreOrder()
    {
        List<int> result = new();
        PreOrder(_root, result);
        return result;
    }

    private static void PreOrder(TreeNode? node, List<int> result)
    {
        if (node is null) return;

        result.Add(node.Value);
        PreOrder(node.Left, result);
        PreOrder(node.Right, result);
    }

    public List<int> PostOrder()
    {
        List<int> result = new();
        PostOrder(_root, result);
        return result;
    }

    private static void PostOrder(TreeNode? node, List<int> result)
    {
        if (node is null) return;

        PostOrder(node.Left, result);
        PostOrder(node.Right, result);
        result.Add(node.Value);
    }

    /// <summary>Level order needs a QUEUE rather than recursion.</summary>
    public List<int> LevelOrder()
    {
        List<int> result = new();
        if (_root is null) return result;

        Queue<TreeNode> pending = new();
        pending.Enqueue(_root);

        while (pending.Count > 0)
        {
            TreeNode node = pending.Dequeue();
            result.Add(node.Value);

            if (node.Left is not null) pending.Enqueue(node.Left);
            if (node.Right is not null) pending.Enqueue(node.Right);
        }

        return result;
    }

    private static int HeightOf(TreeNode? node) =>
        node is null ? 0 : 1 + Math.Max(HeightOf(node.Left), HeightOf(node.Right));

    /// <summary>Draws the tree sideways: the root on the left, so the biggest values are at the top.</summary>
    public void Print() => Print(_root, 0);

    private static void Print(TreeNode? node, int depth)
    {
        if (node is null) return;

        Print(node.Right, depth + 1);
        Console.WriteLine(new string(' ', 6 + depth * 4) + node.Value);
        Print(node.Left, depth + 1);
    }
}

/// <summary>A tree whose leaves are numbers and whose branches are operators.</summary>
public class ExpressionNode
{
    public ExpressionNode(string value, ExpressionNode? left = null, ExpressionNode? right = null)
    {
        Value = value;
        Left = left;
        Right = right;
    }

    public string Value { get; }
    public ExpressionNode? Left { get; }
    public ExpressionNode? Right { get; }

    private bool IsLeaf => Left is null && Right is null;

    public string InOrder() => IsLeaf ? Value : $"({Left!.InOrder()} {Value} {Right!.InOrder()})";

    public string PreOrder() => IsLeaf ? Value : $"{Value} {Left!.PreOrder()} {Right!.PreOrder()}";

    public string PostOrder() => IsLeaf ? Value : $"{Left!.PostOrder()} {Right!.PostOrder()} {Value}";

    public double Evaluate()
    {
        if (IsLeaf) return double.Parse(Value);

        double left = Left!.Evaluate();
        double right = Right!.Evaluate();

        return Value switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => left / right,
            _ => throw new ArgumentException($"Unknown operator {Value}"),
        };
    }
}
