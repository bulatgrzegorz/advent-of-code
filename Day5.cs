using Xunit;
using OrderingRulesAfter = System.Linq.ILookup<int,int>;
using OrderingRulesBefore = System.Linq.ILookup<int,int>;
using Input = (System.Linq.ILookup<int,int> orderingRulesAfter, System.Linq.ILookup<int,int> orderingRulesBefore, System.Collections.Generic.List<int[]> updates);

namespace adventOfCode;

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
    private const string InputFile = "Day5.txt";
    
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
        var input = GetInput(File.ReadLines(InputFile));
        
        var sum = ValidateUpdates(input, (t, x) => t ? x : [])
            .Where(x => x is {Length: > 0})
            .Sum(x => x.AsSpan().MiddleElement());

        Assert.Equal(5129, sum);
    }
    
    [Fact]
    public void Second()
    {
        var input = GetInput(File.ReadLines(InputFile));
        var comparer = new OrderingRulesComparer(input.orderingRulesAfter, input.orderingRulesBefore);
        
        var sum = ValidateUpdates(input, (t, x) => t ? [] : x.Order(comparer).ToArray())
            .Where(x => x is {Length: > 0})
            .Sum(x => x.AsSpan().MiddleElement());

        Assert.Equal(4077, sum);
    }

    private class OrderingRulesComparer(OrderingRulesAfter orderingRulesAfter, OrderingRulesBefore orderingRulesBefore) : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (orderingRulesBefore[x].Contains(y)) return 1;
            if (orderingRulesAfter[x].Contains(y)) return -1;
            return 0;
        }
    }

    private static IEnumerable<int[]> ValidateUpdates(Input input, Func<bool, int[], int[]> updatesFunc)
    {
        foreach (var update in input.updates)
        {
            var isUpdateCorrect = CheckUpdate(update, input.orderingRulesBefore, input.orderingRulesAfter);
            yield return updatesFunc(isUpdateCorrect, update);
        }
    }

    private static bool CheckUpdate(int[] update, ILookup<int, int> orderingRulesBefore, ILookup<int, int> orderingRulesAfter)
    {
        for (var i = 0; i < update.Length; i++)
        {
            var pages = update.AsSpan();
            foreach (var mustBeBefore in orderingRulesBefore[update[i]])
            {
                //we are checking if after given page there are any pages that should be placed before it
                if (pages[(i+1)..].Contains(mustBeBefore))
                {
                    return false;
                }
            }

            foreach (var mustBeAfter in orderingRulesAfter[update[i]])
            {
                //we are checking if before given page there are any pages that should be placed after it
                if (pages[..i].Contains(mustBeAfter))
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
        
        OrderingRulesAfter orderingRulesAfter = orderingRules.ToLookup(x => x.first, x => x.second);
        OrderingRulesBefore orderingRulesBefore = orderingRules.ToLookup(x => x.second, x => x.first);
        
        return (orderingRulesAfter, orderingRulesBefore, updates);
    }
}