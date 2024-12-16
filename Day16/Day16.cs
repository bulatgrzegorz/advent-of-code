using System.Runtime.InteropServices;
using Xunit;
using Distance = int;

namespace adventOfCode.Day16;

public class Day16
{
    private const string InputFile = "Day16/Day16.input";
    private const string ExampleInput = """
                                        ###############
                                        #.......#....E#
                                        #.#.###.#.###.#
                                        #.....#.#...#.#
                                        #.###.#####.#.#
                                        #.#.#.......#.#
                                        #.#.#####.###.#
                                        #...........#.#
                                        ###.#.#####.#.#
                                        #...#.....#.#.#
                                        #.#.#.###.#.#.#
                                        #.....#...#.#.#
                                        #.###.#.#.#.#.#
                                        #S..#.....#...#
                                        ###############
                                        """;
    
    public enum Direction { Up, Left, Down, Right }
    private static readonly Direction[] AllDirections = [Direction.Up, Direction.Down, Direction.Left, Direction.Right];

    private static Dictionary<Direction, Direction> _oppositeOrientation = new()
    {
        [Direction.Down] = Direction.Up,
        [Direction.Up] = Direction.Down,
        [Direction.Left] = Direction.Right,
        [Direction.Right] = Direction.Left
    };
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
        Coordinate end = matrix.First(x => x.Value == 'E').Key;;

        var queue = new PriorityQueue<(Coordinate coordinate, Direction direction), Distance>();

        var distances = new Dictionary<(Coordinate coordinate, Direction orientation), (Coordinate? previous, Direction previousOrientation, Distance distance)>
            {
                [(start, Direction.Right)] = (null, default, 0)
            };

        queue.Enqueue((start, Direction.Right), 0);

        IEnumerable<(Coordinate Coordinate, Distance distance, Direction orientation)> GetNeighbours(Coordinate coordinate, Direction orientation)
        {
            var oppositeOrientation = _oppositeOrientation[orientation];
            foreach (var direction in AllDirections)
            {
                if(direction == oppositeOrientation) continue;
                
                var movedCoordinate = coordinate.Move(direction);
                if(matrix[movedCoordinate] is '#') continue;
                
                var rotation = Math.Abs(direction - orientation) % 2;
                yield return (movedCoordinate, rotation * 1000 + 1, direction);
            }
        }

        var result = 0;
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            //build route
            if (current.coordinate == end)
            {
                // var endd = distances.First(x => x.Key.coordinate == end);
                result = distances[(current.coordinate, current.direction)].distance;
                // result = endd.Value.distance;
                var route = BuildRoute(distances, current.coordinate, current.direction);
                break;
            }
            
            var (previous, previousOrientation, currentNodeDistance) = CollectionsMarshal.GetValueRefOrAddDefault(distances, (current.coordinate, current.direction), out _);
            foreach (var edge in GetNeighbours(current.coordinate, current.direction))
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
        
        Assert.Equal(105508, result);
    }
    //
    static List<(Coordinate, Distance)> BuildRoute(Dictionary<(Coordinate coordinate, Direction orientation), (Coordinate? previous, Direction previousOrientation, Distance distance)> distances, Coordinate endNode, Direction endOrientation)
    {
        var route = new List<(Coordinate, Distance)>();
        Coordinate? prev = endNode;
        Direction previousOrientation = endOrientation;
    
        // Keep examining the previous version until we
        // get back to the start node
        while (prev is not null)
        {
            var current = prev;
            var currentOrientation = previousOrientation;
            (prev, previousOrientation, var distance) = distances[(current.Value, currentOrientation)];
            route.Add((current.Value, distance));
        }
    
        // reverse the route
        route.Reverse();
        return route;
    }
}