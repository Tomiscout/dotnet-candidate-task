namespace PathFinder;

public class PathFinder
{
    /// <param name="from">Departure</param>
    /// <param name="to">Destination</param>
    /// <param name="pathsCSV">Times (hours and minutes) to travel between cities e.g. "Paris,Barcelona,9:40"</param>
    public async Task<(TimeSpan, string)> FindFastestPath(string from, string to, string pathsCSV)
    {
        var pathsMap = new List<string[]>();
        var reader = new StringReader(pathsCSV);
        var line = reader.ReadLine();
        while (!string.IsNullOrEmpty(line))
        {
            var i = line.IndexOf(',');
            var j = 0;
            var data = new string[3];
            while (i != -1)
            {
                data[j] = line.Substring(0, i);
                j = j + 1;
                line = line.Substring(i + 1);
                i = line.IndexOf(',');
                if (i < 0)
                    data[j] = line;
            }
            pathsMap.Add(data);
            line = reader.ReadLine();
        }

        var paths = new List<List<string[]>>();

        foreach (var mainPath in pathsMap)
        {
            if (mainPath[0] == from &&
                mainPath[1] == to)
            {
                paths.Add(new List<string[]>{mainPath});
                continue;
            }
            else if (mainPath[0] == to &&
                     mainPath[1] == from)
            {
                paths.Add(new List<string[]>{new []{mainPath[1], mainPath[0], mainPath[2]}});
                continue;
            }

            var newPath = new List<string[]>();
            if (mainPath[0] == from)
            {
                newPath.Add(mainPath);
                paths.AddRange(FindDeeperPath(mainPath[1], to, newPath, pathsMap));
            }
            else if (mainPath[1] == from)
            {
                newPath.Add(new []{mainPath[1], mainPath[0], mainPath[2]});
                paths.AddRange(FindDeeperPath(mainPath[0], to, newPath, pathsMap));
            }
        }

        (TimeSpan, string) shortestPath = (new TimeSpan(), "");
        foreach (var foundPath in paths)
        {
            TimeSpan runningTime = new TimeSpan();
            int i = 0;
            foreach (var pathDistance in foundPath)
            {
                runningTime = runningTime + TimeSpan.Parse(pathDistance[2]);
                if (i > 0)
                    runningTime = runningTime + TimeSpan.FromMinutes(30);

                i++;
            }

            if (runningTime < shortestPath.Item1 || shortestPath.Item1.Minutes == 0)
            {
                shortestPath = (runningTime, "");
                var separator = "";
                var destination = "";
                foreach (var path in foundPath)
                {
                    shortestPath.Item2 += separator + path[0];
                    separator = ",";
                    destination = path[1];
                }
                shortestPath.Item2 += separator + destination;
            }
        }

        return shortestPath;
    }

    private List<List<string[]>> FindDeeperPath(string from, string to, List<string[]> path, List<string[]> pathsMap)
    {
        var paths = new List<List<string[]>>();
        foreach (var mapPath in pathsMap)
        {
            if (path.Any(item => item[0] == mapPath[0]))
            {
                continue;
            }
            else if (mapPath[0] == from &&
                     mapPath[1] == to)
            {
                var newPath = new List<string[]>(path);
                newPath.Add(mapPath);
                paths.Add(newPath);
                continue;
            }
            else if (mapPath[0] == to &&
                     mapPath[1] == from)
            {
                var newPath = new List<string[]>(path);
                newPath.Add(new []{mapPath[1], mapPath[0], mapPath[2]});
                paths.Add(newPath);
                continue;
            }
            else if (mapPath[0] == from)
            {
                var newPath = new List<string[]>(path);
                newPath.Add(mapPath);
                paths.AddRange(FindDeeperPath(mapPath[1], to, newPath, pathsMap));
                continue;
            }
            else if (mapPath[1] == from)
            {
                var newPath = new List<string[]>(path);
                newPath.Add(new []{mapPath[1], mapPath[0], mapPath[2]});
                paths.AddRange(FindDeeperPath(mapPath[0], to, newPath, pathsMap));
                continue;
            }
        }

        return paths;
    }
}