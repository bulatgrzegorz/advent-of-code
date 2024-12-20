using Xunit;

namespace adventOfCode._2024.Day20;

public class Day20
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
    public void First()
    { 
        var input = InputHelper.GetInputLines().Select(x => x.ToCharArray()).ToArray();
        
        var cheats = Calculate(input, 2);

        Assert.Equal(1321, cheats);
    }
    
    [Fact]
    public void Second()
    { 
        var input = InputHelper.GetInputLines().Select(x => x.ToCharArray()).ToArray();
        
        var cheats = Calculate(input, 20);

        Assert.Equal(971737, cheats);
    }

    private static int Calculate(char[][] input, int maxCheatDistance)
    {
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
        var path = GetPath(start, end, matrix);
        var pathIndexes = Enumerable.Range(0, path.Length).ToArray();
        
        int Cheats(int i)
        {
            var result = 0;
            foreach (var j in pathIndexes[..i])
            {
                var cheatDistance = ManhattanDistance(path[i], path[j]);
                // end---------i<---cheat--->j-------start
                //   ^----x----^             ^
                //   ^----------y------------|
                // saving is difference between normal distance from j to i and cheat distance
                // to get distance from j to i we have to:
                // calculate difference between x and y (which is (path.Length - j) - (path.Length - i) = i - j
                var saving = (i - j) - cheatDistance;
                if (cheatDistance <= maxCheatDistance && saving >= 100) result++;
            }
            
            return result;
        }

        var cheats = pathIndexes.AsParallel().Select(Cheats).Sum();
        return cheats;
    }

    private static int ManhattanDistance(Coordinate start, Coordinate end)
    {
        return Math.Abs(start.Row - end.Row) + Math.Abs(start.Col - end.Col);
    }

    private static Coordinate[] GetPath(Coordinate start, Coordinate end, Dictionary<Coordinate, char> matrix)
    {
        List<Coordinate> path = [start];
        var (previous, current) = (default(Coordinate), start);
        while (current != end)
        {
            foreach (var direction in AllDirections)
            {
                var moved = current.Move(direction);
                if(matrix[moved] is '#') continue;
                if(moved == previous) continue;
                
                (previous, current) = (current, moved);
                path.Add(current);
                break;
            }
        }

        return path.ToArray();
    }
}