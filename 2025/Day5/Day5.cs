using Xunit;

namespace adventOfCode._2025.Day5;

public class Day5
{
    private const string ExampleInput = """
                                        3-5
                                        10-14
                                        16-20
                                        12-18
                                        
                                        1
                                        5
                                        8
                                        11
                                        17
                                        32
                                        """;

    private readonly record struct Range(long Start, long End)
    {
        public bool IsInRange(long value) => value >= Start && value <= End;

        public bool Intersects(Range other) => End >= other.Start && Start <= other.End;
    };
    
    [Fact]
    public void Example()
    {
        List<Range> ranges = new();
        List<long> ids = new();
        foreach (var line in ExampleInput.Split(Environment.NewLine).Select(x => x.ReplaceLineEndings(string.Empty)))
        {
            if(string.IsNullOrEmpty(line)) continue;
            var parts = line.Split('-');
            if (parts.Length == 2)
            {
                ranges.Add(new Range(long.Parse(parts[0]), long.Parse(parts[1])));
            }
            else
            {
                ids.Add(long.Parse(line));
            }
        }

        var result = ids.Count(x => ranges.Any(y => y.IsInRange(x)));
        
        Assert.Equal(3, result);
    }
    
    [Fact]
    public void First()
    {
        List<Range> ranges = new();
        List<long> ids = new();
        foreach (var line in InputHelper.ReadInputLines())
        {
            if(string.IsNullOrEmpty(line)) continue;
            var parts = line.Split('-');
            if (parts.Length == 2)
            {
                ranges.Add(new Range(long.Parse(parts[0]), long.Parse(parts[1])));
            }
            else
            {
                ids.Add(long.Parse(line));
            }
        }

        var result = ids.Count(x => ranges.Any(y => y.IsInRange(x)));
        
        Assert.Equal(888, result);
    }
    
    [Fact]
    public void Second()
    {
        HashSet<Range> ranges = [];
        
        foreach (var line in InputHelper.ReadInputLines().TakeWhile(x => !string.IsNullOrEmpty(x)))
        {
            var parts = line.Split('-');
            ranges.Add(new Range(long.Parse(parts[0]), long.Parse(parts[1])));
        }

        bool anyOther;
        do
        {
            HashSet<Range> newRanges = [];
            anyOther = false;
            foreach (var range in ranges)
            {
                var allIntersecting = ranges.Where(x => range.Intersects(x)).ToArray();
                if (allIntersecting.Length == 1)
                {
                    newRanges.Add(range);
                    continue;
                }
                
                anyOther = true;
                
                newRanges.Add(new Range(allIntersecting.Min(x => x.Start), allIntersecting.Max(x => x.End)));
            }
            
            ranges = newRanges;
        }
        while (anyOther);

        var sum = ranges.Sum(x => x.End - x.Start + 1);
        
        Assert.Equal(344378119285354, sum);
    }
}