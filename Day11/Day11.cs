using System.Collections.Concurrent;
using System.Globalization;
using Xunit;

namespace adventOfCode.Day11;

public class Day11
{
    private const string ExampleInput = "125 17";
    private const string InputFile = "Day11/Day11.input";

    private record Tree(long Value, int Generation)
    {
        public bool IsOmitted { get; set; }
        public bool IsOver { get; set; }
        public Tree? Left { get; set; }
        public Tree? Right { get; set; }
    }

    [Fact]
    public void Test()
    {
        var stones = ExampleInput.Split(' ').Select(long.Parse).ToList();

        Assert.Distinct(stones);
        
        var result = CountStones(stones, 25);

        Assert.Equal(55312, result);
    }
    
    [Fact]
    public void First()
    {
        var stones = File.ReadAllText(InputFile).Split(' ').Select(long.Parse).ToList();

        Assert.Distinct(stones);
        
        var result = CountStones(stones, 25);

        Assert.Equal(233050, result);
    }
    
    [Fact]
    public void Second()
    {
        var stones = File.ReadAllText(InputFile).Split(' ').Select(long.Parse).ToList();
        
        Assert.Distinct(stones);
        
        var result = CountStones(stones, 75);

        Assert.Equal(276661131175807, result);
    }

    private long CountStones(List<long> stones, int generations)
    {
        var stoneTreeHeads = new ConcurrentDictionary<long, Tree>();
        var trees = stones.Select(x => new Tree(x, 0)).ToList();

        foreach (var tree in trees.AsParallel())
        {
            Traverse(tree, stoneTreeHeads, generations);
        }

        long result = 0;
        var sumCache = new ConcurrentDictionary<(long treeValue, int generation), long>();
        Parallel.ForEach(trees, () => 0L, (tree, _, localSum) =>
            {
                localSum += Sum(tree, 0, stoneTreeHeads, sumCache, generations);
                return localSum;
            },
            localSum => Interlocked.Add(ref result, localSum));
        
        return result;
    }

    private static long Sum(Tree? tree, int generation, ConcurrentDictionary<long, Tree> stoneTreeHeads, ConcurrentDictionary<(long treeValue, int generation), long> sumCache, int generations)
    {
        if (tree is null) return 0L;
        if (generation == generations) return 1;
        
        if (sumCache.TryGetValue((tree.Value, generation), out var sum)) return sum;

        tree = tree.IsOmitted || tree.IsOver ? stoneTreeHeads[tree.Value] : tree;

        var result =
            Sum(tree.Left, generation + 1, stoneTreeHeads, sumCache, generations) +
            Sum(tree.Right, generation + 1, stoneTreeHeads, sumCache, generations);

        sumCache.TryAdd((tree.Value, generation), result);

        return result;
    }

    private void Traverse(Tree? tree, ConcurrentDictionary<long, Tree> stoneTreeHeads, int generations)
    {
        if(tree is null) return;

        stoneTreeHeads.AddOrUpdate(tree.Value, _ => tree, (_, existing) =>
        {
            if (tree.Generation < existing.Generation)
            {
                return tree;
            }

            tree.IsOmitted = true;
            return existing;
        });

        if(tree.IsOmitted) return;
        
        if (tree.Generation == generations)
        {
            tree.IsOver = true;
            return;
        }
        
        if (tree.Value == 0)
        {
            tree.Left = new Tree(1, tree.Generation + 1);
        }
        else if (tree.Value.ToString().Length % 2 == 0)
        {
            var stoneNumber = tree.Value.ToString(CultureInfo.InvariantCulture).AsSpan();
            var firstStone = long.Parse(stoneNumber[..(stoneNumber.Length/2)]);
            var secondStone = long.Parse(stoneNumber[(stoneNumber.Length/2)..]);
                    
            tree.Left = new Tree(firstStone, tree.Generation + 1);
            tree.Right = new Tree(secondStone, tree.Generation + 1);
        }
        else
        {
            tree.Left = new Tree(tree.Value * 2024, tree.Generation + 1);
        }
            
        Traverse(tree.Left, stoneTreeHeads, generations);
        Traverse(tree.Right, stoneTreeHeads, generations);
    }
}