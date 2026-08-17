using LearnCSharp.Core;

namespace LearnCSharp.Lessons.Collections;

// Notes: docs/module-3/3.3.md
public sealed class L03_StacksQueues : LessonBase
{
    public override string Id => "3.3";
    public override string Title => "Stack, Queue and LinkedList";

    public override void Run()
    {
        Section("Stack: last in, first out (LIFO)");

        Stack<string> plates = new();
        plates.Push("bottom");
        plates.Push("middle");
        plates.Push("top");

        Out("Count", plates.Count);
        Out("Peek() - look, do not take", plates.Peek());
        Out("Pop()", plates.Pop());
        Out("Pop()", plates.Pop());
        Out("Count now", plates.Count);

        Out("TryPop on an empty stack", new Stack<int>().TryPop(out int _));

        Section("Stack in action: reversing");

        Stack<char> letters = new();
        foreach (char c in "stressed") letters.Push(c);
        Out("\"stressed\" reversed", string.Concat(letters));

        Section("Stack in action: matching brackets");

        foreach (string expression in new[] { "(a+b)*[c-d]", "(a+b))", "{[()]}", "{[(])}" })
            Out($"IsBalanced(\"{expression}\")", IsBalanced(expression));

        Section("Queue: first in, first out (FIFO)");

        Queue<string> checkout = new();
        checkout.Enqueue("Ada");
        checkout.Enqueue("Alan");
        checkout.Enqueue("Grace");

        Out("Count", checkout.Count);
        Out("Peek() - who is next", checkout.Peek());
        Out("Dequeue()", checkout.Dequeue());
        Out("Dequeue()", checkout.Dequeue());
        Out("still waiting", string.Join(", ", checkout));

        Section("Queue in action: a print spooler");

        Queue<string> printJobs = new(["report.pdf", "essay.docx", "photo.png"]);
        while (printJobs.Count > 0)
            Out("printing", printJobs.Dequeue());

        Section("PriorityQueue: lowest priority number comes out first");

        PriorityQueue<string, int> hospital = new();
        hospital.Enqueue("sprained ankle", 5);
        hospital.Enqueue("chest pain", 1);
        hospital.Enqueue("broken arm", 3);

        while (hospital.Count > 0)
            Out("next patient", hospital.Dequeue());

        Section("LinkedList: cheap inserts anywhere, no indexing");

        LinkedList<int> chain = new([10, 20, 30]);
        Out("chain", string.Join(" -> ", chain));
        Out("First", chain.First?.Value);
        Out("Last", chain.Last?.Value);

        LinkedListNode<int> twenty = chain.Find(20)!;
        chain.AddAfter(twenty, 25);
        chain.AddBefore(twenty, 15);
        chain.AddFirst(5);

        Out("after inserting 5, 15, 25", string.Join(" -> ", chain));
        // chain[2];   <- does not exist: a linked list has no index

        Section("Choosing a collection");

        Out("List<T>", "ordered, indexed, grows - the default");
        Out("Dictionary<K,V>", "look something up by key, O(1)");
        Out("HashSet<T>", "unique items, fast Contains");
        Out("Stack<T>", "undo history, back button, depth-first search, call stack");
        Out("Queue<T>", "print jobs, breadth-first search, anything first-come-first-served");
        Out("PriorityQueue<T,P>", "scheduling, A*, Dijkstra");
        Out("LinkedList<T>", "lots of inserts and deletes in the middle - rare in practice");

        Note("Lesson 5.4 builds a Stack and a Queue from scratch so you can see how they work inside.");
    }

    /// <summary>Classic stack algorithm: every opening bracket must be closed in the right order.</summary>
    private static bool IsBalanced(string expression)
    {
        Stack<char> open = new();

        foreach (char c in expression)
        {
            if (c is '(' or '[' or '{')
            {
                open.Push(c);
            }
            else if (c is ')' or ']' or '}')
            {
                if (open.Count == 0) return false;

                char last = open.Pop();
                bool matches = (last, c) is ('(', ')') or ('[', ']') or ('{', '}');
                if (!matches) return false;
            }
        }

        return open.Count == 0;
    }
}
