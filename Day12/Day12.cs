using Xunit;

namespace adventOfCode.Day12;

public class Day12
{
    private const string InputFile = "Day12/Day12.input";

    private enum Direction { Up, Down, Left, Right };

    private static readonly Direction[] AllDirections = [Direction.Up, Direction.Down, Direction.Left, Direction.Right];
    private static readonly Direction[] UpDownDirections = [Direction.Up, Direction.Down];
    private static readonly Direction[] LeftRightDirections = [Direction.Left, Direction.Right];
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
            var currentType = position.Type;
            var coordinate = position.Coordinate;
            var movedCoordinate = coordinate.Move(direction);
            if (IsOutOfBounds(movedCoordinate))
            {
                moved = default;
                return false;
            }
            
            moved = new Position(movedCoordinate, Matrix[movedCoordinate.Row][movedCoordinate.Col]);
            return moved.Type == currentType && alreadyVisited.Add(movedCoordinate);
        }
        
        private bool IsOutOfBounds(Coordinate coordinate) => coordinate.Row < 0 || coordinate.Row > _rows - 1 || coordinate.Col < 0 || coordinate.Col > _cols - 1;

        public int CountNeighbors(char type, HashSet<Coordinate> field)
        {
            var result = 0;
            foreach (var coordinate in field)
            {
                foreach (var direction in AllDirections)
                {
                    if(HasNeighbors(type, coordinate, direction)) result++;
                }
                
            }

            return result;
        }

        public bool HasNeighbors(char type, Coordinate coordinate, Direction direction)
        {
            var neighborCoordinate = coordinate.Move(direction);
            return IsOutOfBounds(neighborCoordinate) || Matrix[neighborCoordinate.Row][neighborCoordinate.Col] != type;
        }
        
    }

    [Fact]
    public void First()
    {
        var map = GetMap(File.ReadAllLines(InputFile));
        
        var visited = new HashSet<Coordinate>();
        var typeVisited = new List<(char type, HashSet<Coordinate> traversed)>();
        while (FindNextNotVisited(map, visited, out var next))
        {
            var traversed = Traverse(map, next);
            typeVisited.Add((next.Type, traversed));
            
            visited.UnionWith(traversed);
        }

        var result = typeVisited.Select(x => map.CountNeighbors(x.type, x.traversed) * x.traversed.Count).Sum();
        
        Assert.Equal(1573474, result);
    }
    
    [Fact]
    public void Second()
    {
        var map = GetMap(File.ReadAllLines(InputFile));
        
        var visited = new HashSet<Coordinate>();
        var typeVisited = new List<(char type, HashSet<Coordinate> traversed)>();
        while (FindNextNotVisited(map, visited, out var next))
        {
            var traversed = Traverse(map, next);
            typeVisited.Add((next.Type, traversed));
            
            visited.UnionWith(traversed);
        }

        var result = 0;
        foreach (var (type, coordinates) in typeVisited)
        {
            var fences = 0;
            
            var rows = coordinates.GroupBy(x => x.Row).OrderBy(x => x.Key);
            foreach (var row in rows)
            {
                foreach (var direction in UpDownDirections)
                {
                    var colsWithNeighbors = row.Where(x => map.HasNeighbors(type, x, direction))
                        .Select(x => x.Col)
                        .Order()
                        .ToList();

                    fences += FindContinuousGroups(colsWithNeighbors);    
                }
            }
            
            var cols = coordinates.GroupBy(x => x.Col).OrderBy(x => x.Key);
            foreach (var col in cols)
            {
                foreach (var direction in LeftRightDirections)
                {
                    var rowsWithNeighbors = col.Where(x => map.HasNeighbors(type, x, direction))
                        .Select(x => x.Row)
                        .Order()
                        .ToList();

                    fences += FindContinuousGroups(rowsWithNeighbors);
                }
            }
            
            result += coordinates.Count * fences;
        }
        
        Assert.Equal(966476, result);
    }

    private static int FindContinuousGroups(List<int> numbers) =>
        numbers switch
        {
            [] => 0,
            _ => numbers.Zip(numbers.Skip(1), (a, b) => b == a + 1 ? 0 : 1).Sum() + 1
        };

    private static HashSet<Coordinate> Traverse(Map map, Position position, HashSet<Coordinate>? visited = null)
    {
        visited ??= [];
        visited.Add(position.Coordinate);

        foreach (var direction in AllDirections)
        {
            if(!map.Move(position, direction, visited, out var newPosition)) continue;
            Traverse(map, newPosition, visited);
        }

        return visited;
    }

    private static bool FindNextNotVisited(Map map, HashSet<Coordinate> visited, out Position nextPosition)
    {
        for (var r = 0; r < map.Matrix.Length; r++)
        {
            for (var c = 0; c < map.Matrix[0].Length; c++)
            {
                var coordinate = new Coordinate(r, c);
                if(visited.Contains(coordinate)) continue;
                
                nextPosition = new Position(coordinate, map.Matrix[r][c]);
                return true;
            }
        }

        nextPosition = default;
        return false;
    }
    
    private static Map GetMap(string[] input) => new(input.Select(x => x.ToArray()).ToArray());
}