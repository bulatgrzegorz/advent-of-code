using System.Collections;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Text.RegularExpressions;
using Xunit;

namespace adventOfCode._2024.Day24;

public class Day24
{
    private const string ExampleInput = """
                                        x00: 1
                                        x01: 0
                                        x02: 1
                                        x03: 1
                                        x04: 0
                                        y00: 1
                                        y01: 1
                                        y02: 1
                                        y03: 1
                                        y04: 1
                                        
                                        ntg XOR fgs -> mjb
                                        y02 OR x01 -> tnw
                                        kwq OR kpj -> z05
                                        x00 OR x03 -> fst
                                        tgd XOR rvg -> z01
                                        vdt OR tnw -> bfw
                                        bfw AND frj -> z10
                                        ffh OR nrd -> bqk
                                        y00 AND y03 -> djm
                                        y03 OR y00 -> psh
                                        bqk OR frj -> z08
                                        tnw OR fst -> frj
                                        gnj AND tgd -> z11
                                        bfw XOR mjb -> z00
                                        x03 OR x00 -> vdt
                                        gnj AND wpb -> z02
                                        x04 AND y00 -> kjc
                                        djm OR pbm -> qhw
                                        nrd AND vdt -> hwm
                                        kjc AND fst -> rvg
                                        y04 OR y02 -> fgs
                                        y01 AND x02 -> pbm
                                        ntg OR kjc -> kwq
                                        psh XOR fgs -> tgd
                                        qhw XOR tgd -> z09
                                        pbm OR djm -> kpj
                                        x03 XOR y03 -> ffh
                                        x00 XOR y04 -> ntg
                                        bfw OR bqk -> z06
                                        nrd XOR fgs -> wpb
                                        frj XOR qhw -> z04
                                        bqk OR frj -> z07
                                        y03 OR x01 -> nrd
                                        hwm AND bqk -> z03
                                        tgd XOR rvg -> z12
                                        tnw OR pbm -> gnj
                                        """;

    private const string Example2 = """
                                    x00: 0
                                    x01: 1
                                    x02: 0
                                    x03: 1
                                    x04: 0
                                    x05: 1
                                    y00: 0
                                    y01: 0
                                    y02: 1
                                    y03: 1
                                    y04: 0
                                    y05: 1

                                    x00 AND y00 -> z05
                                    x01 AND y01 -> z02
                                    x02 AND y02 -> z01
                                    x03 AND y03 -> z03
                                    x04 AND y04 -> z04
                                    x05 AND y05 -> z00
                                    """;

    [Fact]
    public void Example()
    {
        // var lines = ExampleInput.Split(Environment.NewLine);
        var lines = InputHelper.GetInputLines();
        var wires = lines.TakeWhile(x => !string.IsNullOrEmpty(x)).ToArray();
        var gates = lines[(wires.Length + 1)..].ToArray();

        var w =
            wires.Select(x => x.Split(':')).ToDictionary(x => x[0], x => x[1].Trim() == "1");

        var a = ParseGates(gates).ToArray();
        var zets = a.Where(x => x.result.StartsWith('z')).Select(x => x.result).ToList();
        while (true)
        {
            foreach (var (left, right, result, op, _) in a.Where(x => x.toExecute))
            {
                if (!w.TryGetValue(left, out var l) || !w.TryGetValue(right, out var r)) continue;

                if (w.ContainsKey(result))
                {
                    w[result] = Execute(l, r, op);
                }
                else
                {
                    w.Add(result, Execute(l, r, op));
                }
            }

            if (zets.TrueForAll(x => w.ContainsKey(x)))
            {
                break;
            }
        }

        var zetsValues = w.Where(x => zets.Contains(x.Key)).OrderByDescending(x => x.Key)
            .Select(x => x.Value ? '1' : '0');

        Assert.Equal(2024, Convert.ToInt64(string.Join("", zetsValues), 2));
    }

    [Fact]
    public void Test()
    {
        // var x = 15;
        // var y = 13;
        // var xb = IntToBitArray(x).Reverse().ToArray();
        // var yb = IntToBitArray(y).Reverse().ToArray();
        // var xb = new bool[] { true, true, false, true };
        //
        // var s = Add(xb, yb);
        // var sr = s.Reverse().ToArray();
        // Console.WriteLine(string.Join("", s.Select(b => b ? '1' : '0')));
        // Console.WriteLine(string.Join("", sr.Select(b => b ? '1' : '0')));
        //
        // var ss = BoolArrayToInt(sr);
    }

    private record Operation(string Left, string Right, Op Op, string Result)
    {
        public string Result { get; set; } = Result;
        public bool EqualsS(Operation? other) => other switch
        {
            null => false,
            var (_, _, o, _) when o != Op => false,
            var (l, r, _, res) when !string.IsNullOrEmpty(res) => ((l, r) == (Left, Right) || (l, r) == (Right, Left)) && res == Result,
            var (l, r, _, _) => (l, r) == (Left, Right) || (l, r) == (Right, Left)
        };
    }
    
    [Fact]
    public void Second()
    {
        var result = new List<string>();
        
        var lines = InputHelper.GetInputLines();
        var wires = lines.TakeWhile(x => !string.IsNullOrEmpty(x)).ToArray();
        var gates = lines[(wires.Length + 1)..].ToArray();

        var ops = ParseOperations(gates).ToArray();
        var a = ParseGates(gates).ToArray();
        var z00 = a.Single(x => x.result.Equals("z00"));
        Debug.Assert(z00.op == Op.Xor);
        var index = 1;
        var cinTemplate = new Operation("x00", "y00", Op.And, string.Empty);
        var cin = ops.Single(x => x.EqualsS(cinTemplate));
        var cinS = cin.Result; 
        while (index < 45)
        {
            var xyXorTemplate = new Operation(GetX(index), GetY(index), Op.Xor, string.Empty);
            var xyAndTemplate = new Operation(GetX(index), GetY(index), Op.And, string.Empty);
            
            var xyXor = ops.Single(x => x.EqualsS(xyXorTemplate));
            var xyAnd = ops.Single(x => x.EqualsS(xyAndTemplate));
            
            var zTemplate = new Operation(xyXor.Result, cinS, Op.Xor, string.Empty);
            var zCandidate = ops.SingleOrDefault(x => x.EqualsS(zTemplate));
            if (zCandidate == null)
            {
                result.AddRange([xyXor.Result, xyAnd.Result]);
                (xyXor.Result, xyAnd.Result) = (xyAnd.Result, xyXor.Result);
                zTemplate = new Operation(xyXor.Result, cinS, Op.Xor, string.Empty);
                zCandidate = ops.SingleOrDefault(x => x.EqualsS(zTemplate));
            }
            
            if (zCandidate!.Result != GetZ(index))
            {
                var actualZ = ops.Single(x => x.Result == GetZ(index));
                result.AddRange([actualZ.Result, zCandidate.Result]);
                (actualZ.Result, zCandidate.Result) = (zCandidate.Result, actualZ.Result);
            }
            
            var andTemplate = new Operation(xyXor.Result, cinS, Op.And, string.Empty);
            
            
            // var z = ops.Single(x => x.Result == GetZ(index));
            // if (z is not { Op: Op.Xor } ||
            //     ((z.Left, z.Right) != (xyXor.Result, cinS) && 
            //      (z.Left, z.Right) != (cinS, xyXor.Result)))
            // {
            //     //swap
            //     var toSwap = ops.Single(x => x.EqualsS(new Operation(xyXor.Result, cinS, Op.Xor, string.Empty)));
            //     result.AddRange([z.Result, toSwap.Result]);
            //     (z.Result, toSwap.Result) = (toSwap.Result, z.Result);
            // }
                        
            var and = ops.Single(x => x.EqualsS(andTemplate));
            var coutTemplate = new Operation(and.Result, xyAnd.Result, Op.Or, string.Empty);
            var cout = ops.Single(x => x.EqualsS(coutTemplate));
            cinS = cout.Result;
            index++;
        }

        Assert.Equal("css,cwt,gdd,jmv,pqt,z05,z09,z37", string.Join(",", result.OrderBy(x => x)));
    }

    private static string GetX(int index) => $"x{index:00}";
    private static string GetY(int index) => $"y{index:00}";
    private static string GetZ(int index) => $"z{index:00}";
    public static int BoolArrayToInt(bool[] boolArray)
    {
        int result = 0;

        // Iterate through the bool array and set the corresponding bits in the result
        for (int i = 0; i < boolArray.Length; i++)
        {
            if (boolArray[i])  // If the current bit is 'true' (1)
            {
                result |= (1 << (boolArray.Length - 1 - i));  // Set the corresponding bit in the result
            }
        }

        return result;
    }

    private static bool[] IntToBitArray(int value)
    {
        // List to hold the bits
        List<bool> bitList = [];

        // Iterate over each bit of the integer
        for (var i = 31; i >= 0; i--)
        {
            // Check if the bit at position i is 1 or 0
            var bit = (value & (1 << i)) != 0;
            bitList.Add(bit);
        }

        // Remove leading zeros (optional, if you don't want a full 32-bit array)
        var leadingZeroIndex = bitList.FindIndex(b => b);
        return leadingZeroIndex != -1 ? bitList.GetRange(leadingZeroIndex, bitList.Count - leadingZeroIndex).ToArray() :
            // If there are no leading ones (in case of 0), return an empty array
            [];
    }
    
    
    
    private static (bool cout, bool s) HalfAdder(bool a, bool b) => (a & b, a ^ b);

    private static (bool cout, bool s) FullAdder(bool a, bool b, bool cin)
    {
        var (firstHaCout, firstHaS) = HalfAdder(a, b);
        var (secondHaCount, secondHaS) = HalfAdder(firstHaS, cin);
        
        return (firstHaCout | secondHaCount, secondHaS);
    }

    private static bool[] Add((bool a, bool b)[] bits)
    {
        var result = new bool[bits.Length + 1];
        var cin = false;
        for (var i = 0; i < bits.Length; i++)
        {
            var (a, b) = bits[i];
            (cin, result[i]) = FullAdder(a, b, cin);
        }
        
        result[^1] = cin;

        return result;
    }
    
    private static bool[] Add(bool[] x, bool[] y)
    {
        Debug.Assert(x.Length == y.Length);
        
        var result = new bool[x.Length + 1];
        var cin = false;
        for (var i = 0; i < x.Length; i++)
        {
            (cin, result[i]) = FullAdder(x[i], y[i], cin);
        }
        
        result[^1] = cin;

        return result;
    }

    private bool Execute(bool l, bool r, Op op) => op switch
    {
        Op.Or => l | r,
        Op.And => l & r,
        Op.Xor => l ^ r,
        _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
    };
    private enum Op {Or, And, Xor }

    private IEnumerable<(string left, string right, string result, Op op, bool toExecute)> ParseGates(string[] lines)
    {
        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"([\w0-9]+) (\w+) ([\w0-9]+) -> ([\w0-9]+)");
            var op = match.Groups[2].Value switch
            {
                "OR" => Op.Or,
                "AND" => Op.And,
                "XOR" => Op.Xor,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            yield return (match.Groups[1].Value, match.Groups[3].Value, match.Groups[4].Value, op, true);
        }
    }
    
    private IEnumerable<Operation> ParseOperations(string[] lines)
    {
        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"([\w0-9]+) (\w+) ([\w0-9]+) -> ([\w0-9]+)");
            var op = match.Groups[2].Value switch
            {
                "OR" => Op.Or,
                "AND" => Op.And,
                "XOR" => Op.Xor,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            yield return new Operation(match.Groups[1].Value, match.Groups[3].Value, op, match.Groups[4].Value);
        }
    }
}