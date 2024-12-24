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
}