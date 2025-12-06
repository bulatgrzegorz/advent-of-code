using System.Numerics;
using System.Text.RegularExpressions;
using Xunit;

namespace adventOfCode._2025.Day6;

public class Day6
{
    private const string ExampleInput = """
                                        123 328  51 64 
                                         45 64  387 23 
                                          6 98  215 314
                                        *   +   *   +  
                                        """;

    [Fact]
    public void First()
    {
        var lines = InputHelper.GetInputLines();
        // var lines = ExampleInput.Split(Environment.NewLine).Select(x => x.ReplaceLineEndings(string.Empty)).ToList();
        
        var numberArrays = lines[..^1].Select(x => Regex.Matches(x, "\\d+").Select(y => int.Parse(y.Value)).ToArray()).ToArray();
        
        var individualColumns = new long[numberArrays[0].Length][];
        for (var i = 0; i < numberArrays.Length; i++)
        {
            for (var j = 0; j < numberArrays[i].Length; j++)
            {
                individualColumns[j] ??= new long[numberArrays.Length];
                individualColumns[j][i] = numberArrays[i][j];
            }
        }

        var allSumColumns = lines[^1].Replace(" ", string.Empty).AsSpan().IndexOfAll(["+"]);
        var sum = individualColumns.Select((x, j) => allSumColumns.Contains(j) ? x.Sum() : x.Mul()).Sum();
        
        Assert.Equal(4648618073226, sum);
    }
    
    [Fact]
    public void Second()
    {
        var lines = InputHelper.GetInputLines();
        // var lines = ExampleInput.Split(Environment.NewLine);
        var characters = lines.Select(x => x.ReplaceLineEndings(string.Empty).ToCharArray()).ToArray();
        
        var columns = new List<long>[lines[^1].Count(x => x is '+' or '*')];

        var columnHeight = lines.Length - 1;
        var individualColumn = new char[columnHeight];
        var columnIndex = 0;

        var columnHeightRange = Enumerable.Range(0, columnHeight).ToArray();
        for (var i = 0; i < characters[0].Length; i++)
        {
            if (columnHeightRange.All(x => characters[x][i] is ' ')) //that's mean we are going to another column
            {
                columnIndex++;
                continue;
            }
            
            for (var j = 0; j < columnHeight; j++)
            {
                individualColumn[j] = characters[j][i];
            }

            columns[columnIndex] ??= new List<long>();
            columns[columnIndex].Add(individualColumn.ToLong());
        }
        
        var allSumColumns = lines[^1].Replace(" ", string.Empty).AsSpan().IndexOfAll(["+"]);
        var sum = columns.Select((x, j) => allSumColumns.Contains(j) ? x.Sum() : x.Mul()).Sum();

        Assert.Equal(7329921182115, sum);
    }
}

public static class Extensions
{
    extension<T>(IEnumerable<T> array) where T : INumber<T>
    {
        public T Mul()
        {
            return array.Aggregate(T.MultiplicativeIdentity, (current, t) => current * t);
        }
    }

    extension(char[] array)
    {
        public long ToLong()
        {
            long result = 0;
            
            if (array is not {Length: > 0}) throw new FormatException();

            foreach (var x in array)
            {
                if(x is ' ') continue;
                
                var c = x - '0';
                if ((uint)c > 9)
                {
                    throw new FormatException(); // not a digit
                }

                result = result * 10 + c;
            }

            return result;
        }
    }
}