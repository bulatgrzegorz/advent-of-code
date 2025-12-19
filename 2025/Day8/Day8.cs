using Xunit;

namespace adventOfCode._2025.Day8;

public class Day8
{
    private const string ExampleInput = """
                                        162,817,812
                                        57,618,57
                                        906,360,560
                                        592,479,940
                                        352,342,300
                                        466,668,158
                                        542,29,236
                                        431,825,988
                                        739,650,466
                                        52,470,668
                                        216,146,977
                                        819,987,18
                                        117,168,530
                                        805,96,715
                                        346,949,466
                                        970,615,88
                                        941,993,340
                                        862,61,35
                                        984,92,344
                                        425,690,689
                                        """;
    
    [Fact]
    public void First()
    {
        // var lines = ExampleInput.GetExampleInputLines();
        var lines = InputHelper.GetInputLines();

        var boxes = lines.Select((x, i) =>
        {
            var parts = x.Split(",");
            return new Box(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]), i);
        }).ToArray();

        var pairDistances = boxes
            .SelectMany(x => boxes.Where(o => x != o), (x, y) => (x, y))
            .Select(pair => (pair.x, pair.y, distance: pair.x.Distance(pair.y)))
            .OrderBy(x => x.distance);

        var allCircuits = new HashSet<(Box b1, Box b2)>();

        int[] parent = [..Enumerable.Range(0, boxes.Length)];
        var rank = new int[boxes.Length];

        foreach (var (b1, b2, _) in pairDistances)
        {
            if(!allCircuits.Add(Box.ToTuple(b1, b2))) continue;
            
            Union(b1.Index, b2.Index, rank, parent);
            
            if (allCircuits.Count > 999)
            {
                break;
            }
        }
        
        var sizes = new int[boxes.Length];
        for (var i = 0; i < boxes.Length; i++)
        {
            sizes[Find(i, parent)]++;
        }
        
        var result3 = sizes
            .OrderByDescending(x => x)
            .Take(3)
            .Mul();

        Assert.Equal(50568, result3);
    }
    
    [Fact]
    public void Second()
    {
        // var lines = ExampleInput.GetExampleInputLines();
        var lines = InputHelper.GetInputLines();

        var boxes = lines.Select((x, i) =>
        {
            var parts = x.Split(",");
            return new Box(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]), i);
        }).ToArray();

        var pairDistances = boxes
            .SelectMany(x => boxes.Where(o => x != o), (x, y) => (x, y))
            .Select(pair => (pair.x, pair.y, distance: pair.x.Distance(pair.y)))
            .OrderBy(x => x.distance);

        var allCircuits = new HashSet<(Box b1, Box b2)>();

        int[] parent = [..Enumerable.Range(0, boxes.Length)];
        var rank = new int[boxes.Length];

        var groups = boxes.Length;
        foreach (var (b1, b2, _) in pairDistances)
        {
            if(!allCircuits.Add(Box.ToTuple(b1, b2))) continue;
            
            if(Union(b1.Index, b2.Index, rank, parent))
                groups--;

            if (groups != 1) continue;
            
            Assert.Equal(36045012, b1.X * b2.X);
            return;
        }
    }

    private static int Find(int x, int[] parent)
    {
        while (parent[x] != x)
        {
            parent[x] = parent[parent[x]]; // path halving
            x = parent[x];
        }
        return x;
    }

    private static bool Union(int x, int y, int[] rank, int[] parent)
    {
        var (rx, ry) = (Find(x, parent),  Find(y, parent));
        if (rx == ry) return false;

        if (rank[rx] < rank[ry]) parent[rx] = ry;
        else if (rank[rx] > rank[ry]) parent[ry] = rx;
        else
        {
            parent[ry] = rx;
            rank[rx]++;
        }
        
        return true;
    }
    
    
    private readonly record struct Box(long X, long Y, long Z, int Index)
    {
        public double Distance(Box other)
        {
            var x = (X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y) + (Z - other.Z) * (Z - other.Z);
            return Math.Sqrt(x);
        }
         
        public static (Box Box1, Box Box2) ToTuple(Box box1, Box box2) => box1 < box2 ? (box1, box2) : (box2, box1);
        
        public static bool operator <(Box a, Box b) => a.X + a.Y + a.Z < b.X + b.Y + b.Z ;

        public static bool operator >(Box a, Box b) => a.X + a.Y + a.Z > b.X + b.Y + b.Z;
    }
}