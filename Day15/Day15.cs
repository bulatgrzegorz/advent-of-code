using System.Net.Sockets;
using Xunit;

namespace adventOfCode.Day15;

public class Day15
{
    private const string InputFile = "Day15/Day15.input";

    // private const string InputExample = """
    //                                     #######
    //                                     #...#.#
    //                                     #.....#
    //                                     #..OO@#
    //                                     #..O..#
    //                                     #.....#
    //                                     #######
    //                                     
    //                                     <vv<<^^<<^^
    //                                     """;
    private const string InputExample = """
                                        ##########
                                        #..O..O.O#
                                        #......O.#
                                        #.OO..O.O#
                                        #..O@..O.#
                                        #O#..O...#
                                        #O..O..O.#
                                        #.OO.O.OO#
                                        #....O...#
                                        ##########
                                        
                                        <vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^
                                        vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v
                                        ><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<
                                        <<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^
                                        ^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><
                                        ^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^
                                        >^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^
                                        <><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>
                                        ^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>
                                        v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^
                                        """;
    
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
                Direction.Left => [..boxes.Where(x => x.RightSide.Row == LeftSide.Row && x.RightSide.Col == LeftSide.Col - 1)],
                Direction.Right => [..boxes.Where(x => x.LeftSide.Row == RightSide.Row && x.LeftSide.Col == RightSide.Col + 1)],
                Direction.Up or Direction.Down =>
                [
                    ..boxes.Where(x => x.IsIn(LeftSide.Move(direction))),
                    ..boxes.Where(x => x.IsIn(RightSide.Move(direction)))
                ],
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

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
            if (movedChar is '#')
            {
                moved = default;
                return false;
            }

            if (movedChar is '.')
            {
                Matrix[movedCoordinate.Row][movedCoordinate.Col] = '@';
                Matrix[initialRobot.Row][initialRobot.Col] = '.';
                moved = movedCoordinate;
                return true;
            }

            var firstBox = movedCoordinate;
            while (true)
            {
                movedCoordinate = movedCoordinate.Move(direction);
                movedChar = Matrix[movedCoordinate.Row][movedCoordinate.Col];
                if (movedChar is '#')
                {
                    moved = default;
                    return false;
                }
                
                if (movedChar is '.')
                {
                    Matrix[movedCoordinate.Row][movedCoordinate.Col] = 'O';
                    Matrix[firstBox.Row][firstBox.Col] = '@';
                    Matrix[initialRobot.Row][initialRobot.Col] = '.';
                    moved = firstBox;
                    return true;
                }
            }
        }
        
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

        public void Print()
        {
            Console.WriteLine("===============");
            
            for (var r = 0; r < _rows; r++)
            {
                for (var c = 0; c < _cols; c++)
                {
                    Console.Write(Matrix[r][c]);
                }
                
                Console.WriteLine();
            }
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
    public void Test()
    {
        var (map, directions) = ParseInput(InputExample.Split(Environment.NewLine));
        var robot = map.FindRobot();
        map.Print();
        
        foreach (var direction in directions)
        {
            if (map.Move(robot, direction, out var moved))
            {
                robot = moved;
            }
            map.Print();
        }
        
        Assert.Equal(2028, map.CountBoxes());
    }

    [Fact]
    public void First()
    {
        var (map, directions) = ParseInput(File.ReadAllLines(InputFile));
        var robot = map.FindRobot();
        // map.Print();
        
        foreach (var direction in directions)
        {
            if (map.Move(robot, direction, out var moved))
            {
                robot = moved;
            }
            // map.Print();
        }
        
        Assert.Equal(2028, map.CountBoxes());
    }
    
    [Fact]
    public void Second()
    {
        // var (map, robot, directions) = ParseInput2(InputExample.Split(Environment.NewLine));
        var (map, robot, directions) = ParseInput2(File.ReadAllLines(InputFile));
        // map.Print();
        
        var a = map.Walls.MaxBy(x => x.Col + x.Row);
        var matrix = new char[a.Row + 1][];
        for (int r = 0; r < matrix.Length; r++)
        {
            matrix[r] = new char[a.Col + 1];
            Array.Fill(matrix[r], '.');
        }
        
        // Print(matrix, map, robot);
        
        foreach (var direction in directions)
        {
            if (map.Move(robot, direction, out var moved))
            {
                robot = moved;
                // Print(matrix, map, robot);
            }
            // map.Print();
        }
        
        Assert.Equal(9021, map.Boxes.Sum(x => x.LeftSide.Row * 100 + x.LeftSide.Col));
    }

    private void Print(char[][] matrix, Map2 map, Coordinate robot)
    {
        Console.WriteLine("============================");
        for (int r = 0; r < matrix.Length; r++)
        {
            for (int c = 0; c < matrix[0].Length; c++)
            {
                if (map.Walls.Contains(new Coordinate(r, c)))
                {
                    Console.Write('#');
                }
                else if (map.Boxes.Any(x => x.LeftSide == new Coordinate(r, c)))
                {
                    Console.Write('[');
                }
                else if (map.Boxes.Any(x => x.RightSide == new Coordinate(r, c)))
                {
                    Console.Write(']');
                }
                else if (robot == new Coordinate(r, c))
                {
                    Console.Write('@');
                }
                else
                {
                    Console.Write('.');
                }
            }
            
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    private (Map2 map, Coordinate robot, Direction[] directions) ParseInput2(ReadOnlySpan<string> input)
    {
        var separator = input.IndexOf(string.Empty);
        var map = GetMap(input[..separator].ToArray());
        var boxes = new HashSet<Box>();
        var walls = new HashSet<Coordinate>();
        var doubled = Double(map.Matrix);
        Coordinate robot = default;
        for (int r = 0; r < doubled.Length; r++)
        {
            for (int c = 0; c < doubled[0].Length; c++)
            {
                switch (doubled[r][c])
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
        return (new Map2(boxes, walls), robot, GetDirections(input[separator..].ToArray()));
    }

    private char[][] Double(char[][] matrix)
    {
        var rows = new List<List<char>>();
        foreach (var matrixRow in matrix)
        {
            var row = new List<char>();
            for (var c = 0; c < matrix[0].Length; c++)
            {
                switch (matrixRow[c])
                {
                    case '#':
                        row.Add('#');
                        row.Add('#');
                        break;
                    case '@':
                        row.Add('@');
                        row.Add('.');
                        break;
                    case '.':
                        row.Add('.');
                        row.Add('.');
                        break;
                    default:
                        row.Add('[');
                        row.Add(']');
                        break;
                }
            }
            
            rows.Add(row);
        }
        
        return rows.Select(x => x.ToArray()).ToArray();
    }

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