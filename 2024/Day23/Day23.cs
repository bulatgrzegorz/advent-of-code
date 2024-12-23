using System.Collections.Concurrent;
using Xunit;

namespace adventOfCode._2024.Day23;

public class Day23
{
    [Fact]
    public void First()
    {
        var connections = InputHelper.GetInputLines().Select(x => x.Split('-')).ToArray();
        var left2Right = connections.ToLookup(x => x[0], x => x[1]);
        var right2Left = connections.ToLookup(x => x[1], x => x[0]);

        var founded = new HashSet<string>();
        const int lanPartySize = 3;
        
        foreach (var connection in left2Right.Where(x => x.Key.StartsWith('t')))
        {
            Traverse(lanPartySize - 1, [connection.Key], left2Right, right2Left, [], founded);
        }

        Assert.Equal(1170, founded.Count);
    }
    
    [Fact]
    public void Second()
    {
        var connections = InputHelper.GetInputLines().Select(x => x.Split('-')).ToArray();
        var left2Right = connections.ToLookup(x => x[0], x => x[1]);
        var right2Left = connections.ToLookup(x => x[1], x => x[0]);

        ConcurrentDictionary<string, string[]> allEdges = [];
        foreach (var v in left2Right.Union(right2Left))
        {
            allEdges.AddOrUpdate(v.Key, s => [s, ..v], (s, vv) => [..vv, ..v]);
        }

        string[] currentLongest = [];
        foreach (var (key, values) in allEdges)
        {
            var currentInt = values;
            if(currentLongest.SequenceEqual(values)) continue;
            foreach (var val in values)
            {
                if(val == key) continue;
                var intersected = allEdges[val].Intersect(currentInt).ToArray();
                
                //if it has no elements else then itself and "parent", then ignore it
                var has = intersected.AsSpan().IndexOfAnyExcept([val, key]);
                if(has is -1) continue;
                
                currentInt = intersected;
            }
            
            if(currentInt.Length > currentLongest.Length) currentLongest = currentInt;
        }

        Assert.Equal("bo,dd,eq,ik,lo,lu,ph,ro,rr,rw,uo,wx,yg", string.Join(",", currentLongest.Order()));
    }

    private static void Traverse(int level, string[] components, ILookup<string, string> left2Right, ILookup<string, string> right2Left, HashSet<(int, string)> alreadyTraversed, HashSet<string> result)
    {
        if (level > 0)
        {
            foreach (var v in left2Right[components[^1]].Union(right2Left[components[^1]]))
            {
                if(components.Contains(v)) continue;
                if(!alreadyTraversed.Add((level, string.Join("", ((string[])[..components, v]).Order())))) continue;
                
                Traverse(level - 1, [..components, v], left2Right, right2Left, alreadyTraversed, result);
            }
            
            return;
        }

        if (left2Right[components[^1]].Union(right2Left[components[^1]]).All(v => v != components[0])) return;
        
        result.Add(string.Join(",", components.Order()));
    }
}