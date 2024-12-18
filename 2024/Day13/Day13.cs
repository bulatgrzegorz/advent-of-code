using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Xunit;

namespace adventOfCode._2024.Day13;

public class Day13
{
    [Fact]
    public void First()
    {
        var input = ParseInput(InputHelper.GetInputLines());

        decimal result = 0;
        foreach (var ((x1, y1), (x2, y2), (xp, yp)) in input)
        {
            var a = (x2 * yp - y2 * xp) / (y1 * x2 - x1 * y2);
            var b = (xp - a * x1) / x2;
            
            if(a is < 0 or > 100 || a % 1 is not 0 || b is < 0 or > 100 || b % 1 is not 0) continue;
            result += a * 3 + b;
        }
        
        Assert.Equal(36838, result);
    }
    
    [Fact]
    public void Second()
    {
        var input = ParseInput(InputHelper.GetInputLines(), 10000000000000);

        decimal result = 0;
        foreach (var ((x1, y1), (x2, y2), (xp, yp)) in input)
        {
            var a = (x2 * yp - y2 * xp) / (y1 * x2 - x1 * y2);
            var b = (xp - a * x1) / x2;
            
            if(a < 0 || a % 1 is not 0 || b < 0 || b % 1 is not 0) continue;
            result += a * 3 + b;
        }
        
        Assert.Equal(83029436920891, result);
    }

    private readonly record struct Tuple(decimal X, decimal Y);
    
    private static IEnumerable<(Tuple A, Tuple B, Tuple Price)> ParseInput(string[] input, decimal add = 0)
    {
        var exampleLines = input.Where(x => !string.IsNullOrWhiteSpace(x));

        var chunk = exampleLines.Chunk(3).ToList();
        foreach (var line in chunk)
        {
            var buttonA = Parse(line[0], @"Button A: X\+(\d+), Y\+(\d+)");
            var buttonB = Parse(line[1], @"Button B: X\+(\d+), Y\+(\d+)");
            var price = Parse(line[2], @"Prize: X=(\d+), Y=(\d+)", add);

            yield return (buttonA, buttonB, price);
        }
    }

    private static Tuple Parse(string input, [StringSyntax("Regex")] string pattern, decimal add = 0)
    {
        var match = Regex.Match(input, pattern);
        return new Tuple(int.Parse(match.Groups[1].Value) + add, int.Parse(match.Groups[2].Value) + add);
    }
}