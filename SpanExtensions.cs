namespace adventOfCode2024;

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