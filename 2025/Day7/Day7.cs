using Xunit;

namespace adventOfCode._2025.Day7;

public class Day7
{
    private const string ExampleInput = """
                                        .......S.......
                                        ...............
                                        .......^.......
                                        ...............
                                        ......^.^......
                                        ...............
                                        .....^.^.^.....
                                        ...............
                                        ....^.^...^....
                                        ...............
                                        ...^.^...^.^...
                                        ...............
                                        ..^...^.....^..
                                        ...............
                                        .^.^.^.^.^...^.
                                        ...............
                                        """;

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    };

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

    private readonly record struct Position(Coordinate Coordinate, char Type);

    private readonly record struct Map(char[][] Matrix)
    {
        private readonly int _rows = Matrix.Length;
        private readonly int _cols = Matrix[0].Length;

        public bool Move(Position position, Direction direction, HashSet<Coordinate> alreadyVisited, out Position moved)
        {
            return Move(position, direction, out moved) && alreadyVisited.Add(moved.Coordinate);
        }
        
        public bool Move(Position position, Direction direction, out Position moved)
        {
            var coordinate = position.Coordinate;
            var movedCoordinate = coordinate.Move(direction);
            if (IsOutOfBounds(movedCoordinate))
            {
                moved = default;
                return false;
            }

            moved = new Position(movedCoordinate, Matrix[movedCoordinate.Row][movedCoordinate.Col]);
            return true;
        }

        private bool IsOutOfBounds(Coordinate coordinate) => coordinate.Row < 0 || coordinate.Row > _rows - 1 ||
                                                             coordinate.Col < 0 || coordinate.Col > _cols - 1;

        public Position GetStartingPosition()
        {
            for (var r = 0; r < Matrix.Length; r++)
            {
                for (var c = 0; c < Matrix[0].Length; c++)
                {
                    if(Matrix[r][c] == 'S') return new Position(new Coordinate(r, c), 'S');
                }
            }
        
            throw new Exception("No such position");
        }
    }

    [Fact]
    public void Example()
    {
        var lines = ExampleInput.GetExampleInputLines();
        
        var map = GetMap(lines);
        var visited = new HashSet<Coordinate>();
        
        var splitCount = 0;
        Traverse(map, map.GetStartingPosition(), ref splitCount, visited);
        
        Assert.Equal(21, splitCount);
    }
    
    [Fact]
    public void First()
    {
        var lines = InputHelper.GetInputLines();
        
        var map = GetMap(lines);
        var visited = new HashSet<Coordinate>();
        
        var splitCount = 0;
        Traverse(map, map.GetStartingPosition(), ref splitCount, visited);
        
        Assert.Equal(1555, splitCount);
    }
    
    [Fact]
    public void Second()
    {
        // var lines = ExampleInput.GetExampleInputLines();
        var lines = InputHelper.GetInputLines();
        
        var map = GetMap(lines);

        var result = Traverse2(map, map.GetStartingPosition(), []);
        
        Assert.Equal(12895232295789, result);
    }

    private static void Traverse(Map map, Position position, ref int splitCount, HashSet<Coordinate>? visited = null)
    {
        visited ??= [];
        visited.Add(position.Coordinate);

        switch (position.Type)
        {
            case 'S':
            case '.':
            {
                if (!map.Move(position, Direction.Down, visited, out var newPosition)) return; 
                Traverse(map, newPosition, ref splitCount, visited);
                return;
            }
            case '^':
            {
                Interlocked.Increment(ref splitCount);
                
                if(map.Move(position, Direction.Left, visited,  out var leftSplit))
                    Traverse(map, leftSplit, ref splitCount, visited);
            
                if(map.Move(position, Direction.Right, visited,  out var rightSplit))
                    Traverse(map, rightSplit, ref splitCount, visited);
                
                return;
            }
        }
    }
    
    private static long Traverse2(Map map, Position position, Dictionary<Coordinate, long> cached)
    {
        if (cached.TryGetValue(position.Coordinate, out var cachedResult)) return cachedResult;
        
        switch (position.Type)
        {
            case 'S':
            case '.':
            {
                return !map.Move(position, Direction.Down, out var newPosition) ? 1 : Traverse2(map, newPosition, cached);
            }
            case '^':
            {
                long result = 0;
                if (map.Move(position, Direction.Left, out var leftSplit))
                { 
                    result += Traverse2(map, leftSplit, cached);
                }
                
                if (map.Move(position, Direction.Right, out var rightSplit))
                {
                    result += Traverse2(map, rightSplit, cached);
                }

                cached.TryAdd(position.Coordinate, result);
                    
                return result;
            }
        }
        
        throw new Exception("No such position");
    }

    private static Map GetMap(string[] input) => new(input.Select(x => x.ToArray()).ToArray());
}