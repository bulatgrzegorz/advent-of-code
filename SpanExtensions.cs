using System.Buffers;

namespace adventOfCode;

public static class SpanExtensions
{
    extension(ReadOnlySpan<char> span)
    {
        public int IndexOf(string value, int startIndex)
        {
            var indexInSlice = span[startIndex..].IndexOf(value);

            if (indexInSlice == -1)
            {
                return -1;
            }

            return startIndex + indexInSlice;
        }

        public ILookup<int, int> IndexOfAll(ReadOnlySpan<string> searchValues)
        {
            var result = new List<(int index, int length)>();
            foreach (var searchValue in searchValues)
            {
                var index = 0;
                while (true)
                {
                    var indexInSlice = span[index..].IndexOf(searchValue);
                    if (indexInSlice == -1)
                    {
                        break;
                    }
            
                    result.Add((indexInSlice + index, searchValue.Length));
                    index += indexInSlice + searchValue.Length;
                }
            }
        
            return result.ToLookup(x => x.index, x => x.length);
        }

        public List<int> IndexOfAll(SearchValues<string> searchValues, int lengthOfValues)
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

        public int LastIndexOf(string value, int startIndex, int count)
        {
            var indexInSlice = span[(startIndex-count)..startIndex].LastIndexOf(value);

            if (indexInSlice == -1)
            {
                return -1;
            }

            return startIndex - count + indexInSlice;
        }
    }

    public static int MiddleElement(this Span<int> elements) => elements[elements.Length/2];
}