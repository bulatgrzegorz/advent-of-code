using Xunit;

namespace adventOfCode._2024.Day21;

public class Day21
{
    //Known optimal paths between directional buttons. Some are omitted, as we know that others are faster any time (like A -> <, we are not using <v<)
    private static readonly Dictionary<char, Dictionary<char, string[]>> OptimalDirectionalPaths = new()
    {
        ['A'] = new()
        {
            ['A'] = [],
            ['^'] = ["<"],
            ['>'] = ["v"],
            ['v'] = ["<v", "v<"],
            ['<'] = ["v<<"],
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
            ['A'] = [">>^"],
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
        //Path between buttons on numeric keyboard
        public static IEnumerable<Move[]> GetMoves(Point start, Point end)
        {
            var diffVector = end - start;

            //if we might cross empty space on 3,0 - we just ignore this road and take moves up at first
            if (start.Row == 3 && end.Col == 0)
            {
                yield return 
                [
                    ..Enumerable.Repeat(diffVector.Row > 0 ? Move.Down : Move.Up, int.Abs(diffVector.Row)),
                    ..Enumerable.Repeat(diffVector.Col > 0 ? Move.Right : Move.Left, int.Abs(diffVector.Col))
                ];
                yield break;
            }

            //opposite, if we might cross empty space, but going in reverse, we will first take moves right
            if (start.Col == 0 && end.Row == 3)
            {
                yield return 
                [
                    ..Enumerable.Repeat(diffVector.Col > 0 ? Move.Right : Move.Left, int.Abs(diffVector.Col)),
                    ..Enumerable.Repeat(diffVector.Row > 0 ? Move.Down : Move.Up, int.Abs(diffVector.Row))
                ];
                yield break;
            }
            
            //otherwise we will test both paths
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

    private readonly Point[] _numerics =
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
        var res = InputHelper.GetInputLines().Select(x => NewMethod(x, 2)).Sum();
     
        Assert.Equal(136780, res);
    }
    
    [Fact]
    private void Second()
    {
        var res = InputHelper.GetInputLines().Select(x => NewMethod(x, 25)).Sum();
     
        Assert.Equal(167538833832712, res);
    }

    private long NewMethod(string input, int robotsDepth)
    {
        var result = 0L;
        
        var previousNumeric = 10; //A
        foreach (var inputChar in input)
        {
            var numeric = inputChar is 'A' ? 10 : inputChar - '0';

            var shortestRobotsMovePath = long.MaxValue;
            foreach (var numericPathCandidate in Point.GetMoves(_numerics[previousNumeric], _numerics[numeric]))
            {
                var robotsCandidatePathResult = 0L;
                Move[] numericMoves = [..numericPathCandidate, (Move)4];
                
                var robotsPreviousMove = 'A';
                foreach (var numericMove in numericMoves.Select(x => MoveToChar[(int)x]))
                {
                    robotsCandidatePathResult += Step(numericMove, 0, robotsDepth - 1, robotsPreviousMove, []);
                    
                    robotsPreviousMove = numericMove;
                }
                
                if (robotsCandidatePathResult < shortestRobotsMovePath)
                {
                    shortestRobotsMovePath = robotsCandidatePathResult;
                }
            }
            
            result += shortestRobotsMovePath;
            
            previousNumeric = numeric;
        }

        Console.WriteLine(result);
        
        Console.WriteLine(string.Join("", input));

        return long.Parse(input.Where(char.IsDigit).ToArray()) * result;
    }

    private static long Step(char move, int level, int maxLevel, char previous, Dictionary<(char, int, char), long> cache)
    {
        long result = 0;
        var possiblePaths = OptimalDirectionalPaths[previous][move];
        if (level < maxLevel)
        {
            if (possiblePaths.Length is 0) return 1;

            var stepMin = long.MaxValue;
            foreach (var possiblePath in possiblePaths)
            {
                char[] possiblePathChars = [..possiblePath, 'A'];
                var stepPrevious = 'A';
                var stepResult = 0L;
                foreach (var c in possiblePathChars)
                {
                    if (!cache.TryGetValue((c, level + 1, stepPrevious), out var cachedResult))
                    {
                        var r = Step(c, level + 1, maxLevel, stepPrevious, cache);
                        cache.Add((c, level + 1, stepPrevious), r);
                        stepResult += r;
                    }
                    else
                    {
                        stepResult += cachedResult;
                    }

                    stepPrevious = c;
                }

                if (stepResult < stepMin) stepMin = stepResult;
            }

            return stepMin;
        }

        if (possiblePaths is not [])
        {
            //we are just taking any at this stage, we know in fact that even when multiple - all will be same lenght  
            var shortest = possiblePaths[0];

            result += shortest.Length + 1;
        }
        else
        {
            result += 1;
        }

        return result;
    }
}