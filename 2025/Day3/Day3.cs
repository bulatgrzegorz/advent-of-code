using Xunit;

namespace adventOfCode._2025.Day3;

public class Day3
{
    private const string ExampleInput = """
                                        987654321111111
                                        811111111111119
                                        234234234234278
                                        818181911112111
                                        """;
    [Fact]
    public void Example()
    {
        var input = ExampleInput.Split(Environment.NewLine).Select(x => x.ReplaceLineEndings(string.Empty));

        long sum = 0;

        foreach (var item in input.Select(x => x.ToCharArray().Select(y => int.Parse(y.ToString())).ToArray()))
        {
            var highest = 0;
            for (var i = 0; i < item.Length - 1; i++)
            {
                var number = item[i] * 10 + item[(i+1)..].Max();
                highest = number > highest ? number : highest;
            }
            
            sum += highest;
        }
        
        Assert.Equal(357, sum);
    }
    
    [Fact]
    public void First()
    {
        var input = InputHelper.ReadInputLines();

        long sum = 0;

        foreach (var item in input.Select(x => x.ToCharArray().Select(y => int.Parse(y.ToString())).ToArray()))
        {
            var highest = 0;
            for (var i = 0; i < item.Length - 1; i++)
            {
                var number = item[i] * 10 + item[(i+1)..].Max();
                highest = number > highest ? number : highest;
            }
            
            sum += highest;
        }
        
        Assert.Equal(17034, sum);
    }
    
    [Fact]
    public void Second()
    {
        var input = InputHelper.ReadInputLines();
        
        long sum = 0;

        foreach (var items in input.Select(x => x.ToCharArray().Select(y => int.Parse(y.ToString())).Index().ToArray()))
        {
            var digits = 0;
            var indexes = new int[12];
            
            FillGapsWithHighest(items, indexes, ref digits);
            
            sum += CreateResult(items, indexes);
        }
        
        Assert.Equal(168798209663590, sum);
    }

    private void FillGapsWithHighest((int index, int item)[] items, int[] indexes, ref int digits)
    {
        if(items is not {Length: >0 }) return;

        var highestItem = items.MaxBy(x => x.item);
        var highestItems = items.Index().Where(x => x.Item.item == highestItem.item);

        var chunks = new List<(int start, int end)>();
        
        var start = 0;
        foreach (var (relativeIndex, (absoluteIndex, _)) in highestItems.OrderBy(x => x.Item.index))
        {
            chunks.Add((start, relativeIndex));
            start = relativeIndex + 1;
            indexes[digits++] = absoluteIndex;
            
            if(digits == 12) return;
        }
        chunks.Add((start, items.Length));

        foreach (var index in chunks.OrderByDescending(x => x.start))
        {
            FillGapsWithHighest(items[index.start..index.end], indexes, ref digits);
            
            if(digits == 12) return;
        }
    }

    private static long CreateResult((int Index, int Item)[] items, int[] indexes)
    {
        return indexes.OrderBy(x => x).Aggregate<int, long>(0, (current, index) => current * 10 + items[index].Item);
    }
}