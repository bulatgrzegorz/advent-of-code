using Xunit;

namespace adventOfCode.Day9;

public class Day9
{
    private const string InputFile = "Day9/Day9.input";

    [Fact]
    public void Second()
    {
        var (files, blanks) = CreateMemory2(File.ReadAllText(InputFile));

        var lastFile = files.Last();
        var lastFileIndex = lastFile.Key;
        while (lastFileIndex > 0)
        {
            var file = files[lastFileIndex];
            foreach (var (i, blank) in blanks.Index())
            {
                if (blank.index >= file.index)
                {
                    blanks = blanks[..i];
                    break;
                }

                if (file.size > blank.size) continue;
                
                files[lastFileIndex] = (blank.index, file.size);
                
                if (file.size == blank.size) blanks.RemoveAt(i);
                else blanks[i] = (blank.index + file.size, blank.size - file.size);
                    
                break;
            }
            
            lastFileIndex--;
        }

        var result = files.Sum(x => Enumerable.Range(x.Value.index, x.Value.size).Sum(y => (long)x.Key*y));
        
        Assert.Equal(6408966547049, result);
    }
    
    [Fact]
    public void First()
    {
        var memory = CreateMemory(File.ReadAllText(InputFile)).ToArray();
        
        var blanks = memory.Index().Where(x => x.Item is -1).Select(x => x.Index);

        var memorySpan = memory.AsSpan();
        foreach (var blank in blanks)
        {
            memorySpan = memorySpan[..(memorySpan.LastIndexOfAnyExcept(-1) + 1)];
            
            if(blank >= memorySpan.Length) break;
            memorySpan[blank] = memorySpan[^1];
            memorySpan = memorySpan[..^1];
        }

        var result = SumElementAndItemProduct(memorySpan);

        Assert.Equal(6384282079460, result);
    }

    private static long SumElementAndItemProduct(Span<int> memorySpan)
    {
        long result = 0;
        for (var i = 0; i < memorySpan.Length; i++)
        {
            result += memorySpan[i]*i;
        }

        return result;
    }
    
    private IEnumerable<int> CreateMemory(string input)
    {
        var fileIndex = 0;
        for (var index = 0; index < input.Length; index++)
        {
            var length = input[index] - '0';
            if (index % 2 == 0)
            {
                for (var i = 0; i < length; i++)
                {
                    yield return fileIndex;
                }
                
                fileIndex++;
            }
            else
            {
                for (var i = 0; i < length; i++)
                {
                    yield return -1;
                }
            }
        }
    }
    
    private (Dictionary<int, (int index, int size)> files, List<(int index, int size)> blanks) CreateMemory2(string input)
    {
        var files = new Dictionary<int, (int index, int size)>();
        var blanks = new List<(int index, int size)>();
        
        var fileIndex = 0;
        var position = 0;
        for (var index = 0; index < input.Length; index++)
        {
            var length = input[index] - '0';
            if(length is 0) continue;
            
            if (index % 2 == 0)
            {
                files[fileIndex++] = (position, length);
            }
            else
            {
                blanks.Add((position, length));
            }
            
            position += length;
        }
        
        return (files, blanks);
    }
}