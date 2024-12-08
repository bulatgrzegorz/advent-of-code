using Xunit;

namespace adventOfCode;

public class Day7
{
    private const string ExampleInput = """
                                        190: 10 19
                                        3267: 81 40 27
                                        83: 17 5
                                        156: 15 6
                                        7290: 6 8 6 15
                                        161011: 16 10 13
                                        192: 17 8 14
                                        21037: 9 7 18 13
                                        292: 11 6 16 20
                                        """;

    private const string InputFile = "Day7.txt";

    private enum Operation
    {
        Sum,
        Mul,
        Join
    };
    
    [Fact]
    private void Example()
    {
        var input = ParseInput(ExampleInput.Split(Environment.NewLine));
        UInt128 result = 0;
        Operation[] operations = [Operation.Sum, Operation.Mul];
        foreach (var (testValue, numbers) in input)
        {
            if (CanBeProduced(testValue, numbers[0], numbers[1..], operations))
            {
                result += testValue;
            }
        }
        
        Assert.Equal((ulong)3749, result);
    }
    
    [Fact]
    private void ExampleSecond()
    {
        var input = ParseInput(ExampleInput.Split(Environment.NewLine));
        UInt128 result = 0;
        Operation[] operations = [Operation.Sum, Operation.Mul, Operation.Join];
        foreach (var (testValue, numbers) in input)
        {
            if (CanBeProduced(testValue, numbers[0], numbers[1..], operations))
            {
                result += testValue;
            }
        }
        
        Assert.Equal((UInt128)11387, result);
    }
    
    [Fact]
    private void First()
    {
        var input = ParseInput(File.ReadAllLines(InputFile));
        UInt128 result = 0;
        Operation[] operations = [Operation.Sum, Operation.Mul];
        foreach (var (testValue, numbers) in input)
        {
            if (CanBeProduced(testValue, numbers[0], numbers[1..], operations))
            {
                result += testValue;
            }
        }
        
        Assert.Equal((UInt128)303766880536, result);
    }
    
    [Fact]
    private void Second()
    {
        var input = ParseInput(File.ReadAllLines(InputFile));
        UInt128 result = 0;
        Operation[] operations = [Operation.Sum, Operation.Mul, Operation.Join];
        foreach (var (testValue, numbers) in input)
        {
            if (CanBeProduced(testValue, numbers[0], numbers[1..], operations))
            {
                result += testValue;
            }
        }
        
        Assert.Equal((UInt128)337041851384440, result);
    }

    private bool CanBeProduced(UInt128 target, UInt128 acc, ReadOnlySpan<UInt128> nums, Operation[] operations)
    {
        if(acc > target) return false;
        if (nums is []) return acc == target;

        if (nums is not [var head, .. var tail]) return false;
        
        foreach (var operation in operations)
        {
            if (CanBeProduced(target, ApplyOperation(acc, head, operation), tail, operations))
            {
                return true;
            }
        }

        return false;
    }

    private UInt128 ApplyOperation(UInt128 accumulator, UInt128 value, Operation operation) => operation switch
    {
        Operation.Sum => accumulator + value,
        Operation.Mul => accumulator * value,
        Operation.Join => UInt128.Parse($"{accumulator}{value}"),
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
    };
    
    private static IEnumerable<(UInt128 testValue, UInt128[] numbers)> ParseInput(string[] inputLines)
    {
        foreach (var line in inputLines)
        {
            var parts = line.Split(':');
            var testValue = UInt128.Parse(parts[0]);
            var numbers = parts[1]
                .Split(' ')
                .Where(x => UInt128.TryParse(x, out _))
                .Select(UInt128.Parse)
                .ToArray();
            
            yield return (testValue, numbers);
        }
    }
}