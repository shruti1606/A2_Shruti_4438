using System;

public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}

public class Player
{
    public string Name { get; }
    public Position Position { get; set; }
    public int GemCount { get; set; }

    public Player(string name, Position position)
    {
        Name = name;
        Position = position;
        GemCount = 0;
    }

    public void Move(char direction)
    {
        switch (direction)
        {
            case 'U':
                Position.Y--;
                break;
            case 'D':
                Position.Y++;
                break;
            case 'L':
                Position.X--;
                break;
            case 'R':
                Position.X++;
                break;
            default:
                Console.WriteLine("Invalid direction input.");
                break;
        }
    }
}

public class Cell
{
    public string Occupant { get; set; }
}

public class Board
{
    public Cell[,] Grid { get; }
    private readonly Random _random;

    public Board()
    {
        Grid = new Cell[6, 6];
        _random = new Random();
        InitBoard();
    }

    private void InitBoard()
    {
        // Initialization of all cells with empty occupant
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Grid[i, j] = new Cell { Occupant = "-" };
            }
        }

        // to place players
        Grid[0, 0].Occupant = "P1";
        Grid[5, 5].Occupant = "P2";

        // to place gems
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (_random.Next(10) < 2) // 20% chance of placing a gem
                {
                    Grid[i, j].Occupant = "G";
                }
            }
        }

        // to place obstacles
        for (int i = 0; i < 6; i++)
        {
            int obsX = _random.Next(6);
            int obsY = _random.Next(6);
            Grid[obsX, obsY].Occupant = "O";
        }
    }

    public void Display()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Console.Write(Grid[i, j].Occupant + " ");
            }
            Console.WriteLine();
        }
    }

    public bool IsValidMove(Player player, char direction)
    {
        int newX = player.Position.X;
        int newY = player.Position.Y;

        switch (direction)
        {
            case 'U':
                newY--;
                break;
            case 'D':
                newY++;
                break;
            case 'L':
                newX--;
                break;
            case 'R':
                newX++;
                break;
            default:
                return false;
        }

        if (newX < 0 || newX >= 6 || newY < 0 || newY >= 6)
        {
            return false; // Out of bounds
        }

        if (Grid[newY, newX].Occupant == "O")
        {
            return false; // Obstacle
        }

        return true;
    }

    public void CollectGem(Player player)
    {
        if (Grid[player.Position.Y, player.Position.X].Occupant == "G")
        {
            player.GemCount++;
            Grid[player.Position.Y, player.Position.X].Occupant = "-";
            Console.WriteLine($"{player.Name} collected a gem!");
        }
    }
}

public class Game
{
    public Board Board { get; }
    public Player Player1 { get; }
    public Player Player2 { get; }
    public Player CurrentTurn { get; private set; }
    public int TotalTurns { get; private set; }

    public Game()
    {
        Board = new Board();
        Player1 = new Player("P1", new Position(0, 0));
        Player2 = new Player("P2", new Position(5, 5));
        CurrentTurn = Player1;
        TotalTurns = 0;
    }

    public void Start()
    {
        while (!IsGameOver())
        {
            Console.WriteLine($"Turn {TotalTurns + 1}");
            Board.Display();
            Console.WriteLine($"Current player: {CurrentTurn.Name}");
            Console.Write("Enter move (U/D/L/R): ");
            char direction = char.ToUpper(Console.ReadKey().KeyChar);
            Console.WriteLine();

            if (Board.IsValidMove(CurrentTurn, direction))
            {
                CurrentTurn.Move(direction);
                Board.CollectGem(CurrentTurn);
                TotalTurns++;
                SwitchTurn();
            }
            else
            {
                Console.WriteLine("Invalid move. Try again.");
            }
        }

        AnnounceWinner();
    }

    public void SwitchTurn()
    {
        CurrentTurn = CurrentTurn == Player1 ? Player2 : Player1;
    }

    public bool IsGameOver()
    {
        return TotalTurns >= 30;
    }

    public void AnnounceWinner()
    {
        Console.WriteLine("Game Over!");
        Console.WriteLine($"Player 1 collected {Player1.GemCount} gems.");
        Console.WriteLine($"Player 2 collected {Player2.GemCount} gems.");

        if (Player1.GemCount > Player2.GemCount)
        {
            Console.WriteLine("Player 1 wins!");
        }
        else if (Player2.GemCount > Player1.GemCount)
        {
            Console.WriteLine("Player 2 wins!");
        }
        else
        {
            Console.WriteLine("It's a tie!");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Start();
    }
}