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
            if (CanBeProduced(numbers, testValue, operations))
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
            if (CanBeProduced(numbers, testValue, operations))
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
            if (CanBeProduced(numbers, testValue, operations))
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
            if (CanBeProduced(numbers, testValue, operations))
            {
                result += testValue;
            }
        }
        
        Assert.Equal((UInt128)337041851384440, result);
    }

    private static bool CanBeProduced(UInt128[] numbers, UInt128 testValue, Operation[] operations)
    {
        foreach (var permutation in GenerateAllPermutations(operations, numbers.Length - 1))
        {
            if (CanBeProducedFromPermutation(numbers, testValue, permutation))
            {
                return true;
            }
        }

        return false;
    }

    private static bool CanBeProducedFromPermutation(UInt128[] numbers, UInt128 testValue, Operation[] permutation)
    {
        var accumulator = numbers[0];
        for (var i = 1; i < numbers.Length; i++)
        {
            try
            {
                accumulator = permutation[i - 1] switch
                {
                    Operation.Sum => accumulator + numbers[i],
                    Operation.Mul => accumulator * numbers[i],
                    Operation.Join => UInt128.Parse($"{accumulator}{numbers[i]}"),
                    _ => throw new ArgumentOutOfRangeException()
                };
            
                if(accumulator > testValue)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
        
        return accumulator == testValue;
    }

    static IEnumerable<T[]> GenerateAllPermutations<T>(T[] numbers, int length)
    {
        var currentPermutation = new T[length];
        
        // Recursive backtracking to generate all permutations
        return Backtrack(numbers, currentPermutation, 0);
    }

    private static IEnumerable<T[]> Backtrack<T>(T[] numbers, T[] currentPermutation, int position)
    {
        if (position == currentPermutation.Length)
        {
            // When the current permutation is complete, add it to the result list
            yield return [..currentPermutation];
            yield break;
        }

        foreach (var number in numbers)
        {
            // Assign the number at index `i` to the current position
            currentPermutation[position] = number;
            foreach (var result in Backtrack(numbers, currentPermutation, position + 1))
            {
                yield return result;
            }
        }
    }

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