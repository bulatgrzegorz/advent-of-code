using System.Collections.Immutable;
using Xunit;

namespace adventOfCode;

public class Day4
{
    private const string InputFile = "Day4.input";

    [Fact]
    public void First()
    {
        var s = File.ReadLines(InputFile).Select(x => x.ToImmutableArray()).ToImmutableArray();
        
        var sum = 0;
        int[] possibleDirections = [-1, 0, 1];

        var rows = s.Length;
        var cols = s[0].Length;
        
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if(s[r][c] is not 'X') continue;
                foreach (var dr in possibleDirections)
                {
                    foreach (var dc in possibleDirections)
                    {
                        if(dr is 0 && dc is 0) continue;
                        if(!(0 <= r + 3*dr && r + 3*dr < rows)) continue;
                        if(!(0 <= c + 3*dc && c + 3*dc < cols)) continue;
                        
                        if(s[r + dr][c + dc] is not 'M') continue;
                        if(s[r + 2*dr][c + 2*dc] is not 'A') continue;
                        if(s[r + 3*dr][c + 3*dc] is not 'S') continue;

                        sum++;
                    }
                }
            }
        }
        
        Assert.Equal(2500, sum);
    }
    
    [Fact]
    public void Second()
    {
        var s = File.ReadLines(InputFile).Select(x => x.ToImmutableArray()).ToImmutableArray();
        
        var sum = 0;
        
        var rows = s.Length;
        var cols = s[0].Length;
        
        for (var r = 1; r < rows - 1; r++)
        {
            for (var c = 1; c < cols - 1; c++)
            {
                if(s[r][c] is not 'A') continue;

                if (s[r - 1][c - 1] is 'M' && s[r + 1][c + 1] is 'S')
                {
                    if(s[r-1][c+1] is 'M' && s[r+1][c-1] is 'S') sum++;
                    if(s[r-1][c+1] is 'S' && s[r+1][c-1] is 'M') sum++;
                }
                
                if (s[r - 1][c - 1] is 'S' && s[r + 1][c + 1] is 'M')
                {
                    if(s[r-1][c+1] is 'M' && s[r+1][c-1] is 'S') sum++;
                    if(s[r-1][c+1] is 'S' && s[r+1][c-1] is 'M') sum++;
                }
            }
        }
        
        Assert.Equal(1933, sum);
    }
}