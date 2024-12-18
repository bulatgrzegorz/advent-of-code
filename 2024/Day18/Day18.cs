using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Xunit;
using Distance = int;

namespace adventOfCode._2024.Day18;

public class Day18
{
    public enum Direction { Up, Left, Down, Right }
    private static readonly Direction[] AllDirections = [Direction.Up, Direction.Down, Direction.Left, Direction.Right];
    public readonly record struct Coordinate(int Row, int Col)
    {
        public Coordinate Move(Direction direction) => direction switch
        {
            Direction.Up => this with { Row = Row - 1 },
            Direction.Down => this with { Row = Row + 1 },
            Direction.Left => this with { Col = Col - 1 },
            Direction.Right => this with { Col = Col + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    
    [Fact]
    public void Test()
    {
        var rows = 71;
        var cols = 71;

        var input = InputHelper.GetInput();
        var walls = new HashSet<Coordinate>();
        foreach (var match in Regex.Matches(input, @"(\d+),(\d+)").Take(1024))
        {
            var row = int.Parse(match.Groups[1].Value);
            var col = int.Parse(match.Groups[2].Value);
            
            walls.Add(new(row, col));
        }


        var start = new Coordinate(0, 0);
        var end = new Coordinate(rows - 1, cols - 1);

        var result = Dijkstra(walls, start, end);

        Assert.Equal(382, result);
    }
    
    [Fact]
    public void Second()
    {
        var rows = 71;
        var cols = 71;

        var start = new Coordinate(0, 0);
        var end = new Coordinate(rows - 1, cols - 1);
        var input = InputHelper.GetInput();
        var walls = new List<Coordinate>();
        foreach (Match match in Regex.Matches(input, @"(\d+),(\d+)"))
        {
            var row = int.Parse(match.Groups[1].Value);
            var col = int.Parse(match.Groups[2].Value);
            
            walls.Add(new(row, col));
        }
        
        var (lo, hi) = (0, walls.Count);
        while (hi - lo > 1) {
            var m = (lo + hi) / 2;
            if (!BreadthFirstSearch(start, walls.Take(m).ToHashSet(), end)) {
                hi = m;
            } else {
                lo = m;
            }
        }
        
        Assert.Equal(new Coordinate(6, 36), walls[lo]);
    }

    private bool BreadthFirstSearch(Coordinate start, HashSet<Coordinate> walls, Coordinate end)
    {
        var visited = new HashSet<Coordinate>();
        var queue = new Queue<Coordinate>();

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            if (!visited.Add(node)) continue;
            
            foreach (var neighbor in GetNeighbours(walls, node))
            {
                if (neighbor.Coordinate == end) return true;
                queue.Enqueue(neighbor.Coordinate);
            }
        }

        return false;
    }

    private static int Dijkstra(HashSet<Coordinate> walls, Coordinate start, Coordinate end)
    {
        var queue = new PriorityQueue<Coordinate, Distance>();

        var distances = new Dictionary<Coordinate, (Coordinate? previous, Distance distance)>
        {
            [start] = (null, 0)
        };

        queue.Enqueue(start, 0);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            if (current == end) return distances[current].distance;
            
            var (_, currentNodeDistance) = CollectionsMarshal.GetValueRefOrAddDefault(distances, current, out _);
            foreach (var edge in GetNeighbours(walls, current))
            {
                var edgeNode = CollectionsMarshal.GetValueRefOrAddDefault(distances, edge.Coordinate, out var edgeExisted);

                var newDistance = currentNodeDistance + edge.distance;

                if (!edgeExisted || newDistance < edgeNode.distance)
                {
                    distances[edge.Coordinate] = (current, newDistance);

                    queue.Remove(edge.Coordinate, out _, out _);
                    queue.Enqueue(edge.Coordinate, newDistance);
                }
            }
        }

        return 0;
    }
    
    private static IEnumerable<(Coordinate Coordinate, Distance distance)> GetNeighbours(HashSet<Coordinate> walls, Coordinate coordinate)
    {
        foreach (var direction in AllDirections)
        {
            var movedCoordinate = coordinate.Move(direction);
            if(movedCoordinate is {Col: < 0 or > 70} or {Row: < 0 or > 70}) continue;
            if(walls.Contains(movedCoordinate)) continue;
                
            yield return (movedCoordinate, 1);
        }
    }
}