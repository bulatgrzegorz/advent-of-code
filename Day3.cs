using Xunit;

namespace adventOfCode2024;

public class Day3
{
    private const string ExampleInput = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
    private const string SecondExampleInput = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
    private const string MultiplyMarker = "mul(";
    private const string DoMarker = "do()";
    private const string DontMarker = "don't()";
    private const string InputFile = "Day3.txt";

    [Fact]
    public void Example()
    {
        var sum = CalculateSumOfMultiplies(ExampleInput);

        Assert.Equal(161, sum);
    }
    
    [Fact]
    public void First()
    {
        var sum = CalculateSumOfMultiplies(File.ReadAllText(InputFile).AsSpan());

        Assert.Equal(188741603, sum);
    }
    
    [Fact]
    public void SecondExample()
    {
        var sum = CalculateSumOfMultiplies2(SecondExampleInput);

        Assert.Equal(48, sum);
    }
    
    [Fact]
    public void Second()
    {
        var sum = CalculateSumOfMultiplies2(File.ReadAllText(InputFile));

        Assert.Equal(67269798, sum);
    }

    private static int CalculateSumOfMultiplies(ReadOnlySpan<char> input)
    {
        var sum = 0;
        var currentIndex = 0;
        while (true)
        {
            currentIndex = input.IndexOf(MultiplyMarker, currentIndex);
            if(currentIndex < 0) break;
            
            currentIndex += MultiplyMarker.Length; 

            var firstNumber = GetNumber(input, ref currentIndex);
            if(firstNumber is null) continue;
            
            var maybeComma = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeComma is not ',') continue;
            currentIndex++;
            
            var secondNumber = GetNumber(input, ref currentIndex);
            if(secondNumber is null) continue;
            
            var maybeClosedBracket = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeClosedBracket is not ')') continue;
            currentIndex++;
            
            sum += firstNumber.Value * secondNumber.Value;
        }

        return sum;
    }
    
    private static int CalculateSumOfMultiplies2(ReadOnlySpan<char> input)
    {;
        var sum = 0;
        var currentIndex = 0;
        var currentDontIndex = -1;
        var currentDoIndex = 0;
        while (true)
        {
            var currentMultiplyIndex = input.IndexOf(MultiplyMarker, currentIndex);
            if(currentMultiplyIndex < 0) break;
            
            var canProcessMultiply = CanProcessMultiply(input, currentMultiplyIndex, ref currentDontIndex, ref currentDoIndex);
            currentIndex = currentMultiplyIndex + MultiplyMarker.Length;
            if(!canProcessMultiply) continue;

            var firstNumber = GetNumber(input, ref currentIndex);
            if(firstNumber is null) continue;
            
            var maybeComma = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeComma is not ',') continue;
            currentIndex++;
            
            var secondNumber = GetNumber(input, ref currentIndex);
            if(secondNumber is null) continue;
            
            var maybeClosedBracket = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeClosedBracket is not ')') continue;
            currentIndex++;
            
            sum += firstNumber.Value * secondNumber.Value;
        }

        return sum;
    }
    
    private static bool CanProcessMultiply(ReadOnlySpan<char> input, int currentMultiplyIndex, ref int currentDontIndex, ref int currentDoIndex)
    {
        //------don't-----do-------mul--------
        //------v---------v--------v-------
        //------*---------*--------*-------
        //we are looking for don'ts and do's between current multiply and last occurrence. Then we check which one was last 
        var dontIndex = input.LastIndexOf(DontMarker, currentMultiplyIndex, currentMultiplyIndex - (currentDontIndex is -1 ? 0 : currentDontIndex));
        if(dontIndex > 0) currentDontIndex = dontIndex;
        
        if(currentDontIndex < 0 || currentDontIndex > currentMultiplyIndex) return true;
        
        var doIndex = input.LastIndexOf(DoMarker, currentMultiplyIndex, currentMultiplyIndex - currentDoIndex);
        if(doIndex > 0) currentDoIndex = doIndex;
        
        return currentDoIndex >= currentDontIndex;
    }

    private static int? GetNumber(ReadOnlySpan<char> input, ref int currentIndex)
    {
        var startingIndex = currentIndex;
        while (true)
        {
            var nextChar = GetCharFromStringAndProgress(input, currentIndex);
            if (!nextChar.HasValue || !char.IsDigit(nextChar.Value)) break;

            currentIndex++;
        }

        return currentIndex > startingIndex && int.TryParse(input[startingIndex..currentIndex], out var result) ? result : null;
    }

    private static char? GetCharFromStringAndProgress(ReadOnlySpan<char> input, int position)
    {
        if (position < 0 || position >= input.Length) return null;
        
        return input[position];
    }
}