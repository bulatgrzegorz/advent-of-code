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
        var sum = CalculateSumOfMultiplies(File.ReadAllText(InputFile));

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

    private static int CalculateSumOfMultiplies(string input)
    {
        Span<char> firstNumberBuffer = new char[3];
        Span<char> secondNumberBuffer = new char[3];
        var sum = 0;
        var currentIndex = 0;
        while (true)
        {
            firstNumberBuffer.Clear();
            secondNumberBuffer.Clear();
            
            currentIndex = input.IndexOf(MultiplyMarker, currentIndex, StringComparison.Ordinal);
            if(currentIndex < 0) break;
            
            currentIndex += MultiplyMarker.Length; 

            var firstNumber = GetNumber(input, ref currentIndex, firstNumberBuffer);
            if(firstNumber is null) continue;
            
            var maybeComma = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeComma is not ',') continue;
            currentIndex++;
            
            var secondNumber = GetNumber(input, ref currentIndex, secondNumberBuffer);
            if(secondNumber is null) continue;
            
            var maybeClosedBracket = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeClosedBracket is not ')') continue;
            currentIndex++;
            
            sum += firstNumber.Value * secondNumber.Value;
        }

        return sum;
    }
    
    private static int CalculateSumOfMultiplies2(string input)
    {
        Span<char> firstNumberBuffer = new char[3];
        Span<char> secondNumberBuffer = new char[3];
        var sum = 0;
        var currentIndex = 0;
        var currentDontIndex = -1;
        var currentDoIndex = 0;
        while (true)
        {
            firstNumberBuffer.Clear();
            secondNumberBuffer.Clear();
            
            var currentMultiplyIndex = input.IndexOf(MultiplyMarker, currentIndex, StringComparison.Ordinal);
            if(currentMultiplyIndex < 0) break;
            
            var canProcessMultiply = CanProcessMultiply(input, currentMultiplyIndex, ref currentDontIndex, ref currentDoIndex);
            currentIndex = currentMultiplyIndex + MultiplyMarker.Length;
            if(!canProcessMultiply) continue;

            var firstNumber = GetNumber(input, ref currentIndex, firstNumberBuffer);
            if(firstNumber is null) continue;
            
            var maybeComma = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeComma is not ',') continue;
            currentIndex++;
            
            var secondNumber = GetNumber(input, ref currentIndex, secondNumberBuffer);
            if(secondNumber is null) continue;
            
            var maybeClosedBracket = GetCharFromStringAndProgress(input, currentIndex);
            if(maybeClosedBracket is not ')') continue;
            currentIndex++;
            
            sum += firstNumber.Value * secondNumber.Value;
        }

        return sum;
    }
    
    private static bool CanProcessMultiply(string input, int currentMultiplyIndex, ref int currentDontIndex, ref int currentDoIndex)
    {
        var dontIndex = input.LastIndexOf(DontMarker, currentMultiplyIndex, currentMultiplyIndex - (currentDontIndex is -1 ? 0 : currentDontIndex), StringComparison.Ordinal);
        if(dontIndex > 0) currentDontIndex = dontIndex;
        
        if(currentDontIndex < 0 || currentDontIndex > currentMultiplyIndex) return true;
        //------don't----------------------
        //------v--------------------------
        //------*----------m---------------
        
        //we have some don't in between
        
        var doIndex = input.LastIndexOf(DoMarker, currentMultiplyIndex, currentMultiplyIndex - currentDoIndex, StringComparison.Ordinal);
        if(doIndex > 0) currentDoIndex = doIndex;
        
        return currentDoIndex >= currentDontIndex;
    }

    private static int? GetNumber(string input, ref int currentIndex, Span<char> firstNumberBuffer)
    {
        var numberIndex = 0;
        while (true)
        {
            var nextChar = GetCharFromStringAndProgress(input, currentIndex);
            if (!nextChar.HasValue || !char.IsDigit(nextChar.Value)) break;

            currentIndex++;
            firstNumberBuffer[numberIndex] = nextChar.Value;
            numberIndex++;
        }

        return int.TryParse(firstNumberBuffer, out var result) ? result : null;
    }

    private static char? GetCharFromStringAndProgress(string input, int position)
    {
        if (position < 0 || position >= input.Length) return null;
        
        return input[position];
    }
}