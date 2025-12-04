using Xunit;

namespace adventOfCode._2024.Day15;

public class Day15
{
    private enum Direction { Up, Down, Left, Right }
    private readonly record struct Coordinate(int Row, int Col)
    {
        public Coordinate Move(Direction direction) => direction switch
        {
            Direction.Up => this with { Row = Row - 1 },
            Direction.Down => this with { Row = Row + 1 },
            Direction.Left => this with { Col = Col - 1 },
            Direction.Right => this with { Col = Col + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
    private class Box(Coordinate leftSide, Coordinate rightSide)
    {
        public Coordinate LeftSide { get; set; } = leftSide;
        public Coordinate RightSide { get; set; } = rightSide;

        public bool IsIn(Coordinate coordinate) => 
            (coordinate.Row == LeftSide.Row && coordinate.Col == LeftSide.Col) || 
            (coordinate.Row == RightSide.Row && coordinate.Col == RightSide.Col);

        public bool HasWallOnWay(HashSet<Coordinate> walls, Direction direction) => 
            walls.Contains(LeftSide.Move(direction)) || 
            walls.Contains(RightSide.Move(direction));

        public Box[] BoxesOnWay(HashSet<Box> boxes, Direction direction)
        {
            return direction switch
            {
                Direction.Left => [..boxes.Where(x => x.OnSameRow(this) && x.RightSide.Col == LeftSide.Col - 1)],
                Direction.Right => [..boxes.Where(x => x.OnSameRow(this) && x.LeftSide.Col == RightSide.Col + 1)],
                Direction.Up or Direction.Down =>
                [
                    ..boxes.Where(x => x.IsIn(LeftSide.Move(direction))),
                    ..boxes.Where(x => x.IsIn(RightSide.Move(direction)))
                ],
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        private bool OnSameRow(Box other) => LeftSide.Row == other.LeftSide.Row;

        public void Move(Direction direction)
        {
            LeftSide = LeftSide.Move(direction);
            RightSide = RightSide.Move(direction);
        }
    }

    private readonly record struct Map2(HashSet<Box> Boxes, HashSet<Coordinate> Walls)
    {
        public bool Move(Coordinate initialRobot, Direction direction, out Coordinate moved)
        {
            var movedCoordinate = initialRobot.Move(direction);
            if (Walls.Contains(movedCoordinate))
            {
                moved = default;
                return false;
            }

            var boxesOn = Boxes.Where(x => x.IsIn(movedCoordinate)).ToList();
            if (boxesOn.Count is 0)
            {
                moved = movedCoordinate;
                return true;
            }
            
            var boxOn = boxesOn.First();
            if (boxOn.HasWallOnWay(Walls, direction))
            {
                moved = default;
                return false;
            }

            var boxesToMove = new HashSet<Box>() { boxOn };
            if (!TraverseBoxes(direction, boxOn, boxesToMove))
            {
                moved = default;
                return false;
            }

            foreach (var boxToMove in boxesToMove)
            {
                boxToMove.Move(direction);
            }
            
            moved = movedCoordinate;
            return true;
        }

        private bool TraverseBoxes(Direction direction, Box boxOn, HashSet<Box> boxesToMove)
        {
            var walls = Walls;

            var boxesOnWay = boxOn.BoxesOnWay(Boxes, direction);
            if (boxesOnWay is [])
            {
                return true;
            }

            if (boxesOnWay.Any(x => x.HasWallOnWay(walls, direction)))
            {
                return false;
            }

            boxesToMove.UnionWith(boxesOnWay);

            foreach (var boxOnWay in boxesOnWay)
            {
                if (!TraverseBoxes(direction, boxOnWay, boxesToMove))
                {
                    return false;
                }
            }

            return true;
        }
    }
    
    private readonly record struct Map(char[][] Matrix)
    {
        private readonly int _rows = Matrix.Length;
        private readonly int _cols = Matrix[0].Length;
        
        public bool Move(Coordinate initialRobot, Direction direction, out Coordinate moved)
        {
            var movedCoordinate = initialRobot.Move(direction);
            var movedChar = Matrix[movedCoordinate.Row][movedCoordinate.Col];
            switch (movedChar)
            {
                case '#':
                    moved = default;
                    return false;
                case '.':
                    Set('@', movedCoordinate);
                    Set('.', initialRobot);
                    moved = movedCoordinate;
                    return true;
            }

            var firstBox = movedCoordinate;
            while (true)
            {
                movedCoordinate = movedCoordinate.Move(direction);
                movedChar = Matrix[movedCoordinate.Row][movedCoordinate.Col];
                switch (movedChar)
                {
                    case '#':
                        moved = default;
                        return false;
                    case '.':
                        Set('O', movedCoordinate);
                        Set('@', firstBox);
                        Set('.', initialRobot);
                        moved = firstBox;
                        return true;
                }
            }
        }
        
        private void Set(char value, Coordinate coordinate) => Matrix[coordinate.Row][coordinate.Col] = value;
        
        public Coordinate FindRobot()
        {
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    if(Matrix[r][c] == '@') return new Coordinate(r, c);
                }
            }
            
            throw new Exception("Could not find robot in given map");
        }

        public int CountBoxes()
        {
            var result = 0;
            
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    if(Matrix[r][c] is 'O') result += 100 * r + c;
                }
            }

            return result;
        }
    }

    [Fact]
    public void First()
    {
        var (map, directions) = ParseInput(InputHelper.GetInputLines());
        var robot = map.FindRobot();

        foreach (var direction in directions)
        {
            if (map.Move(robot, direction, out var moved))
            {
                robot = moved;
            }
        }
        
        Assert.Equal(1441031, map.CountBoxes());
    }
    
    [Fact]
    public void Second()
    {
        var (map, robot, directions) = ParseInput2(InputHelper.GetInput());

        foreach (var direction in directions)
        {
            if (map.Move(robot, direction, out var moved))
            {
                robot = moved;
            }
        }
        
        Assert.Equal(1425169, map.Boxes.Sum(x => x.LeftSide.Row * 100 + x.LeftSide.Col));
    }

    private (Map2 map, Coordinate robot, Direction[] directions) ParseInput2(string input)
    {
        input = Double(input);
        var lines = input.Split(Environment.NewLine).AsSpan();
        var separator = lines.IndexOf(string.Empty);
        var boxes = new HashSet<Box>();
        var walls = new HashSet<Coordinate>();
        
        var matrix = lines[..separator].ToArray().Select(x => x.ToArray()).ToArray();
        Coordinate robot = default;
        for (var r = 0; r < matrix.Length; r++)
        {
            for (var c = 0; c < matrix[0].Length; c++)
            {
                switch (matrix[r][c])
                {
                    case '#':
                        walls.Add(new Coordinate(r, c));
                        break;
                    case '@':
                        robot = new Coordinate(r, c);
                        break;
                    case '[':
                        boxes.Add(new Box(new Coordinate(r, c), new Coordinate(r, c + 1)));
                        break;
                }
            }
        }
        return (new Map2(boxes, walls), robot, GetDirections(lines[separator..].ToArray()));
    }

    private static string Double(string input) => input.Replace("#", "##").Replace(".", "..").Replace("O", "[]").Replace("@", "@.");

    private static (Map map, Direction[] directions) ParseInput(ReadOnlySpan<string> input)
    {
        var separator = input.IndexOf(string.Empty);
        return (GetMap(input[..separator].ToArray()), GetDirections(input[separator..].ToArray()));
    }

    private static Direction[] GetDirections(string[] input) => input.SelectMany(x => x.ToCharArray()).Select(x => x switch
    {
        '^' => Direction.Up,
        '<' => Direction.Left,
        'v' => Direction.Down,
        '>' => Direction.Right,
        _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
    }).ToArray();
    
    private static Map GetMap(string[] input) => new(input.Select(x => x.ToArray()).ToArray());
}