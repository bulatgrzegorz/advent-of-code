using Xunit;

namespace adventOfCode;

public class Day6
{
    private const string InputFile = "Day6/Day6.input";
    private enum Direction { Up, Down, Left, Right };
    private readonly record struct Coordinate(int Row, int Col)
    {
        public Coordinate Move(Direction direction) => direction switch
        {
            Direction.Up => this with { Row = Row - 1 },
            Direction.Down => this with { Row = Row + 1 },
            Direction.Left => this with { Col = Col - 1 },
            Direction.Right => this with { Col = Col + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    };

    private readonly record struct Guard(Coordinate Coordinate, Direction Direction)
    {
        public bool IsLookingAtObstruction(char[][] map)
        {
            var position = Coordinate.Move(Direction);
            if(position.Row < 0 || position.Row > map.Length - 1 || position.Col < 0 || position.Col > map[0].Length - 1) return false;
            return map[position.Row][position.Col] is '#' or 'O';
        }

        public Guard TurnRight() => Direction switch {
            Direction.Up => this with { Direction = Direction.Right},
            Direction.Down => this with { Direction = Direction.Left},
            Direction.Left => this with { Direction = Direction.Up},
            Direction.Right => this with { Direction = Direction.Down},
            _ => throw new ArgumentOutOfRangeException()
        };
        public Guard Move() => this with { Coordinate = Coordinate.Move(Direction) };
        public bool IsOut(int rows, int cols) => Coordinate.Row < 0 || Coordinate.Row > rows - 1 || Coordinate.Col < 0 || Coordinate.Col > cols - 1;
    }

    private const char Up = '^';
    private const char Down = 'v';
    private const char Right = '>';
    private const char Left = '<';

    private static readonly Dictionary<char, Direction> Directions = new()
    {
        { Up, Direction.Up },
        { Down, Direction.Down },
        { Left, Direction.Left },
        { Right, Direction.Right }
    };
    private const string ExampleInput = """
                                        ....#.....
                                        .........#
                                        ..........
                                        ..#.......
                                        .......#..
                                        ..........
                                        .#..^.....
                                        ........#.
                                        #.........
                                        ......#...
                                        """;

    [Fact]
    public void First()
    {
        var input = File.ReadLines(InputFile).Select(x => x.ToArray()).ToArray();
        
        var visited = GetVisited(input, out _);
        
        Assert.Equal(5095, visited.CountBy(x => x.Coordinate).Count());
    }
    
    [Fact]
    public void FirstExample()
    {
        var input = ExampleInput.Split(Environment.NewLine).Select(x => x.ToArray()).ToArray();

        var visited = GetVisited(input, out _);
        
        Assert.Equal(41, visited.CountBy(x => x.Coordinate).Count());
    }
    
    [Fact]
    public void Second()
    {
        var input = File.ReadLines(InputFile).Select(x => x.ToArray()).ToArray();

        var visited = GetVisited(input, out var guard);
        
        var guardPath = visited.Skip(1);

        var count = 0;
        
        foreach (var guardTrace in guardPath.GroupBy(x => x.Coordinate))
        {
            var tempValue = input[guardTrace.Key.Row][guardTrace.Key.Col];
            input[guardTrace.Key.Row][guardTrace.Key.Col] = 'O';
            
            GetVisited(input, guard, out var isStuck);
            if (isStuck)
            {
                count++;
            }

            input[guardTrace.Key.Row][guardTrace.Key.Col] = tempValue;
        }
        
        Assert.Equal(1933, count);
    }

    private static HashSet<Guard> GetVisited(char[][] input, out Guard guard)
    {
        var rows = input.Length;
        var cols = input[0].Length;
        var result = new HashSet<Guard>();
        
        guard = FindGuard(rows, cols, input);
        result.Add(guard);
        
        return GetVisited(input, guard, out _, result);
    }
    
    private static HashSet<Guard> GetVisited(char[][] input, Guard guard, out bool isStuck, HashSet<Guard>? path = null)
    {
        path ??= [];
        isStuck = false;
        var rows = input.Length;
        var cols = input[0].Length;
        
        while (true)
        {
            while (guard.IsLookingAtObstruction(input))
            {
                guard = guard.TurnRight();
            }
            
            guard = guard.Move();
            if(guard.IsOut(rows, cols)) break;
            
            //if we already had this coordinate on same direction that mean that we are going to be stuck in loop
            if (path.Add(guard)) continue;
            
            isStuck = true;
            break;
        }

        return path;
    }

    private static Guard FindGuard(int rows, int cols, char[][] input)
    {
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (!Directions.TryGetValue(input[r][c], out var direction)) continue;
                
                return new Guard(new Coordinate(r, c), direction);
            }
        }

        throw new Exception("Could not find guard on input map");
    }
}