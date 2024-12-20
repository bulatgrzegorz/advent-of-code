using System.Collections.Frozen;
using Xunit;

namespace adventOfCode._2024.Day19;

public class Day19
{
    [Fact]
    public void First()
    {
        var exampleLines = InputHelper.GetInputLines();
        var possibilities = exampleLines[0].Split(", ").ToFrozenSet();

        var towels = exampleLines[2..];
        var result = towels.Select(x => Check(0, x, possibilities, [])).Count(x => x > 0);

        Assert.Equal(240, result);
    }
    
    [Fact]
    public void Second()
    {
        var exampleLines = InputHelper.GetInputLines();
        var possibilities = exampleLines[0].Split(", ").ToFrozenSet();

        var towels = exampleLines[2..];
        var result = towels.Select(x => Check(0, x, possibilities, [])).Sum(x => x);

        Assert.Equal(848076019766013, result);
    }

    private static long Check(int currentSum, string toCheck, FrozenSet<string> possibilities, Dictionary<int, long> cache)
    {
        if (currentSum == toCheck.Length) return 1;

        var result = 0L;
        for (var i = 1; i <= toCheck.Length - currentSum; i++)
        {
            var next = currentSum + i;
            var partToCheck = toCheck[currentSum..next];
            if (!possibilities.Contains(partToCheck)) continue;
            
            if (cache.TryGetValue(next, out var value))
            {
                result += value;
            }
            else
            {
                var r = Check(currentSum + i, toCheck, possibilities, cache);
                cache.TryAdd(next, r);
                result += r;
            }
        }

        return result;
    }
}