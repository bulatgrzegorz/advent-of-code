using System.Collections.Immutable;
using Xunit;

namespace adventOfCode;

public class Day8
{
    private const string ExampleInput = """
                                        ............
                                        ........0...
                                        .....0......
                                        .......0....
                                        ....0.......
                                        ......A.....
                                        ............
                                        ............
                                        ........A...
                                        .........A..
                                        ............
                                        ............
                                        """;
    private const string InputFile = "Day8/Day8.input";
    
    [Fact]
    public void Example()
    {
        var positions = GetAllPositions(ExampleInput.Split(Environment.NewLine));
        
        var antinodes = GetAntinodesFirst(positions.rows, positions.cols, positions.symbols);
    
        Assert.Equal(14, antinodes.Count);
    }
    
    [Fact]
    public void First()
    {
        var positions = GetAllPositions(File.ReadAllLines(InputFile));
        
        var antinodes = GetAntinodesFirst(positions.rows, positions.cols, positions.symbols);

        Assert.Equal(341, antinodes.Count);
    }

    [Fact]
    public void Second()
    {
        var positions = GetAllPositions(File.ReadAllLines(InputFile));

        var antinodes = GetAntinodesSecond(positions.rows, positions.cols, positions.symbols);
    
        Assert.Equal(1134, antinodes.Count);
    }
    
    private static HashSet<Point> GetAntinodesFirst(int rows, int cols, ILookup<char, Point> symbols)
    {
        var antinodes = new HashSet<Point>();
        foreach (var symbol in symbols)
        {
            var positionPairs = symbol.SelectMany((x, i) => symbol.Skip(i + 1), (x, y) => (x, y));
            foreach (var (x, y) in positionPairs)
            {
                var x1 = 2*x - y;
                if (x1.IsInBounds(rows, cols)) antinodes.Add(x1);
                
                var x2 = 2*y - x;
                if (x2.IsInBounds(rows, cols)) antinodes.Add(x2);
            }
        }

        return antinodes;
    }
    
    private static HashSet<Point> GetAntinodesSecond(int rows, int cols, ILookup<char, Point> symbols)
    {
        var antinodes = new HashSet<Point>();
        foreach (var symbol in symbols)
        {
            var positionPairs = symbol.SelectMany((x, i) => symbol.Skip(i + 1), (x, y) => (x, y));
            foreach (var (x, y) in positionPairs)
            {
                antinodes.Add(x);
                antinodes.Add(y);
                var k = 1;
                while (true)
                {
                    var r = (k + 1) * x - k*y;
                    if(r.IsInBounds(rows, cols)) antinodes.Add(r);
                    else break;

                    k++;
                }
                
                k = 1;
                while (true)
                {
                    var r = (k + 1) * y - k*x;
                    if(r.IsInBounds(rows, cols)) antinodes.Add(r);
                    else break;

                    k++;
                }
            }
        }

        return antinodes;
    }

    private readonly record struct Point(int Row, int Col)
    {
        public static Point operator -(Point a, Point b) => new(a.Row - b.Row, a.Col - b.Col);
        public static Point operator +(Point a, Point b) => new(a.Row + b.Row, a.Col + b.Col);
        public static Point operator *(int b, Point a) => new(a.Row *  b, a.Col * b);
        public bool IsInBounds(int rows, int cols) => Row >= 0 && Row <= rows - 1 && Col >= 0 && Col <= cols - 1;
    }

    private (int rows, int cols, ILookup<char, Point> symbols) GetAllPositions(string[] input)
    {
        var map = input.Select(x => x.ToImmutableArray()).ToImmutableArray();

        var symbols = new List<(char symbol, Point position)>();
        var rows = map.Length;
        var cols = map[0].Length;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var symbol = map[r][c];
                if(symbol == '.') continue;
                symbols.Add((symbol, new Point(r, c)));
            }
        }
        
        return (rows, cols, symbols.ToLookup(x => x.symbol, x => x.position));
    }
}