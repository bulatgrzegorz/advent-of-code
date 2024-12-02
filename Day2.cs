using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace adventOfCode2024;

public class Day2
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Day2(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private const string InputFile = "Day2.txt";
    private const string ExampleInput = """
                                        7 6 4 2 1
                                        1 2 7 8 9
                                        9 7 6 2 1
                                        1 3 2 4 5
                                        8 6 4 4 1
                                        1 3 6 7 9
                                        """;
    [Fact]
    public void Example()
    {
        var input = ExampleInput.Split(Environment.NewLine);
        var reports = input.Select(x => x.Split(' ').Select(int.Parse).ToArray());

        var sum = 0;
        foreach (var report in reports)
        {
            sum += IsReportSafe(report) ? 1 : 0;
        }
        
        Assert.Equal(2, sum);
    }
    
    [Fact]
    public void First()
    {
        var input = File.ReadAllLines(InputFile);
        var reports = input.Select(x => x.Split(' ').Select(int.Parse).ToArray());

        var sum = 0;
        foreach (var report in reports)
        {
            sum += IsReportSafe(report) ? 1 : 0;
        }
        
        Assert.Equal(202, sum);
    }
    
    [Theory]
    [InlineData("11 14 15 12 14 16 17 17", false)]
    [InlineData("1 1 2 9 9", false)]
    [InlineData("55 55 59 62 66", false)]
    [InlineData("16 19 16 13 12", true)]
    [InlineData("63 63 64 65 67 70 72", true)]
    public void SecondTests(string example, bool expected)
    {
        int[] a = [1, 2, 3];
        var b = a[..1];
        var b2 = a[1..];
        
        var report = example.Split(' ').Select(int.Parse).ToArray();

        Assert.Equal(expected, IsReportSafe2(report));
    }
    
    [Fact]
    public void SecondExample()
    {
        var input = ExampleInput.Split(Environment.NewLine);
        var reports = input.Select(x => x.Split(' ').Select(int.Parse).ToArray());

        var sum = 0;
        foreach (var report in reports)
        {
            sum += IsReportSafe2(report) ? 1 : 0;
        }
        
        Assert.Equal(4, sum);
    }
    
    [Fact]
    public void Second()
    {
        var input = File.ReadAllLines(InputFile);
        var reports = input.Select(x => x.Split(' ').Select(int.Parse).ToArray());

        var sum = 0;
        foreach (var report in reports)
        {
            sum += IsReportSafe2(report) ? 1 : 0;
        }
        
        Assert.Equal(271, sum);
    }

    private static bool IsReportSafe(int[] report)
    {
        int? sign = default;
        for (var i = 1; i < report.Length; i++)
        {
            var diff = report[i] - report[i - 1];
            sign ??= diff < 0 ? -1 : 1;
            
            if (!IsDiffCorrectSize(diff)) return false;
            if (!IsDiffCorrectSign(diff, sign)) return false;
        }

        return true;
    }
    
    private static bool IsReportSafe2(Span<int> report, bool errorMode = false)
    {
        int? sign = default;
        for (var i = 1; i < report.Length; i++)
        {
            var diff = report[i] - report[i - 1];
            sign ??= diff < 0 ? -1 : 1;

            if(IsDiffCorrectSize(diff) && IsDiffCorrectSign(diff, sign)) continue;
            if(errorMode) return false;
            
            //if it's already end, just remove last index and everything will be fine
            if(i == report.Length - 1) return true;

            //otherwise we will check other possibilities after removing some of the elements
            for (var j = 0; j <= i; j++)
            {
                if(IsReportSafe2(ExcludeElement(report, j), true)) return true;
            }

            return false;
        }
        
        return true;
    }

    private static int[] ExcludeElement(Span<int> span, int index)
    {
        return [..span[..index], ..span[(index + 1)..]];
    }
        

    private static bool IsDiffCorrectSign(int diff, [DisallowNull] int? sign)
    {
        return diff < 0 && sign is -1 || diff > 0 && sign is 1;
    }

    private static bool IsDiffCorrectSize(int diff)
    {
        return Math.Abs(diff) is >= 1 and <= 3;
    }
}