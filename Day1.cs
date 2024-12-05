using System.Collections.Immutable;
using Xunit;

namespace adventOfCode;

public class Day1
{
    private const string InputFile = "Day1.txt";
    private const string ExampleInput = """
                                        3   4
                                        4   3
                                        2   5
                                        1   3
                                        3   9
                                        3   3
                                        """;
    [Fact]
    public void Example()
    {
        var input = ExampleInput.Split(Environment.NewLine);
        var sum = CalculateDistance(input);

        Assert.Equal(11, sum);
    }
    
    [Fact]
    public void First()
    {
        var input = File.ReadAllLines(InputFile);
        var sum = CalculateDistance(input);

        Assert.Equal(2264607, sum);
    }
    
    [Fact]
    public void Second()
    {
        var input = File.ReadAllLines(InputFile);
        var sum = CalculateSimilarity(input);

        Assert.Equal(19457120, sum);
    }

    private static int CalculateDistance(string[] input)
    {
        Span<int> first = new int[input.Length];
        Span<int> second = new int[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            var nums = input[i].Split("   ");
            first[i] = int.Parse(nums[0]);
            second[i] = int.Parse(nums[1]);
        }
        
        first.Sort();
        second.Sort();

        var sum = 0;
        for (var i = 0; i < first.Length; i++)
        {
            sum += Math.Abs(first[i] - second[i]);
        }

        return sum;
    }
    
    private static int CalculateSimilarity(string[] input)
    {
        Span<int> first = new int[input.Length];
        var second = new int[input.Length];
        for (var i = 0; i < input.Length; i++)
        {
            var nums = input[i].Split("   ");
            first[i] = int.Parse(nums[0]);
            second[i] = int.Parse(nums[1]);
        }
        
        var secondCountLookup = second
            .CountBy(x => x)
            .ToImmutableDictionary(x => x.Key, x => x.Value);
        
        var similarity = 0;
        foreach (var x in first)
        {
            similarity += x * secondCountLookup.GetValueOrDefault(x, 0);
        }

        return similarity;
    }
}