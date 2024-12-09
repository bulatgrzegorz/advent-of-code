using Xunit;

namespace adventOfCode.Day9;

public class Day9
{
    private const string InputFile = "Day9/Day9.input";
    private const string ExampleInput = "2333133121414131402";

    [Fact]
    public void Example()
    {
        var memory = CreateMemory(ExampleInput).ToArray().AsSpan();

        var result = CompactFirst(memory);

        Assert.Equal(1928, result);
    }
    
    [Fact]
    public void ExampleSecond()
    {
        var memory = CreateMemory(ExampleInput).ToArray().AsSpan();

        var result = CompactSecond2(memory);

        Assert.Equal(2858, result);
    }
    
    [Fact]
    public void Second()
    {
        var memory = CreateMemory(File.ReadAllText(InputFile)).ToArray().AsSpan();

        var result = CompactSecond2(memory);

        Assert.Equal(6408966547049, result);
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

    private static long CompactSecond2(Span<string> memory)
    {
        var mem = memory;
        while (true)
        {
            var lastDigitIndex = mem.LastIndexOfAnyExcept(".");
            var firstDigitIndex = FirstDigitIndex(mem, lastDigitIndex);
        
            var firstFreeSpaceIndex = mem.IndexOf(".");
            if(firstFreeSpaceIndex < 0) break;

            var span = mem[firstFreeSpaceIndex..(lastDigitIndex+1)];
            while (span.Length > 0 && CompactSecondInternal(span))
            {
                firstFreeSpaceIndex = span.IndexOf(".");
                var lastFreeSpaceIndex = LastIndex(span, firstFreeSpaceIndex);
                
                span = span[(lastFreeSpaceIndex+1)..];
            }

            mem = mem[..firstDigitIndex];
        }
        
        long result = 0;
        for (var i = 0; i < memory.LastIndexOfAnyExcept(".") + 1; i++)
        {
            if(memory[i] is ".") continue;
            result += long.Parse(memory[i])*i;
        }

        return result;
    }

    private static bool CompactSecondInternal(Span<string> memory)
    {
        var lastDigitIndex = memory.LastIndexOfAnyExcept(".");
        var firstDigitIndex = FirstDigitIndex(memory, lastDigitIndex);
        
        var firstFreeSpaceIndex = memory.IndexOf(".");
        if(firstFreeSpaceIndex < 0) return false;
        var lastFreeSpaceIndex = LastIndex(memory, firstFreeSpaceIndex);
        
        var digitLength = lastDigitIndex - firstDigitIndex + 1;
        var freeSpaceLength = lastFreeSpaceIndex - firstFreeSpaceIndex + 1;
        if (lastFreeSpaceIndex > lastDigitIndex)
        {
            return false;
        }

        if (digitLength > freeSpaceLength) return true;
        
        var fi = firstFreeSpaceIndex;
        var dl = lastDigitIndex;
        for (var i = 0; i < digitLength; i++)
        {
            (memory[fi++], memory[dl]) = (memory[dl--], ".");
        }

        return false;

    }

    private static int FirstDigitIndex(ReadOnlySpan<string> memory, int index)
    {
        var digit = memory[index];
        while (index > 0)
        {
            index -= 1;
            if(memory[index] == digit) continue;

            return index + 1;
        }

        return index;
    }
    
    private static int LastIndex(ReadOnlySpan<string> memory, int index)
    {
        var value = memory[index];
        while (index < memory.Length - 1)
        {
            index += 1;
            if(memory[index] == value) continue;

            return index - 1;
        }

        return index;
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
}