using System.Collections.Immutable;
using Xunit;

namespace adventOfCode._2025.Day2;

public class Day2
{
    private const string ExampleInput = """
                                        11-22,95-115,998-1012,1188511880-1188511890,222220-222224,
                                        1698522-1698528,446443-446449,38593856-38593862,565653-565659,
                                        824824821-824824827,2121212118-2121212124
                                        """;
    [Fact]
    public void Example()
    {
        var input = ExampleInput.ReplaceLineEndings(string.Empty);

        long sum = 0;
        foreach (var ranges in input.Split(',').Select(x =>
                 {
                     var parts = x.Split('-');
                     return (startS: parts[0], start: long.Parse(parts[0]), endS: parts[1], end: long.Parse(parts[1]));
                 }))
        {
            var current = ranges.start;
            var currentSpan = ranges.startS.AsSpan();
            while (current <= ranges.end)
            {
                if (currentSpan.Length % 2 == 0)
                {
                    if (currentSpan[..(currentSpan.Length / 2)].SequenceEqual(currentSpan[(currentSpan.Length / 2)..]))
                    {
                        sum += current;
                    }
                }
                
                current++;
                currentSpan = current.ToString().AsSpan();
            }
        }

        Assert.Equal(1227775554, sum);
    }
    
    [Fact]
    public void First()
    {
        var input = InputHelper.GetInput();

        long sum = 0;
        foreach (var ranges in input.Split(',').Select(x =>
                 {
                     var parts = x.Split('-');
                     return (startS: parts[0], start: long.Parse(parts[0]), endS: parts[1], end: long.Parse(parts[1]));
                 }))
        {
            var current = ranges.start;
            var currentSpan = ranges.startS.AsSpan();
            while (current <= ranges.end)
            {
                if (currentSpan.Length % 2 == 0)
                {
                    if (currentSpan[..(currentSpan.Length / 2)].SequenceEqual(currentSpan[(currentSpan.Length / 2)..]))
                    {
                        sum += current;
                    }
                }
                
                current++;
                currentSpan = current.ToString().AsSpan();
            }
        }

        Assert.Equal(31839939622, sum);
    }
    
    [Fact]
    public void Second()
    {
        var input = InputHelper.GetInput();

        long sum = 0;
        foreach (var ranges in input.Split(',').Select(x =>
                 {
                     var parts = x.Split('-');
                     return (startS: parts[0], start: long.Parse(parts[0]), endS: parts[1], end: long.Parse(parts[1]));
                 }))
        {
            var current = ranges.start;
            var currentSpan = ranges.startS.AsSpan();
            while (current <= ranges.end)
            {
                for (var i = 1; i <= currentSpan.Length / 2; i++) // we are iterating just to middle point, there is no point to go further as you will not be able to find pair with such length
                {
                    if(currentSpan.Length % i != 0) continue; //if we cannot fit our chunk in number evenly, quick fail

                    if(currentSpan.ContainsAnyExcept(currentSpan[..i])) continue; //if number contains different digits than we have in our slice, quick fail 
                    
                    var chunks = currentSpan.ToImmutableArray().Chunk(i).Skip(1); //split number into chunks of given length

                    var lookingFor = currentSpan[..i].ToArray();

                    if (!chunks.All(x => x.SequenceEqual(lookingFor))) continue;
                    
                    sum += current;
                    break;
                }
                
                current++;
                currentSpan = current.ToString().AsSpan();
            }
        }

        Assert.Equal(41662374059, sum);
    }
}