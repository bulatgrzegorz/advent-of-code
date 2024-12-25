using System.Text.RegularExpressions;
using Xunit;

namespace adventOfCode._2024.Day24;

public class Day24
{
    [Fact]
    public void First()
    {
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

        Assert.Equal(61495910098126, Convert.ToInt64(string.Join("", zetsValues), 2));
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

        var cin = GetOpRequired(ops, "x00", "y00", Op.And).Result;
        for (var index = 1; index < 45; index++)
        {
            var xn = $"x{index:00}";
            var yn = $"y{index:00}";
            var zn = $"z{index:00}";
            
            var xyXor = GetOpRequired(ops, xn, yn, Op.Xor);
            var xyAnd = GetOpRequired(ops, xn, yn, Op.And);
            
            var z = GetOp(ops, xyXor.Result, cin, Op.Xor);
            if (z is null)
            {
                result.AddRange([xyXor.Result, xyAnd.Result]);
                (xyXor.Result, xyAnd.Result) = (xyAnd.Result, xyXor.Result);
                z = GetOpRequired(ops, xyXor.Result, cin, Op.Xor);
            }
            
            if (z.Result != zn)
            {
                var actualZ = ops.Single(x => x.Result == zn);
                result.AddRange([actualZ.Result, z.Result]);
                (actualZ.Result, z.Result) = (z.Result, actualZ.Result);
            }

            var and = GetOpRequired(ops, xyXor.Result, cin, Op.And);
            
            cin = GetOpRequired(ops, and.Result, xyAnd.Result, Op.Or).Result;
        }

        Assert.Equal("css,cwt,gdd,jmv,pqt,z05,z09,z37", string.Join(",", result.OrderBy(x => x)));
    }

    private static Operation GetOpRequired(Operation[] operations, string l, string r, Op op) => GetOp(operations, l, r, op)!;
    private static Operation? GetOp(Operation[] operations, string l, string r, Op op)
    {
        var template = new Operation(l, r, op, string.Empty);
        return operations.SingleOrDefault(x => x.EqualsS(template));
    }

    private static bool Execute(bool l, bool r, Op op) => op switch
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