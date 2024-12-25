using Xunit;

namespace adventOfCode._2024.Day25;

public class Day25
{
    [Fact]
    public void Test()
    {
        var input = ParseInput(InputHelper.GetInputLines()).ToArray();

        var result = 0;
        foreach (var (key, _) in input.Where(x => x.isKey))
        {
            foreach (var (@lock, _) in input.Where(x => !x.isKey))
            {
                result += key.Zip(@lock).All(x => x.First + x.Second <= 5) ? 1 : 0;
            }
        }
        
        Assert.Equal(3466, result);
    }

    private IEnumerable<(int[] heights, bool isKey)> ParseInput(string[] input)
    {
        var skipped = 0;
        while (skipped < input.Length)
        {
            var single = input.Skip(skipped).TakeWhile(x => !string.IsNullOrEmpty(x)).ToArray();
            var isKey = single[0].AsSpan().IndexOfAnyExcept('#') > -1;
            var result = new int[5];
            Array.Fill(result, -1);
            foreach (var line in single)
            {
                for (var i = 0; i < 5; i++)
                {
                    result[i] += line[i] is '#' ? 1 : 0;
                }
            }
            
            yield return (result, isKey);
            
            skipped += 8;
        }
    }
}