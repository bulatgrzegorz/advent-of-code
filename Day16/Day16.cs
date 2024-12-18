using System.Runtime.InteropServices;
using Xunit;
using Distance = int;

namespace adventOfCode.Day16;

public class Day16
{
    private const string InputFile = "Day16/Day16.input";
    private const string ExampleInput = """
                                        #################
                                        #...#...#...#..E#
                                        #.#.#.#.#.#.#.#.#
                                        #.#.#.#...#...#.#
                                        #.#.#.#.###.#.#.#
                                        #...#.#.#.....#.#
                                        #.#.#.#.#.#####.#
                                        #.#...#.#.#.....#
                                        #.#.#####.#.###.#
                                        #.#.#.......#...#
                                        #.#.###.#####.###
                                        #.#.#...#.....#.#
                                        #.#.#.#####.###.#
                                        #.#.#.........#.#
                                        #.#.#.#########.#
                                        #S#.............#
                                        #################
                                        """;
    
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
        Dictionary<Coordinate, char> matrix = [];
        
        
        var input = File.ReadAllLines(InputFile).Select(x => x.ToCharArray()).ToArray();
        // var input = ExampleInput.Split(Environment.NewLine).Select(x => x.ToCharArray()).ToArray();
        for (var r = 0; r < input.Length; r++)
        {
            for (var c = 0; c < input[0].Length; c++)
            {
                matrix.Add(new Coordinate(r, c), input[r][c]);
            }
        }
        
        Coordinate start = matrix.First(x => x.Value == 'S').Key;
        Coordinate end = matrix.First(x => x.Value == 'E').Key;

        var result = Dijkstra(matrix, start, Direction.Right, end);

        Assert.Equal(105508, result);
    }
    
    [Fact]
    public void Second()
    {
        Dictionary<Coordinate, char> matrix = [];
        
        var input = File.ReadAllLines(InputFile).Select(x => x.ToCharArray()).ToArray();
        // var input = ExampleInput.Split(Environment.NewLine).Select(x => x.ToCharArray()).ToArray();
        for (var r = 0; r < input.Length; r++)
        {
            for (var c = 0; c < input[0].Length; c++)
            {
                matrix.Add(new Coordinate(r, c), input[r][c]);
            }
        }
        
        Coordinate start = matrix.First(x => x.Value == 'S').Key;
        Coordinate end = matrix.First(x => x.Value == 'E').Key;

        var shortestPath = Dijkstra(matrix, start, Direction.Right, end);
        
        var results = new List<HashSet<Coordinate>>();
        Traverse(matrix, start, Direction.Right, end, shortestPath, 0, [], results);
        
        Assert.Equal(548, results.SelectMany(x => x).Distinct().Count());
    }

    private static void Traverse(Dictionary<Coordinate, char> matrix, Coordinate pos, Direction or, Coordinate end,
        Distance pathDistance, Distance totalDistance, HashSet<Coordinate> path, List<HashSet<Coordinate>> results)
    {
        if (!path.Add(pos)) return;
        if (totalDistance > pathDistance) return;

        if(pos == end && totalDistance == pathDistance)
        {
            results.Add(path);
            return;
        }
        
        if(Dijkstra(matrix, pos, or, end) + totalDistance != pathDistance) return;

        foreach (var (moved, distance, orientation) in GetNeighbours(matrix, pos, or))
        {
            Traverse(matrix, moved, orientation, end, pathDistance, totalDistance + distance, [..path], results);
        }
    }

    private static int Dijkstra(Dictionary<Coordinate, char> matrix, Coordinate start, Direction startOrientation, Coordinate end)
    {
        var queue = new PriorityQueue<(Coordinate coordinate, Direction direction), Distance>();

        var distances = new Dictionary<(Coordinate coordinate, Direction orientation), (Coordinate? previous, Direction previousOrientation, Distance distance)>
        {
            [(start, startOrientation)] = (null, default, 0)
        };

        queue.Enqueue((start, startOrientation), 0);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            if (current.coordinate == end) return distances[(current.coordinate, current.direction)].distance;
            
            var (_, _, currentNodeDistance) = CollectionsMarshal.GetValueRefOrAddDefault(distances, (current.coordinate, current.direction), out _);
            foreach (var edge in GetNeighbours(matrix, current.coordinate, current.direction))
            {
                var edgeNode = CollectionsMarshal.GetValueRefOrAddDefault(distances, (edge.Coordinate, edge.orientation), out var edgeExisted);

                var newDistance = currentNodeDistance + edge.distance;

                if (!edgeExisted || newDistance < edgeNode.distance)
                {
                    distances[(edge.Coordinate, edge.orientation)] = (current.coordinate, current.direction, newDistance);

                    queue.Remove((edge.Coordinate, edge.orientation), out _, out _);
                    queue.Enqueue((edge.Coordinate, edge.orientation), newDistance);
                }
            }
        }

        return 0;
    }
    
    private static IEnumerable<(Coordinate Coordinate, Distance distance, Direction orientation)> GetNeighbours(Dictionary<Coordinate, char> matrix, Coordinate coordinate, Direction orientation)
    {
        foreach (var direction in AllDirections)
        {
            var movedCoordinate = coordinate.Move(direction);
            if(matrix[movedCoordinate] is '#') continue;
                
            var rotation = Math.Abs(direction - orientation) % 2;
            yield return (movedCoordinate, rotation * 1000 + 1, direction);
        }
    }
}