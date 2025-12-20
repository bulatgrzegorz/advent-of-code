using System.Collections.Frozen;
using System.Numerics;
using Xunit;

namespace adventOfCode._2025.Day9;

public class Day9
{
    private const string ExampleInput = """
                                        7,1
                                        11,1
                                        11,7
                                        9,7
                                        9,5
                                        2,5
                                        2,3
                                        7,3
                                        """;

    [Fact]
    public void First()
    {
        // var lines = ExampleInput.GetExampleInputLines();
        var lines = InputHelper.GetInputLines();

        var titles = lines.Select((x, i) =>
        {
            var parts = x.Split(",");
            return (long.Parse(parts[0]), long.Parse(parts[1]));
        }).ToArray();

        var maxArea = titles
            .SelectMany(x => titles.Where(o => x != o), (x, y) => (x, y))
            .Select(pair => (pair.x, pair.y, area: pair.x.Area(pair.y)))
            .MaxBy(x => x.area);

        Assert.Equal(4759930955, maxArea.area);
    }
    
    [Fact]
    public void Second()
    {
        // var lines = ExampleInput.GetExampleInputLines();
        var lines = InputHelper.GetInputLines();

        var titles = lines.Select((x, i) =>
        {
            var parts = x.Split(",");
            return (col: long.Parse(parts[0]), row: long.Parse(parts[1]));
        }).ToArray();
        
        static bool Intersects((long col, long row) A, (long col, long row) B, (long col, long row) P) {
            // we want A to be before B
            // ---A------B---
            if (A.col > B.col) return Intersects(B, A, P);

            double pcol = P.col;
            
            //ray on vertex
            //if P lies on same col than A or B we move if to right by little bit 
            //----P----------
            //----A----------
            //-----------B---
            if (P.col == A.col || P.col == B.col)
                pcol += 0.0001;

            //if
            //--------B---P----
            //or
            //---P----A--------
            //or
            //----A----------
            //-----------B---
            //-------P-------
            if (pcol > B.col || pcol < A.col || P.row >= Math.Max(A.row, B.row))
                return false;

            //if
            //-------P-------
            //----A----------
            //-----------B---
            if (P.row < Math.Min(A.row, B.row))
                return true;

            //----A----------
            //---------P-----
            //-----------B---
            double red = (pcol - A.col) / (P.row - A.row);
            double blue = (B.col - A.col) / (double)(B.row - A.row);
            return red >= blue;
        }

        static bool Contains((long col, long row)[] shape, (long x, long y) pnt) {
            var inside = false;
            var len = shape.Length;
            for (var i = 0; i < len; i++) {
                var a =  shape[i];
                var b = shape[(i + 1) % len];
                
                var minCol = Math.Min(a.col, b.col);
                var maxCol = Math.Max(a.col, b.col);
                var minRow = Math.Min(a.row, b.row);
                var maxRow = Math.Max(a.row, b.row);
                
                var (A, B) = (a, b); 
                
                if(pnt.x == A.col && pnt.x == B.col && pnt.y >= minRow && pnt.y <= maxRow) return true;
                if(pnt.y == A.row && pnt.y == B.row && pnt.x >= minCol && pnt.x <= maxCol) return true;
                
                if (Intersects(A, B, pnt))
                    inside = !inside;
            }
            return inside;
        }

        Dictionary<(long, long), bool> cache = [];
        var shapeEdges = CalculateEdges(titles);

        var maxAreas = titles
            .SelectMany(x => titles.Where(o => x != o), (x, y) => (x, y))
            .Select(pair => (pair.x, pair.y, area: pair.x.Area(pair.y)))
            .OrderByDescending(x => x.area);

        long maxArea = 0;
        foreach (var ((p1Col, p1Row), (p2Col, p2Row), area) in maxAreas)
        {
            var fromCol = p1Col.Min(p2Col);
            var toCol = p1Col.Max(p2Col);
            
            var fromRow = p1Row.Min(p2Row);
            var toRow = p1Row.Max(p2Row);

            (long c, long r)[][] a =
            [
                [(fromCol, fromRow), (toCol, fromRow), (1, 0)],
                [(fromCol, toRow), (toCol, toRow), (1, 0)],
                [(fromCol, fromRow), (fromCol, toRow), (0, 1)],
                [(toCol, fromRow), (toCol, toRow), (0, 1)],
            ];

            var isFilled = true;
            foreach (var t in a)
            {
                var (point, until, iter) = (t[0], t[1], t[2]);
                
                var touched = shapeEdges.Contains(point);
                if (!touched)
                {
                    if (cache.TryGetValue(point, out var isInside))
                    {
                        if (!isInside)
                        {
                            isFilled = false;
                            break;
                        }
                    }
                    else
                    {
                        var x = Contains(titles, point);
                        cache.Add(point, x);
                        if (!x)
                        {
                            isFilled = false;
                            break; 
                        }
                    }
                }
                
                while (true)
                {
                    point += iter;
                    if(point.c > until.c || point.r > until.r) break;

                    var nowTouched = shapeEdges.Contains(point);
                    if (touched == nowTouched)
                    {
                        continue;
                    }
                    
                    touched = nowTouched;
                    if(touched) continue;

                    if (cache.TryGetValue(point, out var isInside))
                    {
                        if(isInside) continue;
                    }
                    else
                    {
                        var x = Contains(titles, point);
                        cache.Add(point, x);
                        if(x) continue;
                    }

                    isFilled = false;
                    break;
                }
                
                if(!isFilled) break;
            }

            if (!isFilled) continue;
            
            maxArea = area;
            break;
        }

        Assert.Equal(1525241870, maxArea);
    }

    private static FrozenSet<(long, long)> CalculateEdges((long col, long row)[] titles)
    {
        HashSet<(long, long)> pi = [];
        
        var len = titles.Length;
        for (var k = 0; k < len; k++)
        {
            var a = titles[k];
            var b = titles[(k + 1) % len];

            foreach (var (x, y) in a.Points(b))
            {
                pi.Add((x, y));
            }
        }

        var shapeEdges = pi.ToFrozenSet();
        return shapeEdges;
    }
}

public static class Extensions
{
    extension<T>((T x, T y) title) where T : INumber<T>
    {
        public T Area((T x, T y) other)
        {
            var dx = T.Abs(title.x - other.x) + T.One;
            var dy = T.Abs(title.y - other.y) + T.One;
            
            return dx * dy;
        }

        public static (T x, T y) operator +((T x, T y) left, (T x, T y) right)
        {
            return (left.x + right.x, left.y + right.y);
        }
        
        public IEnumerable<(T x, T y)> Points((T x, T y) other)
        {
            if (title.x == other.x)
            {
                var fromY = title.y.Min(other.y);
                var toY = title.y.Max(other.y);
                for (var i = fromY; i < toY + T.One; i++)
                {
                    yield return (title.x, i);
                }
            }
            else if (title.y == other.y)
            {
                var fromX = title.x.Min(other.x);
                var toX = title.x.Max(other.x);
                for (var i = fromX; i < toX + T.One; i++)
                {
                    yield return (i, title.y);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
    
    extension<T>(T num) where T : INumber<T>
    {
        public T Min(T other)
        {
            return num < other ? num : other;
        }
        
        public T Max(T other)
        {
            return num < other ? other : num;
        }
    }
}