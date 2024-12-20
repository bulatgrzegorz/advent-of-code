using System.Collections.Frozen;
using System.Runtime.InteropServices;
using Xunit;
using Distance = int;

namespace adventOfCode._2024.Day20;

public class Day20
{
    private const string ExampleInput = """
                                        ###############
                                        #...#...#.....#
                                        #.#.#.#.#.###.#
                                        #S#...#.#.#...#
                                        #######.#.#.###
                                        #######.#.#...#
                                        #######.#.###.#
                                        ###..E#...#...#
                                        ###.#######.###
                                        #...###...#...#
                                        #.#####.#.###.#
                                        #.#...#.#.#...#
                                        #.#.#.#.#.#.###
                                        #...#...#...###
                                        ###############
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

        public override string ToString() => $"({Row},{Col})";

        public bool Move(Direction direction, int rows, int cols, out Coordinate moved)
        {
            moved = direction switch
            {
                Direction.Up => this with { Row = Row - 1 },
                Direction.Down => this with { Row = Row + 1 },
                Direction.Left => this with { Col = Col - 1 },
                Direction.Right => this with { Col = Col + 1 },
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            return !moved.IsOutOfBounds(rows, cols);
        }

        private bool IsOutOfBounds(int rows, int cols) => Row < 0 || Row > rows - 1 || Col < 0 || Col > cols - 1;
    }

    [Fact]
    public void Test()
    {
        // var input = ExampleInput.Split(Environment.NewLine).Select(x => x.ToCharArray()).ToArray(); 
        var input = InputHelper.GetInputLines().Select(x => x.ToCharArray()).ToArray();
        
        Dictionary<Coordinate, char> matrix = [];
        for (var r = 0; r < input.Length; r++)
        {
            for (var c = 0; c < input[0].Length; c++)
            {
                matrix.Add(new Coordinate(r, c), input[r][c]);
            }
        }

        Coordinate start = matrix.First(x => x.Value == 'S').Key;
        Coordinate end = matrix.First(x => x.Value == 'E').Key;
        
        var cheatsPositions = FindAllCheats(matrix, input.Length, input[0].Length);
        var walls = matrix.Where(x => x.Value is '#').Select(x => x.Key).ToFrozenSet();

        var result = 0L;
        var baseDistance = Dijkstra(walls, start, end);
        foreach (var cheatPosition in cheatsPositions.Distinct())
        {
            var wallsWithoutCheat = walls.Except([cheatPosition]).ToFrozenSet();
            var distanceWithCheat = Dijkstra(wallsWithoutCheat, start, end);
            
            if (distanceWithCheat <= baseDistance - 100)
            {
                result++;
            }
        }
        
        Assert.Equal(1321, result);
    }
    
    [Fact]
    public void Second()
    { 
        var input = InputHelper.GetInputLines().Select(x => x.ToCharArray()).ToArray();
        
        Dictionary<Coordinate, char> matrix = [];
        for (var r = 0; r < input.Length; r++)
        {
            for (var c = 0; c < input[0].Length; c++)
            {
                matrix.Add(new Coordinate(r, c), input[r][c]);
            }
        }

        Coordinate start = matrix.First(x => x.Value == 'S').Key;
        Coordinate end = matrix.First(x => x.Value == 'E').Key;
        
        var walls = matrix.Where(x => x.Value is '#').Select(x => x.Key).ToFrozenSet();

        var baseDistance = Dijkstra(walls, start, end);
        var threshold = 100;
        var save = baseDistance - threshold;
        
        var notWalls = matrix.Where(x => x.Value is not '#').Select(x => x.Key).ToHashSet();
        var ll = 0;
        
        var cache = new Dictionary<(Coordinate, Coordinate), int>();
        foreach (var notWall in notWalls)
        {
            if (!cache.TryGetValue((start, notWall), out var startToCheatBeginning))
            {
                startToCheatBeginning = Dijkstra(walls, start,  notWall);
                cache[(start, notWall)] = startToCheatBeginning;
            }
            
            foreach (var other in notWalls.Where(x => x != notWall))
            {
                var cheatDist = ManhattanDistance(other, notWall);
                
                if(cheatDist is > 20 or < 1) continue;
                if(startToCheatBeginning + cheatDist > save) continue;
                
                if (!cache.TryGetValue((end, other), out var endToCheatEnd))
                {
                    var man = ManhattanDistance(end, other);
                    if(startToCheatBeginning + cheatDist + man > save) continue;
                    endToCheatEnd = Dijkstra(walls, end,  other);
                    cache[(end, other)] = endToCheatEnd;
                }
                
                if(startToCheatBeginning + cheatDist + endToCheatEnd > save) continue;

                ll++;
            }
        }
        
        Assert.Equal(971737, ll);
    }

    private static int ManhattanDistance(Coordinate start, Coordinate end)
    {
        return Math.Abs(start.Row - end.Row) + Math.Abs(start.Col - end.Col);
    }

    private IEnumerable<Coordinate> FindAllCheats(Dictionary<Coordinate, char> matrix, int rows, int cols)
    {
        foreach (var (coordinate, value) in matrix)
        {
            if(value is not '#') continue;

            if (coordinate.Move(Direction.Right, rows, cols, out var right) && 
                matrix[right] is not '#' &&
                coordinate.Move(Direction.Left, rows, cols, out var left) &&
                matrix[left] is not '#') 
            {
                yield return coordinate;
            }
            else if (coordinate.Move(Direction.Up, rows, cols, out var up) && 
                matrix[up] is not '#' &&
                coordinate.Move(Direction.Down, rows, cols, out var down) &&
                matrix[down] is not '#') 
            {
                yield return coordinate;
            }
        }
    }
    
    private static int Dijkstra(FrozenSet<Coordinate> walls, Coordinate start, Coordinate end)
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
            var (_, currentDistance) = CollectionsMarshal.GetValueRefOrAddDefault(distances, current, out _);
            foreach (var edge in GetNeighbours(walls, current))
            {
                ref var edgeNode = ref CollectionsMarshal.GetValueRefOrAddDefault(distances, edge.Coordinate, out var edgeExisted);

                var newDistance = currentDistance + edge.distance;

                if (!edgeExisted || newDistance < edgeNode.distance)
                {
                    edgeNode = (current, newDistance);
                    
                    queue.Remove(edge.Coordinate, out _, out _);
                    queue.Enqueue(edge.Coordinate, newDistance);
                }
            }
        }

        return 0;
    }
    
    private static IEnumerable<(Coordinate Coordinate, Distance distance)> GetNeighbours(FrozenSet<Coordinate> walls, Coordinate coordinate)
    {
        foreach (var direction in AllDirections)
        {
            var movedCoordinate = coordinate.Move(direction);
            if(walls.Contains(movedCoordinate)) continue;
                
            yield return (movedCoordinate, 1);
        }
    }
}