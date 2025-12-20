using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace adventOfCode._2025.Day10;

public class Day10
{
    private const string ExampleInput = """
                                        [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
                                        [...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
                                        [.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
                                        """;

    [Fact]
    public void First()
    {
        // var lines = ExampleInput.GetExampleInputLines();
        var lines = InputHelper.GetInputLines();

        var result = 0;
        
        var m = lines.Select(x => Regex.Matches(x, @"\[([\.#]+)\]|\(([\d,]+)\)|\{([\d,]+)\}"));
        foreach (var match in m)
        {
            var mask = MaskToBitArray(match[0].Groups[1].ValueSpan);
            PrintBitArray(mask);
            var toggles = match.ToArray()[1..^1].Select(x => x.Groups[2].Value.Split(',').Select(int.Parse))
                .Select(x => TogglesToBitArray(mask.Length, x))
                .ToArray();

            foreach (var toggle in toggles)
            {
                PrintBitArray(toggle);
            }

            BitArray[] start = [new(mask.Length)];

            var generation = 1;
            while (true)
            {
                start = Do(start, toggles).ToArray();

                // Console.WriteLine();
                // for (int i = 0; i < start.Length; i++)
                // {
                //     Console.WriteLine($"[{i}]: {PrintBitArray2(start[i])}");
                // }
                //
                if(start.Any(x =>
                   {
                       var r = x.Xor(mask);
                       var iss = !r.HasAnySet();
                       x.Xor(mask);
                       return iss;
                   })) break;
                
                generation++;
            }
            
            result += generation;
        }
        
        Assert.Equal(4759930955, result);
    }

    IEnumerable<BitArray> Do(IEnumerable<BitArray> starts, BitArray[] toggles)
    {
        return
            from start in starts
            from toggle in toggles
            select ((BitArray)start.Clone()).Xor(toggle);
    }
    
    private BitArray TogglesToBitArray(int maskLength, IEnumerable<int> toggles)
    {
        var bitArray = new BitArray(maskLength);
        foreach (var toggle in toggles)
        {
            bitArray[toggle] = true;
        }
        
        return bitArray;
    }

    private BitArray MaskToBitArray(ReadOnlySpan<char> input)
    {
        var bitArray = new BitArray(input.Length);
        for (var i = 0; i < input.Length; i++)
        {
            bitArray[i] = input[i] != '.';
        }
        
        return bitArray;
    }

    private void PrintBitArray(BitArray bitArray)
    {
        foreach (bool b in bitArray)
        {
            Console.Write(b ? '#' : '.');
        }

        Console.WriteLine();
    }
    
    private string PrintBitArray2(BitArray bitArray)
    {
        var sb = new StringBuilder();
        foreach (bool b in bitArray)
        {
            sb.Append(b ? '#' : '.');
        }

        return sb.ToString();
    }
    
    [Fact]
    public void Second()
    {
        // var lines = ExampleInput.GetExampleInputLines();
        var lines = InputHelper.GetInputLines();

        Assert.Equal(4759930955, 1);
    }
}