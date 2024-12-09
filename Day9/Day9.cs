using Xunit;

namespace adventOfCode.Day9;

public class Day9
{
    private const string InputFile = "Day9/Day9.input";
    private const string ExampleInput = "2333133121414131402";
    private static string[] _digits = "123456789".Select(x => x.ToString()).ToArray();

    [Fact]
    public void Example()
    {
        var memory = CreateMemory(ExampleInput).ToArray().AsSpan();

        var result = CompactFirst(memory);

        Assert.Equal(1928, result);
    }
    
    [Fact]
    public void First()
    {
        var memory = CreateMemory(File.ReadAllText(InputFile)).ToArray().AsSpan();

        var result = CompactFirst(memory);

        Assert.Equal(6384282079460, result);
    }

    private static long CompactFirst(Span<string> memory)
    {
        while (true)
        {
            var lastDigitIndex = memory.LastIndexOfAnyExcept(".");
            var firstFreeSpaceIndex = memory.IndexOf(".");
            
            if(firstFreeSpaceIndex > lastDigitIndex) break;

            (memory[firstFreeSpaceIndex], memory[lastDigitIndex]) = (memory[lastDigitIndex], ".");
        }

        long result = 0;
        for (var i = 0; i < memory.IndexOf("."); i++)
        {
            result += long.Parse(memory[i])*i;
        }

        return result;
    }
    
    private static long CompactSecond(Span<string> memory)
    {
        while (true)
        {
            var lastDigitIndex = memory.LastIndexOfAnyExcept(".");
            var firstFreeSpaceIndex = memory.IndexOf(".");
            
            if(firstFreeSpaceIndex > lastDigitIndex) break;

            (memory[firstFreeSpaceIndex], memory[lastDigitIndex]) = (memory[lastDigitIndex], ".");
        }

        long result = 0;
        for (var i = 0; i < memory.IndexOf("."); i++)
        {
            result += long.Parse(memory[i])*i;
        }

        return result;
    }

    private IEnumerable<string> CreateMemory(string input)
    {
        var fileIndex = 0;
        for (var index = 0; index < input.Length; index++)
        {
            var length = input[index] - '0';
            if (index % 2 == 0)
            {
                for (var i = 0; i < length; i++)
                {
                    yield return fileIndex.ToString();
                }
                
                fileIndex++;
            }
            else
            {
                for (var i = 0; i < length; i++)
                {
                    yield return ".";
                }
            }
        }
    }

    private record struct Memory(int Length, int Index, int FileIndex, bool IsFreeSpace);
}