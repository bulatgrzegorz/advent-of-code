using Xunit;

namespace adventOfCode._2025.Day4;

public class Day4
{
    private const string ExampleInput = """
                                        ..@@.@@@@.
                                        @@@.@.@.@@
                                        @@@@@.@.@@
                                        @.@@@@..@.
                                        @@.@@@@.@@
                                        .@@@@@@@.@
                                        .@.@.@.@@@
                                        @.@@@.@@@@
                                        .@@@@@@@@.
                                        @.@.@@@.@.
                                        """;
    
    public enum Direction { Up, Left, Down, Right }
    private static readonly Direction[][] AllDirections = [
        [Direction.Up], 
        [Direction.Down], 
        [Direction.Left],
        [Direction.Right],
        [Direction.Up, Direction.Left],
        [Direction.Up, Direction.Right],
        [Direction.Down, Direction.Left],
        [Direction.Down, Direction.Right],
    ];
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
    
    private readonly record struct Position(Coordinate Coordinate, char Char);

    private readonly record struct Map(char[][] Matrix)
    {
        private readonly int _rows = Matrix.Length;
        private readonly int _cols = Matrix[0].Length;
        
        public bool Move(Position position, Direction[] directions, out Position moved)
        {
            var movedCoordinate = directions.Aggregate(position.Coordinate, (current, direction) => current.Move(direction));

            if (movedCoordinate.Row < 0 || movedCoordinate.Row > _rows - 1 || movedCoordinate.Col < 0 || movedCoordinate.Col > _cols - 1)
            {
                moved = default;
                return false;
            }
            
            moved = new Position(movedCoordinate, Matrix[movedCoordinate.Row][movedCoordinate.Col]);
            return true;
        }
    }
    
    [Fact]
    public void First()
    {
        var input = InputHelper.GetInputLines();
        
        var map = GetMap(input);
        var rows = map.Matrix.Length;
        var cols = map.Matrix[0].Length;

        var sum = 0;
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if(map.Matrix[r][c] is not '@') continue;
                
                var neightboars = 0;
                foreach (var direction in AllDirections)
                {
                    var moved = map.Move(new Position(new Coordinate(r, c), map.Matrix[r][c]), direction, out var neightboar);
                    neightboars += moved && neightboar.Char is '@' ? 1 : 0;
                }

                sum += neightboars < 4 ?  1 : 0;
            }
        }

        Assert.Equal(1384, sum);
    }
    
    [Fact]
    public void Second()
    {
        // var input = ExampleInput.Split(Environment.NewLine).Select(x => x.ReplaceLineEndings(string.Empty)).ToArray();
        var input = InputHelper.GetInputLines();
        
        var map = GetMap(input);
        var rows = map.Matrix.Length;
        var cols = map.Matrix[0].Length;

        var sum = 0;

        while (true)
        {
            var toBeRemoved = new List<Coordinate>();
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    if(map.Matrix[r][c] is not '@') continue;
                
                    var neightboars = 0;
                    foreach (var direction in AllDirections)
                    {
                        var moved = map.Move(new Position(new Coordinate(r, c), map.Matrix[r][c]), direction, out var neightboar);
                        neightboars += moved && neightboar.Char is '@' ? 1 : 0;
                    }

                    if (neightboars < 4)
                    {
                        toBeRemoved.Add(new Coordinate(r, c));
                    }
                }
            }
            
            if(toBeRemoved.Count is 0) break;
            
            sum += toBeRemoved.Count;

            foreach (var coordinate in toBeRemoved)
            {
                map.Matrix[coordinate.Row][coordinate.Col] = '.';
            }
        }

        Assert.Equal(8013, sum);
    }
    
    private static Map GetMap(string[] input)
    {
        return new Map(input
            .Select(x => x.ToCharArray())
            .ToArray());
    }
}