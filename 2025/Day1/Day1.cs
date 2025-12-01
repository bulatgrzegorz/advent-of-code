using Xunit;

namespace adventOfCode._2025.Day1;

public class Day1
{
    private const string ExampleInput = """
                                        L68
                                        L30
                                        R48
                                        L5
                                        R60
                                        L55
                                        L1
                                        L99
                                        R14
                                        L82
                                        """;
    [Fact]
    public void Example()
    {
        var input = ExampleInput.Split(Environment.NewLine);

        var currentPosition = 50;

        var zeros = 0;
        foreach (var (s, x) in input.Select(x => (x[0], int.Parse(x[1..]))))
        {
            currentPosition = s switch
            {
                'R' => (currentPosition + x) % 100,
                'L' => currentPosition < x ? 100 - (x - currentPosition) : currentPosition - x,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            zeros += currentPosition == 0 ? 1 : 0;
        }

        Assert.Equal(3, zeros);
    }
    
    [Fact]
    public void First()
    {
        var input = InputHelper.ReadInputLines();
        var currentPosition = 50;

        var zeros = 0;
        foreach (var (s, x) in input.Select(x => (x[0], int.Parse(x[1..]))))
        {
            var rotation = x % 100;
            
            currentPosition = s switch
            {
                'R' => (currentPosition + rotation) % 100,
                'L' => currentPosition < rotation ? 100 - (rotation - currentPosition) : currentPosition - rotation,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            zeros += currentPosition == 0 ? 1 : 0;
        }

        Assert.Equal(1084, zeros);
    }
    
    [Fact]
    public void Second()
    {
        var input = InputHelper.ReadInputLines();
        var currentPosition = 50;

        var zeros = 0;
        foreach (var (s, x) in input.Select(x => (x[0], int.Parse(x[1..]))))
        {
            var rotation = x % 100;
            
            var newCurrentPosition = s switch
            {
                'R' => (currentPosition + rotation) % 100,
                'L' => currentPosition < rotation ? 100 - (rotation - currentPosition) : currentPosition - rotation,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var overZero = s switch
            {
                'R' => newCurrentPosition < currentPosition, //moved from left side of zero to right side of zero
                'L' => currentPosition != 0 && newCurrentPosition > currentPosition, //moved from right side of zero to left side of zero (not started from zero)
                _ => throw new ArgumentOutOfRangeException()
            };
            
            zeros += newCurrentPosition == 0 || overZero ? 1 : 0;
            zeros += x / 100; //add full rotations 
            
            currentPosition = newCurrentPosition;
        }

        Assert.Equal(6475, zeros); 
    }
}