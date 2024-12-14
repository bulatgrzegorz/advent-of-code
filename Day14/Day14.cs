using System.Text.RegularExpressions;
using Xunit;

namespace adventOfCode.Day14;

public class Day14
{
    private const string InputFile = "Day14/Day14.input";
    private const string InputExample = """
                                        p=0,4 v=3,-3
                                        p=6,3 v=-1,-3
                                        p=10,3 v=-1,2
                                        p=2,0 v=2,-1
                                        p=0,0 v=1,3
                                        p=3,0 v=-2,-2
                                        p=7,6 v=-1,-3
                                        p=3,0 v=-1,-2
                                        p=9,3 v=2,3
                                        p=7,3 v=-1,2
                                        p=2,4 v=2,-3
                                        p=9,5 v=-3,-3
                                        """;

    [Fact]
    public void Example()
    {
        var wide = 11;
        var tall = 7;
        var seconds = 100;
        
        var robots = ParseRobots(InputExample).ToArray();
        for (var i = 0; i < robots.Length; i++)
        {
            var robot = robots[i];
            var newY = (robot.Y + robot.YVel * seconds) % tall;
            var newX = (robot.X + robot.XVel * seconds) % wide;
            
            newY = newY < 0 ? tall + newY : newY;
            newX = newX < 0 ? wide + newX : newX;
            robots[i] = robot with { X = newX, Y = newY };
        }
        
        var result = CountGroups(wide, tall, robots);

        Assert.Equal(12, result);
    }

    [Fact]
    public void First()
    {
        var wide = 101;
        var tall = 103;
        var seconds = 100;
        
        var robots = ParseRobots(File.ReadAllText(InputFile)).ToArray();
        for (var i = 0; i < robots.Length; i++)
        {
            var robot = robots[i];
            var newY = (robot.Y + robot.YVel * seconds) % tall;
            var newX = (robot.X + robot.XVel * seconds) % wide;
            
            newY = newY < 0 ? tall + newY : newY;
            newX = newX < 0 ? wide + newX : newX;
            robots[i] = robot with { X = newX, Y = newY };
        }
        
        var result = CountGroups(wide, tall, robots);

        Assert.Equal(216772608, result);
    }
    
    [Fact]
    public void Second()
    {
        var wide = 101;
        var tall = 103;
        
        var robots = ParseRobots(File.ReadAllText(InputFile)).ToArray();
        var generation = 1;
        const int varianceThreshold = 1000;
        while (true)
        {
            for (var i = 0; i < robots.Length; i++)
            {
                var robot = robots[i];
                var newY = (robot.Y + robot.YVel) % tall;
                var newX = (robot.X + robot.XVel) % wide;
            
                newY = newY < 0 ? tall + newY : newY;
                newX = newX < 0 ? wide + newX : newX;
                robots[i] = robot with { X = newX, Y = newY };
            }

            if (CalculateTotalVariance(robots) < varianceThreshold)
            {
                Assert.Equal(6888, generation);
                // Console.WriteLine($"Generation: {generation}");
                // PrintRobotsMap(wide, tall, robots);
                return;
            }

            generation++;
        }
    }
    
    private static double CalculateTotalVariance(Robot[] points)
    {
        var meanX = points.Average(p => p.X);
        var meanY = points.Average(p => p.Y);
        
        var sumOfSquaredDistances = points.Sum(p => Math.Pow(p.X - meanX, 2) + Math.Pow(p.Y - meanY, 2));
        return sumOfSquaredDistances / points.Length;
    }
    
    private int CountGroups(int wide, int tall, Robot[] robots)
    {
        var leftTop = 0;
        var leftBottom = 0;
        var rightTop = 0;
        var rightBottom = 0;
        
        var (wideMiddle, tallMiddle) = ((wide - 1)/2, (tall - 1)/2);
        for (var i = 0; i < tall; i++)
        {
            if(tallMiddle == i) continue;
            for (var j = 0; j < wide; j++)
            {
                if (wideMiddle == j) continue;
                var robotsIn = robots.Count(x => x.X == j && x.Y == i);
                if (robotsIn == 0) continue;
                
                _ = (j, i) switch
                {
                    _ when j < wideMiddle && i < tallMiddle => leftTop += robotsIn,
                    _ when j < wideMiddle && i > tallMiddle => leftBottom += robotsIn,
                    _ when j > wideMiddle && i < tallMiddle => rightTop += robotsIn,
                    _ when j > wideMiddle && i > tallMiddle => rightBottom += robotsIn,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        
        return leftBottom * leftTop * rightBottom * rightTop;
    }
    
    private static void PrintRobotsMap(int wide, int tall, Robot[] robots)
    {
        for (var i = 0; i < tall; i++)
        {
            for (var j = 0; j < wide; j++)
            {
                var robotsIn = robots.Any(x => x.X == j && x.Y == i);

                Console.Write(robotsIn ? '*' : '.');
            }
            
            Console.WriteLine();
        }
    }
    
    private readonly record struct Robot(int X, int Y, int XVel, int YVel);
    private static IEnumerable<Robot> ParseRobots(string input)
    {
        foreach (Match match in Regex.Matches(input, @"p=(\d+),(\d+) v=(-?\d+),(-?\d+)"))
        {
            yield return new Robot(
                ConvertGroupIntoInt(match.Groups[1]), 
                ConvertGroupIntoInt(match.Groups[2]), 
                ConvertGroupIntoInt(match.Groups[3]),
                ConvertGroupIntoInt(match.Groups[4]));
        }
    }
    private static int ConvertGroupIntoInt(Group group) => int.Parse(group.Value);
}