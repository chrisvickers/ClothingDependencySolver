using System;
using System.Collections.Generic;
using System.Linq;

public class ClothingOrder
{
    private string[,] dependencies;

    public ClothingOrder(string[,] dependencies)
    {
        this.dependencies = dependencies;
    }

    public void PrintOrder()
    {
        var graph = BuildGraph(dependencies);
        var sortedOrder = TopologicalSort(graph);
        if (sortedOrder == null)
        {
            Console.WriteLine("There is a cycle in the dependencies.");
        }
        else
        {
            PrintSortedOrder(sortedOrder);
        }
    }

    private Dictionary<string, List<string>> BuildGraph(string[,] dependencies)
    {
        var graph = new Dictionary<string, List<string>>();
        var inDegrees = new Dictionary<string, int>();

        int numDependencies = dependencies.GetLength(0);
        for (int i = 0; i < numDependencies; i++)
        {
            string src = dependencies[i, 0];
            string dst = dependencies[i, 1];

            if (!graph.ContainsKey(src))
            {
                graph[src] = new List<string>();
            }
            if (!graph.ContainsKey(dst))
            {
                graph[dst] = new List<string>();
            }
            if (!inDegrees.ContainsKey(dst))
            {
                inDegrees[dst] = 0;
            }

            graph[src].Add(dst);
            inDegrees[dst]++;
        }

        return graph;
    }

    private List<List<string>> TopologicalSort(Dictionary<string, List<string>> graph)
    {
        var inDegree = new Dictionary<string, int>();
        foreach (var item in graph)
        {
            if (!inDegree.ContainsKey(item.Key))
            {
                inDegree[item.Key] = 0;
            }
            foreach (var neighbor in item.Value)
            {
                if (!inDegree.ContainsKey(neighbor))
                {
                    inDegree[neighbor] = 0;
                }
                inDegree[neighbor]++;
            }
        }

        var zeroInDegreeQueue = new Queue<string>(inDegree.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));
        var result = new List<List<string>>();
        int countVisited = 0;

        while (zeroInDegreeQueue.Count > 0)
        {
            var currentBatch = zeroInDegreeQueue.OrderBy(x => x).ToList();
            result.Add(currentBatch);
            zeroInDegreeQueue.Clear();

            foreach (var node in currentBatch)
            {
                foreach (var neighbor in graph[node])
                {
                    inDegree[neighbor]--;
                    if (inDegree[neighbor] == 0)
                    {
                        zeroInDegreeQueue.Enqueue(neighbor);
                    }
                }
                countVisited++;
            }
        }

        if (countVisited != inDegree.Count) // If all nodes were not visited, there's a cycle
        {
            return null;
        }

        return result;
    }

    private void PrintSortedOrder(List<List<string>> sortedOrder)
    {
        foreach (var batch in sortedOrder)
        {
            Console.WriteLine(string.Join(", ", batch));
        }
    }
}

class Program
{
    static void Main()
    {
        var input = new string[,]
        {
            {"t-shirt", "dress shirt"},
            {"dress shirt", "pants"},
            {"dress shirt", "suit jacket"},
            {"tie", "suit jacket"},
            {"pants", "suit jacket"},
            {"belt", "suit jacket"},
            {"suit jacket", "overcoat"},
            {"dress shirt", "tie"},
            {"suit jacket", "sun glasses"},
            {"sun glasses", "overcoat"},
            {"left sock", "pants"},
            {"pants", "belt"},
            {"suit jacket", "left shoe"},
            {"suit jacket", "right shoe"},
            {"right sock", "pants"}
        };

        var clothingOrder = new ClothingOrder(input);
        clothingOrder.PrintOrder();
    }
}
