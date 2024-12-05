using System.Buffers;

namespace adventOfCode;

public static class SpanExtensions
{
    public static int IndexOf(this ReadOnlySpan<char> span, string value, int startIndex)
    {
        var indexInSlice = span[startIndex..].IndexOf(value);

        if (indexInSlice == -1)
        {
            return -1;
        }

        return startIndex + indexInSlice;
    }
    
    public static List<int> IndexOfAll(this ReadOnlySpan<char> span, SearchValues<string> searchValues, int lengthOfValues)
    {
        var startIndex = 0;
        var result = new List<int>();
        while (true)
        {
            var indexInSlice = span[startIndex..].IndexOfAny(searchValues);
            if (indexInSlice == -1)
            {
                break;
            }
            
            result.Add(indexInSlice + startIndex);
            startIndex += indexInSlice + lengthOfValues;
        }
        
        return result;
    }
    
    public static int LastIndexOf(this ReadOnlySpan<char> span, string value, int startIndex, int count)
    {
        var indexInSlice = span[(startIndex-count)..startIndex].LastIndexOf(value);

        if (indexInSlice == -1)
        {
            return -1;
        }

        return startIndex - count + indexInSlice;
    }
}