using Xunit;

namespace adventOfCode._2024.Day10;

public class Day10
{
    private const string InputExample = """
                                        89010123
                                        78121874
                                        87430965
                                        96549874
                                        45678903
                                        32019012
                                        01329801
                                        10456732
                                        """;
    
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
    
    private readonly record struct Position(Coordinate Coordinate, int Height);

    private readonly record struct Map(int[][] Matrix)
    {
        private readonly int _rows = Matrix.Length;
        private readonly int _cols = Matrix[0].Length;
        
        public bool Move(Position position, Direction direction, out Position moved)
        {
            var currentHeight = position.Height;
            var coordinate = position.Coordinate;
            var movedCoordinate = coordinate.Move(direction);
            if (movedCoordinate.Row < 0 || movedCoordinate.Row > _rows - 1 || movedCoordinate.Col < 0 || movedCoordinate.Col > _cols - 1)
            {
                moved = default;
                return false;
            }
            
            moved = new Position(movedCoordinate, Matrix[movedCoordinate.Row][movedCoordinate.Col]);
            return moved.Height - 1 == currentHeight;
        }
    }

    [Fact]
    public void Example()
    {
        var map = GetMap(InputExample.Split(Environment.NewLine));

        var trailheads = GetTrailheads(map);
        
        var result = trailheads.Select(x => Traverse(map, x).Distinct().Count()).Sum();
        
        Assert.Equal(36, result);
    }
    
    [Fact]
    public void First()
    {
        var map = GetMap(InputHelper.GetInputLines());

        var trailheads = GetTrailheads(map);
        
        var result = trailheads.Select(x => Traverse(map, x).Distinct().Count()).Sum();

        Assert.Equal(717, result);
    }
    
    [Fact]
    public void Second()
    {
        var map = GetMap(InputHelper.GetInputLines());

        var trailheads = GetTrailheads(map);
        
        var result = trailheads.Select(x => Traverse(map, x).Count()).Sum();
        
        Assert.Equal(1686, result);
    }

    private static IEnumerable<Coordinate> Traverse(Map map, Position position)
    {
        IEnumerable<Coordinate> traversed = [
            ..Traverse(map, position, Direction.Up),
            ..Traverse(map, position, Direction.Down),
            ..Traverse(map, position, Direction.Right),
            ..Traverse(map, position, Direction.Left),
        ];

        foreach (var tra in traversed)
        {
            yield return tra;
        }
    }
    
    private static IEnumerable<Coordinate> Traverse(Map map, Position position, Direction direction)
    {
        if(!map.Move(position, direction, out var newPosition)) yield break;
        if(newPosition.Height == 9) {yield return newPosition.Coordinate; yield break;}

        foreach (var coordinate in Traverse(map, newPosition))
        {
            yield return coordinate;
        }
    }

    private static IEnumerable<Position> GetTrailheads(Map map)
    {
        var rows = map.Matrix.Length;
        var cols = map.Matrix[0].Length;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (map.Matrix[r][c] is 0) yield return new Position(new Coordinate(r, c), 0);
            }
        }
    }

    private static Map GetMap(string[] input)
    {
        return new Map(input
            .Select(x => x.Select(c => c is '.' ? -1 : c - '0').ToArray())
            .ToArray());
    }
}