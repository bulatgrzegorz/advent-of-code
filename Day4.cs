using System.Buffers;
using System.Collections.Immutable;
using Xunit;
using Input = System.Collections.Immutable.ImmutableArray<System.Collections.Immutable.ImmutableArray<char>>;

namespace adventOfCode2024;

public class Day4
{
    private const string ExampleInput = """
                                        MMMSXXMASM
                                        MSAMXMSMSA
                                        AMXSXMAAMM
                                        MSAMASMSMX
                                        XMASAMXAMM
                                        XXAMMXXAMA
                                        SMSMSASXSS
                                        SAXAMASAAA
                                        MAMMMXMMMM
                                        MXMXAXMASX
                                        """;

    private const string ExampleXmax = """
                                       .M.S......
                                       ..A..MSMS.
                                       .M.S.MAA..
                                       ..A.ASMSM.
                                       .M.S.M....
                                       ..........
                                       S.S.S.S.S.
                                       .A.A.A.A..
                                       M.M.M.M.M.
                                       ..........
                                       """;
    private const string InputFile = "Day4.txt";
    private const string Xmas = "XMAS";
    private const string XmasReversed = "SAMX";
    private const string Mas = "MAS";
    private const string MasReversed = "SAM";

    [Fact]
    public void Example()
    {
        var s = ExampleInput.Split(Environment.NewLine).Select(x => x.ToImmutableArray()).ToImmutableArray();

        var sum = 0;

        sum += Count(GetVerticalLines(s));
        sum += Count(GetHorizontalLines(s));
        sum += Count(GetDiagonalLines(s));
        
        Assert.Equal(18, sum);
    }
    
    [Fact]
    public void First()
    {
        var s = File.ReadLines(InputFile).Select(x => x.ToImmutableArray()).ToImmutableArray();

        var sum = 0;

        sum += Count(GetVerticalLines(s));
        sum += Count(GetHorizontalLines(s));
        sum += Count(GetDiagonalLines(s));
        
        Assert.Equal(2500, sum);
    }
    
    [Fact]
    public void SecondExample()
    {
        var s = ExampleXmax.Split(Environment.NewLine).Select(x => x.ToImmutableArray()).ToImmutableArray();
        var searchValues = SearchValues.Create(new[] { Mas, MasReversed }, StringComparison.Ordinal);
        var sum = 0;

        var (leftToRight, rightToLeft) = GetDiagonalLinesBothWays(s);
        var leftToRightCoordinates = new List<(int, int)>();
        foreach (var diag in leftToRight)
        {
            //coordinates of middle of found words in diagonal
            var coordinates = FindIndexOfAll(diag, searchValues, 2);
            leftToRightCoordinates.AddRange(coordinates);
        }
        
        var rightToLeftCoordinates = new List<(int, int)>();
        foreach (var diag in rightToLeft)
        {
            //coordinates of middle of found words in diagonal
            var coordinates = FindIndexOfAll(diag, searchValues, 2);
            rightToLeftCoordinates.AddRange(coordinates);
        }

        var count = leftToRightCoordinates.Count(x => rightToLeftCoordinates.Any(y => y.Item1 == x.Item1 && y.Item2 == x.Item2));

        Assert.Equal(9, count);
    }
    
    [Fact]
    public void Second()
    {
        var s = File.ReadLines(InputFile).Select(x => x.ToImmutableArray()).ToImmutableArray();
        var searchValues = SearchValues.Create(new[] { Mas, MasReversed }, StringComparison.Ordinal);

        var (leftToRight, rightToLeft) = GetDiagonalLinesBothWays(s);
        var leftToRightCoordinates = new List<(int, int)>();
        foreach (var diag in leftToRight)
        {
            //coordinates of middle of found words in diagonal
            var coordinates = FindIndexOfAll(diag, searchValues, 2);
            leftToRightCoordinates.AddRange(coordinates);
        }
        
        var rightToLeftCoordinates = new List<(int, int)>();
        foreach (var diag in rightToLeft)
        {
            //coordinates of middle of found words in diagonal
            var coordinates = FindIndexOfAll(diag, searchValues, 2);
            rightToLeftCoordinates.AddRange(coordinates);
        }

        var count = leftToRightCoordinates.Count(x => rightToLeftCoordinates.Any(y => y.Item1 == x.Item1 && y.Item2 == x.Item2));

        Assert.Equal(9, count);
    }

    [Fact]
    public void DiagonalTest()
    {
        var matrix = """
                     a    b    c    d    e  
                     g    1    i    1    k 
                     m    n    2    p    r 
                     t    3    w    3    x
                     f    j    l    s    z
                     """;
        
        var matrix1 = matrix.Split(Environment.NewLine).Select(x => x.Split(" ").Where(y => !string.IsNullOrWhiteSpace(y)).Select(x => char.Parse(x)).ToImmutableArray()).ToImmutableArray();
        var searchValues = SearchValues.Create(new[] { "123", "321" }, StringComparison.Ordinal);
        var s = GetDiagonalLinesBothWays(matrix1);
        foreach (var d in s.leftToRight)
        {
            var a = FindIndexOfAll(d, searchValues, 3).ToArray();
        }
        
        foreach (var d in s.rightToLeft)
        {
            var a = FindIndexOfAll(d, searchValues, 3).ToArray();
        }
    }

    [Fact]
    public void TestSpanSearch()
    {
        var testValue = "MASAM";
        var searchValues = SearchValues.Create(new[] { Mas, MasReversed }, StringComparison.Ordinal);
        var result = testValue.AsSpan().IndexOfAll(searchValues, 2);
        
        Assert.Equal(2, result.Count);
        Assert.Equal(0, result[0]);
        Assert.Equal(2, result[1]);
    }
    
    [Fact]
    public void DiagonalTest2()
    {
        var matrix = """
                     1    b    3    d    e  
                     g    2    i    j    k 
                     1    n    3    p    r 
                     t    u    w    3    x
                     f    j    l    s    z
                     """;
        
        var matrix1 = matrix.Split(Environment.NewLine).Select(x => x.Split(" ").Where(y => !string.IsNullOrWhiteSpace(y)).Select(x => char.Parse(x)).ToImmutableArray()).ToImmutableArray();
        var searchValues = SearchValues.Create(new[] { "123", "321" }, StringComparison.Ordinal);
        var s = GetDiagonalLinesBothWays(matrix1);
        foreach (var d in s.leftToRight)
        {
            var a = FindIndexOfAll(d, searchValues, 3).ToArray();
            if (a.Length > 0)
            {
                Console.WriteLine();
            }
        }
        
        foreach (var d in s.rightToLeft)
        {
            var a = FindIndexOfAll(d, searchValues, 3).ToArray();
            if (a.Length > 0)
            {
                Console.WriteLine();
            }
        }
    }

    private IEnumerable<(int, int)> FindIndexOfAll(Diag diag, SearchValues<string> searchValues, int lengthOfValues)
    {
        ReadOnlySpan<char> diagonalSpan = diag.Characters.AsSpan();
        var indexes = diagonalSpan.IndexOfAll(searchValues, lengthOfValues);
        foreach (var index in indexes)
        {
            //we are taking one after found index, as we need middle of word (MAS, SAM)
            yield return diag.Coordinates[index + 1];
        }
    }

    private int Count(IEnumerable<ImmutableArray<char>> lines)
    {
        var sum = 0;
        foreach (var line in lines)
        {
            var lineSpan = line.AsSpan();
            sum += lineSpan.Count(Xmas);
            sum += lineSpan.Count(XmasReversed);
        }

        return sum;
    }

    private static IEnumerable<ImmutableArray<char>> GetHorizontalLines(Input input)
    {
        return input;
    }
    
    private static IEnumerable<ImmutableArray<char>> GetVerticalLines(Input input)
    {
        var buffer = new char[input.Length];
        for (var j = 0; j < input[0].Length; j++)
        {
            for (var i = 0; i < input.Length; i++)
            {
                buffer[i] = input[i][j];
            }
            
            yield return [..buffer];
        }
    }

    private static (IEnumerable<Diag> leftToRight, IEnumerable<Diag> rightToLeft) GetDiagonalLinesBothWays(Input matrix)
    {
        int rows = matrix.Length;
        int cols = matrix[0].Length;
        
        var leftToRight = new List<Diag>();
        // Diagonals from bottom-left to top-right (i - j is constant)
        // Iterate diagonals starting from the bottom-most row
        for (int k = 0; k < rows + cols - 1; k++)
        {
            var character = new List<char>();
            var coordinates = new List<(int, int)>();
            
            // This is for diagonals with i - j = k
            for (int i = Math.Min(k, rows - 1); i >= 0; i--)
            {
                int j = k - i;
                if (j >= 0 && j < cols)
                {
                    character.Add(matrix[i][j]);
                    coordinates.Add((i, j));
                }
            }

            leftToRight.Add(new Diag(){Characters = character.ToArray(), Coordinates = coordinates.ToArray()});
        }
        
        var rightToLeft = new List<Diag>();
        // Diagonals from top-right to bottom-left (i + j is constant)
        // Iterate diagonals starting from the right-most column
        for (int k = 0; k < rows + cols - 1; k++)
        {
            var character = new List<char>();
            var coordinates = new List<(int, int)>();
            
            // This is for diagonals with i + j = k
            for (int i = Math.Min(k, rows - 1); i >= 0; i--)
            {
                int j = k - i;
                if (j >= 0 && j < cols)
                {
                    
                    character.Add(matrix[i][cols - 1 - j]);
                    coordinates.Add((i, cols - 1 - j));
                }
            }
            
            rightToLeft.Add(new Diag(){Characters = character.ToArray(), Coordinates = coordinates.ToArray()});
        }
        
        return (leftToRight, rightToLeft);
    }
    
    private struct Diag
    {
        public char[] Characters;
        public (int, int)[] Coordinates;
    }
    
    private struct Cell
    {
        public int Row;
        public int Column;
        public char Char;
    }
    
    private static IEnumerable<ImmutableArray<T>> GetDiagonalLines<T>(ImmutableArray<ImmutableArray<T>> matrix)
    {
        int rows = matrix.Length;
        int cols = matrix[0].Length;
        
        // Diagonals from bottom-left to top-right (i - j is constant)
        // Iterate diagonals starting from the bottom-most row
        for (int k = 0; k < rows + cols - 1; k++)
        {
            var diagonal = new List<T>();
            
            // This is for diagonals with i - j = k
            for (int i = Math.Min(k, rows - 1); i >= 0; i--)
            {
                int j = k - i;
                if (j >= 0 && j < cols)
                {
                    diagonal.Add(matrix[i][j]);
                }
            }
            
            if (diagonal.Count > 0)
            {
                yield return [..diagonal];
            }
        }

        // Diagonals from top-right to bottom-left (i + j is constant)
        // Iterate diagonals starting from the right-most column
        for (int k = 1; k < rows + cols - 1; k++)
        {
            var diagonal = new List<T>();
            
            // This is for diagonals with i + j = k
            for (int i = Math.Min(k, rows - 1); i >= 0; i--)
            {
                int j = k - i;
                if (j >= 0 && j < cols)
                {
                    diagonal.Add(matrix[i][cols - 1 - j]);
                }
            }
            
            if (diagonal.Count > 0)
            {
                yield return [..diagonal];
            }
        }
    }
}