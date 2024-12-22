using Xunit;

namespace adventOfCode._2024.Day21;

public class Day21
{
    private static readonly Dictionary<char, Dictionary<char, string[]>> OptimalDirectionalPaths = new()
    {
        ['A'] = new()
        {
            ['A'] = [],
            ['^'] = ["<"],
            ['>'] = ["v"],
            ['v'] = ["<v", "v<"],
            ['<'] = ["v<<", "<v<"],
        },
        ['^'] = new()
        {
            ['A'] = [">"],
            ['^'] = [""],
            ['>'] = ["v>", ">v"],
            ['v'] = ["v"],
            ['<'] = ["v<"],
        },
        ['v'] = new()
        {
            ['A'] = ["^>", ">^"],
            ['^'] = ["^"],
            ['>'] = [">"],
            ['v'] = [""],
            ['<'] = ["<"],
        },
        ['<'] = new()
        {
            ['A'] = [">>^", ">^>"],
            ['^'] = [">^"],
            ['>'] = [">>"],
            ['v'] = [">"],
            ['<'] = [""],
        },
        ['>'] = new()
        {
            ['A'] = ["^"],
            ['^'] = ["^<", "<^"],
            ['>'] = [""],
            ['v'] = ["<"],
            ['<'] = ["<<"],
        },
    };
    
    private readonly record struct Point(int Row, int Col)
    {
        public static IEnumerable<Move[]> GetMoves(Point start, Point end)
        {
            var diffVector = end - start;

            if (start.Row == 3 && end.Col == 0)
            {
                yield return 
                [
                    ..Enumerable.Repeat(diffVector.Row > 0 ? Move.Down : Move.Up, int.Abs(diffVector.Row)),
                    ..Enumerable.Repeat(diffVector.Col > 0 ? Move.Right : Move.Left, int.Abs(diffVector.Col))
                ];
                yield break;
            }

            if (start.Col == 0 && end.Row == 3)
            {
                yield return 
                [
                    ..Enumerable.Repeat(diffVector.Col > 0 ? Move.Right : Move.Left, int.Abs(diffVector.Col)),
                    ..Enumerable.Repeat(diffVector.Row > 0 ? Move.Down : Move.Up, int.Abs(diffVector.Row))
                ];
                yield break;
            }
            
            yield return 
            [
                ..Enumerable.Repeat(diffVector.Row > 0 ? Move.Down : Move.Up, int.Abs(diffVector.Row)),
                ..Enumerable.Repeat(diffVector.Col > 0 ? Move.Right : Move.Left, int.Abs(diffVector.Col))
            ];
                
            yield return 
            [
                ..Enumerable.Repeat(diffVector.Col > 0 ? Move.Right : Move.Left, int.Abs(diffVector.Col)),
                ..Enumerable.Repeat(diffVector.Row > 0 ? Move.Down : Move.Up, int.Abs(diffVector.Row))
            ];
        }
        
        public static Point operator -(Point a, Point b) => new(a.Row - b.Row, a.Col - b.Col);
    }

    private enum Move { Up, Left, Down, Right }

    private static readonly Dictionary<int, char> MoveToChar = new()
    {
        [(int)Move.Up] = '^',
        [(int)Move.Right] = '>',
        [(int)Move.Down] = 'v',
        [(int)Move.Left] = '<',
        [4] = 'A'
    };

    private readonly Point[] Numerics =
    [
        new(3, 1), 
        new(2, 0), new(2, 1), new(2, 2), 
        new(1, 0), new(1, 1), new(1, 2), 
        new(0, 0), new(0, 1), new(0, 2),
        new(3, 2)
    ];

    [Fact]
    private void First()
    {
        var example = """
                      029A
                      980A
                      179A
                      456A
                      379A
                      """;

        var res = InputHelper.GetInputLines().Select(NewMethod).Sum();
        
        // var res = NewMethod("456A");
        // var res = example.Split(Environment.NewLine).Select(NewMethod).Sum();
        // var res = NewMethod(example);
        
        Assert.Equal(136780, res);
    }

    private int NewMethod(string example)
    {
        var previousNumeric = 10; //A
        var previousDirectionalFirst = 4;  //A
        var previousDirectionalSecond = 4;  //A
        var previous = 'A';
        // var previous2 = 'A';

        var resultF = 0;
        var kk = 0;
        for (int i = 0; i < example.Length; i++)
        {
            var numeric = example[i] is 'A' ? 10 : example[i] - '0';
            
            // Console.WriteLine($"Numeric: {numeric}, previous: {previousNumeric}");
            var k = 0;
            var smallestYet = int.MaxValue;
            foreach (var numericPathCandidate in Point.GetMoves(Numerics[previousNumeric], Numerics[numeric]))
            {
                var result = 0;
                // Console.WriteLine($"    Numeric candidate {k}");
                Move[] numericMoves = [..numericPathCandidate, (Move)4];

                
                foreach (var numericMove in numericMoves.Select(x => MoveToChar[(int)x]))
                {
                    // Console.WriteLine($"         Numeric move: {numericMove}, previous: {previous}");
                    var possiblePaths = OptimalDirectionalPaths[previous][numericMove];

                    
                    var smallestYet2 = int.MaxValue;
                    
                    foreach (var (i1, possiblePath) in possiblePaths.Index())
                    {
                        // Console.WriteLine($"            Directional path [{i1}]: {possiblePath}");
                        //
                        // Console.WriteLine($"kk: {kk++}");
                        var step = Step(possiblePath);
                        
                        if (step < smallestYet2)
                        {
                            smallestYet2 = step;
                        }
                    }

                    // previous2 = smallestYet2;

                    // Console.WriteLine($"            Directional path: final length: {smallestYet2}. Finished on: {previous2}, path: {smallestYet2}]");
                    result += smallestYet2;
                    previous = numericMove;
                }
                
                if (result < smallestYet)
                {
                    smallestYet = result;
                }
            }
            
            resultF += smallestYet;
            
            previousNumeric = numeric;
        }

        Console.WriteLine(resultF);
        
        Console.WriteLine(string.Join("", example));

        return int.Parse(example.Where(char.IsDigit).ToArray()) * resultF;
    }

    private static int Step(string possiblePath)
    {
        var result = 0;
        var previous = 'A';
        foreach (var move in (char[])[..possiblePath, 'A'])
        {
            var paths = OptimalDirectionalPaths[previous][move];
            if (paths is not [])
            {
                var shortest = paths.OrderBy(x => x.Length).First();
                
                result += shortest.Length + 1;
            }
            else
            {
                result += 1;
            }

            previous = move;
        }
        
        return result;
    }
}