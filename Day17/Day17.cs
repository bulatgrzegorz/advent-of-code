using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Xunit;

namespace adventOfCode.Day17;

public class Day17
{
    private const string FileInput = "Day17/Day17.input";

    private IEnumerable<long> Step(int expected, long value)
    {
        for (var i = 0; i < 8; i++)
        {
            var toCalculate  = value + i;
            var b = toCalculate % 8;
            b ^= 1;
            var c = toCalculate >> (int)b;
            b ^= c;
            b ^= 4;

            if (b % 8 == expected) yield return toCalculate;
        }
    }
    
    [Fact]
    public void Part2()
    {
        var exampleLines = File.ReadAllLines(FileInput);
        
        var program = Regex.Matches(exampleLines[4], @"(\d+)").Select(x => int.Parse(x.Value)).ToArray();
        
        var programReverse = program.Reverse().ToArray();
        
        var result = new HashSet<long>();
        Rev(programReverse, [0], result);

        Assert.Equal(202972175280682, result.Min());
    }

    void Rev(ReadOnlySpan<int> program, IEnumerable<long> stepCandidates, HashSet<long> results)
    {
        if (program is [])
        {
            results.UnionWith(stepCandidates);
            return;
        }

        foreach (var candidate in stepCandidates)
        {
            Rev(program[1..], Step(program[0], candidate << 3), results);
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private enum OpCode
    {
        adv,
        bxl,
        bst,
        jnz,
        bxc,
        @out,
        bdv,
        cdv
    }

    private enum OperandType{Literal, RegA, RegB, RegC, Invalid}

    private static OperandType ComboOperand(int operand) => operand switch
    {
        >= 0 and <= 3 => OperandType.Literal,
        4 => OperandType.RegA,
        5 => OperandType.RegB,
        6 => OperandType.RegC,
        7 => OperandType.Invalid,
        _ => throw new ArgumentOutOfRangeException(nameof(operand), operand, null)
    };

    private static int OperandValue(int regA, int regB, int regC, OperandType operandType, int operand) =>
        operandType switch
        {
            OperandType.Literal => operand,
            OperandType.RegA => regA,
            OperandType.RegB => regB,
            OperandType.RegC => regC,
            _ => throw new ArgumentOutOfRangeException(nameof(operandType), operandType, null)
        };

    private readonly record struct Program(int[] Values)
    {
        public (OpCode, OperandType, int operand) GetInstruction(int index)
        {
            return ((OpCode)Values[index], ComboOperand(Values[index + 1]), Values[index + 1]);
        }
    }
    
    [Fact]
    public void Part1()
    {
        var exampleLines = File.ReadAllLines(FileInput);

        var registerA = int.Parse(Regex.Match(exampleLines[0], @"\d+").Value);
        var registerB = int.Parse(Regex.Match(exampleLines[1], @"\d+").Value);
        var registerC = int.Parse(Regex.Match(exampleLines[2], @"\d+").Value);

        var program = new Program(Regex.Matches(exampleLines[4], @"(\d+)").Select(x => int.Parse(x.Value)).ToArray());

        for (var i = 0; i < program.Values.Length; i += 2)
        {
            var (opCode, operandType, operand) = program.GetInstruction(i);
            ExecuteInstruction(ref i, ref registerA, ref registerB, ref registerC, opCode, operandType, operand);
        }
    }

    private static void ExecuteInstruction(ref int i, ref int registerA, ref int registerB, ref int registerC, OpCode opCode, OperandType operandType, int operand)
    {
        var operandValue = OperandValue(registerA, registerB, registerC, operandType, operand);
        if (opCode is OpCode.adv)
        {
            registerA >>= operandValue;
        }
        else if (opCode is OpCode.bdv)
        {
            registerB = registerA >> operandValue;
        }
        else if (opCode is OpCode.cdv)
        {
            registerC = registerA >> operandValue;
        }
        else if (opCode is OpCode.bxl)
        {
            registerB ^= operand;
        }
        else if (opCode is OpCode.bst)
        {
            registerB = operandValue % 8;
        }
        else if (opCode is OpCode.jnz)
        {
            if(registerA is 0) return;
            i = operand - 2;
        }
        else if (opCode is OpCode.bxc)
        {
            registerB ^= registerC;
        }
        else if (opCode is OpCode.@out)
        {
            Console.WriteLine($"{operandValue % 8},");
        }
    }
}