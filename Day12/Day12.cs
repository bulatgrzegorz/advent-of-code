using Xunit;

namespace adventOfCode.Day12;

public class Day12
{
    private const string InputFile = "Day12/Day12.input";
    private const string InputExample = """
                                        RRRRIICCFF
                                        RRRRIICCCF
                                        VVRRRCCFFF
                                        VVRCCCJFFF
                                        VVVVCJJCFE
                                        VVIVCCJJEE
                                        VVIIICJJEE
                                        MIIIIIJJEE
                                        MIIISIJEEE
                                        MMMISSJEEE
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
            if (movedCoordinate.Row < 0 || movedCoordinate.Row > _rows - 1 || movedCoordinate.Col < 0 || movedCoordinate.Col > _cols - 1)
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
            Direction[] directions = [Direction.Up, Direction.Down, Direction.Left, Direction.Right];
            foreach (var coordinate in field)
            {
                foreach (var direction in directions)
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
        // var map = GetMap(InputExample.Split(Environment.NewLine));
        
        var visited = new HashSet<Coordinate>();
        var typeVisited = new List<(char, HashSet<Coordinate>)>();
        while (FindNextNotVisited(map, visited, out var next))
        {
            var traversed = Traverse(map, next);
            typeVisited.Add((next.Type, traversed));
            
            visited = [..visited, ..traversed];
        }

        var result = 0;
        foreach (var (type, coordinates) in typeVisited)
        {
            var neighbors = map.CountNeighbors(type, coordinates);
            result += coordinates.Count * neighbors;

            Console.WriteLine($"Type: {type} coordinates: {coordinates.Count} and neirberis: {neighbors}");
        }
        
        Assert.Equal(1573474, result);
    }
    
    [Fact]
    public void Second()
    {
        var map = GetMap(File.ReadAllLines(InputFile));
        // var map = GetMap(InputExample.Split(Environment.NewLine));
        
        var visited = new HashSet<Coordinate>();
        var typeVisited = new List<(char, HashSet<Coordinate>)>();
        while (FindNextNotVisited(map, visited, out var next))
        {
            var traversed = Traverse(map, next);
            typeVisited.Add((next.Type, traversed));
            
            visited = [..visited, ..traversed];
        }

        var result = 0;
        Direction[] upDown = [Direction.Up, Direction.Down];
        Direction[] rightLeft = [Direction.Left, Direction.Right];
        foreach (var (type, coordinates) in typeVisited)
        {
            var fences = 0;
            
            var rows = coordinates.GroupBy(x => x.Row).OrderBy(x => x.Key);
            foreach (var row in rows)
            {
                foreach (var direction in upDown)
                {
                    var colsWithNeighbors = row.Where(x => map.HasNeighbors(type, x, direction))
                        .Select(x => x.Col)
                        .Order()
                        .ToList();

                    fences += FindContinuousGroups(colsWithNeighbors).Count;    
                }
            }
            
            var cols = coordinates.GroupBy(x => x.Col).OrderBy(x => x.Key);
            foreach (var col in cols)
            {
                foreach (var direction in rightLeft)
                {
                    var rowsWithNeighbors = col.Where(x => map.HasNeighbors(type, x, direction))
                        .Select(x => x.Row)
                        .Order()
                        .ToList();

                    fences += FindContinuousGroups(rowsWithNeighbors).Count;
                }
            }
            
            result += coordinates.Count * fences;
        }
        
        Assert.Equal(966476, result);
    }
    
    public static List<List<int>> FindContinuousGroups(List<int> numbers)
    {
        if (numbers.Count == 0) return [];
        
        var groups = new List<List<int>>();

        var currentGroup = new List<int> { numbers[0] };

        for (int i = 1; i < numbers.Count; i++)
        {
            if (numbers[i] == numbers[i - 1] + 1)
            {
                // Continue the group
                currentGroup.Add(numbers[i]);
            }
            else
            {
                // End the current group and start a new one
                groups.Add(currentGroup);
                currentGroup = new List<int> { numbers[i] };
            }
        }

        // Add the last group
        groups.Add(currentGroup);

        return groups;
    }

    private static void Traverse(Map map, Position position, Direction direction, HashSet<Coordinate> alreadyVisited)
    {
        if(!map.Move(position, direction, alreadyVisited, out var newPosition)) return;

        Traverse(map, newPosition, alreadyVisited);
    }

    private static HashSet<Coordinate> Traverse(Map map, Position position, HashSet<Coordinate>? visited = null)
    {
        visited ??= new HashSet<Coordinate>();
        visited.Add(position.Coordinate);

        Traverse(map, position, Direction.Up, visited);
        Traverse(map, position, Direction.Down, visited);
        Traverse(map, position, Direction.Right, visited);
        Traverse(map, position, Direction.Left, visited);

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
    
    private static Map GetMap(string[] input)
    {
        return new Map(input
            .Select(x => x.ToArray())
            .ToArray());
    }
}