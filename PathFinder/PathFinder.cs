
namespace PathFinder;

public class PathFinder
{
    private static readonly int _transferCityMinutes = 30;

    /// <param name="from">Departure</param>
    /// <param name="to">Destination</param>
    /// <param name="pathsCSV">Times (hours and minutes) to travel between cities e.g. "Paris,Barcelona,9:40"</param>
    public (TimeSpan, string) FindFastestPath(string from, string to, string pathsCSV)
    {
        var graph = CreateGraphFromCsv(pathsCSV);
        var allPaths = FindAllConnectedPaths(from, to, new List<string>(), graph);

        return FindShortestGraphPath(graph, allPaths);
    }

    private static Dictionary<string, Dictionary<string, int>> CreateGraphFromCsv(string csvContent)
    {
        var graph = new Dictionary<string, Dictionary<string, int>>();

        foreach (var line in csvContent.Split('\n'))
        {
            var parts = line.Split(',');
            var city1 = parts[0];
            var city2 = parts[1];
            var distance = (int)TimeSpan.Parse(parts[2]).TotalMinutes;

            if (!graph.ContainsKey(city1))
                graph[city1] = new Dictionary<string, int>();

            if (!graph.ContainsKey(city2))
                graph[city2] = new Dictionary<string, int>();

            graph[city1][city2] = distance;
            graph[city2][city1] = distance;
        }
        return graph;
    }
    
    private static List<List<string>> FindAllConnectedPaths(string from, string to, List<string> currentPath, Dictionary<string, Dictionary<string, int>> graph)
    {
        List<List<string>> allPaths = new List<List<string>>();

        // Add the current city to the current path
        currentPath.Add(from);

        if (from == to)
        {
            // We have reached the destination city, add the current path to the result
            allPaths.Add(new List<string>(currentPath));
        }
        else if (graph.ContainsKey(from))
        {
            foreach (var neighbor in graph[from])
            {
                if (!currentPath.Contains(neighbor.Key)) // Avoid cycles
                {
                    // Recursive call to explore paths from the neighbor city
                    List<List<string>> pathsFromNeighbor = FindAllConnectedPaths(neighbor.Key, to, currentPath, graph);

                    // Add all paths found from the neighbor to the result
                    allPaths.AddRange(pathsFromNeighbor);
                }
            }
        }

        // Remove the current city from the current path before backtracking
        currentPath.Remove(from);

        return allPaths;
    }

    private static (TimeSpan, string) FindShortestGraphPath(Dictionary<string, Dictionary<string, int>> graph, List<List<string>> paths)
    {
        (TimeSpan, string) shortestPath = (new TimeSpan(), "");
        foreach (var foundPath in paths)
        {
            TimeSpan runningTime = new TimeSpan();
            for(var i = 0; i < foundPath.Count - 1; i++)
            {
                var time = graph[foundPath[i]][foundPath[i + 1]];
                runningTime += TimeSpan.FromMinutes(time);
            }

            runningTime += TimeSpan.FromMinutes(_transferCityMinutes * (foundPath.Count - 2)); //time for every transfer city


            if (runningTime < shortestPath.Item1 || shortestPath.Item1.Minutes == 0)
            {
                shortestPath = (runningTime, string.Join(',', foundPath));
            }
        }
        return shortestPath;
    }
}
