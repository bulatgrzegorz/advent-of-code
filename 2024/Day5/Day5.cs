using Xunit;
using OrderingRules = System.Linq.ILookup<int,int>;
using Input = (System.Linq.ILookup<int,int> orderingRules, System.Collections.Generic.List<int[]> updates);

namespace adventOfCode._2024.Day5;

public class Day5
{
    private const string ExampleInput = """
                                        47|53
                                        97|13
                                        97|61
                                        97|47
                                        75|29
                                        61|13
                                        75|53
                                        29|13
                                        97|29
                                        53|29
                                        61|53
                                        97|53
                                        61|29
                                        47|13
                                        75|47
                                        97|75
                                        47|61
                                        75|61
                                        47|29
                                        75|13
                                        53|13

                                        75,47,61,53,29
                                        97,61,53,29,13
                                        75,29,13
                                        75,97,47,61,53
                                        61,13,29
                                        97,13,75,29,47
                                        """;
    
    [Fact]
    public void Example()
    {
        var input = GetInput(ExampleInput.Split(Environment.NewLine));
        
        var sum = ValidateUpdates(input, (t, x) => t ? x : [])
            .Where(x => x is {Length: > 0})
            .Sum(x => x.AsSpan().MiddleElement());

        Assert.Equal(143, sum);
    }
    
    [Fact]
    public void First()
    {
        var input = GetInput(InputHelper.ReadInputLines());
        
        var sum = ValidateUpdates(input, (t, x) => t ? x : [])
            .Where(x => x is {Length: > 0})
            .Sum(x => x.AsSpan().MiddleElement());

        Assert.Equal(5129, sum);
    }
    
    [Fact]
    public void Second()
    {
        var input = GetInput(InputHelper.ReadInputLines());
        
        var comparer = Comparer<int>.Create((x, y) => input.orderingRules[x].Contains(y) ? 1 : -1);
        var sum = ValidateUpdates(input, (t, x) => t ? [] : x.Order(comparer).ToArray())
            .Where(x => x is {Length: > 0})
            .Sum(x => x.AsSpan().MiddleElement());

        Assert.Equal(4077, sum);
    }

    private static IEnumerable<int[]> ValidateUpdates(Input input, Func<bool, int[], int[]> updatesFunc)
    {
        foreach (var update in input.updates)
        {
            var isUpdateCorrect = CheckUpdate(update, input.orderingRules);
            yield return updatesFunc(isUpdateCorrect, update);
        }
    }

    private static bool CheckUpdate(int[] update, OrderingRules orderingRules)
    {
        var pages = update.AsSpan();
        for (var i = 0; i < update.Length; i++)
        {
            foreach (var mustBeBefore in orderingRules[update[i]])
            {
                //we are checking if after given page there are any pages that should be placed before it
                if (pages[(i+1)..].Contains(mustBeBefore))
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    private static Input GetInput(IEnumerable<string> inputLines)
    {
        var orderingRules = new List<(int first, int second)>();
        var updates = new List<int[]>();
        var inputOrderingRulesPart = true;
        foreach (var line in inputLines)
        {
            if (string.IsNullOrEmpty(line))
            {
                inputOrderingRulesPart = false; 
                continue;
            }

            if (inputOrderingRulesPart)
            {
                var orders = line.Split("|");
                orderingRules.Add((int.Parse(orders[0]), int.Parse(orders[1])));
            }
            else
            {
                updates.Add(line.Split(",").Select(int.Parse).ToArray());
            }
        }
        
        return (orderingRules.ToLookup(x => x.second, x => x.first), updates);
    }
}