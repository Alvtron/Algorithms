using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Library
{
    public class Player
    {
        public Position Position { get; private set; }

        public int Size { get; set; } = 1;

        public static List<Position> PositionHistory { get; private set; }

        public long Moves => PositionHistory.Count;

        public Player(int x, int y)
        {
            Position = new Position(x, y);
        }
        
        public void Shift(int x, int y)
        {
            Move(new Position(Position.X + x, Position.Y + y));
        }

        public void Move(int x, int y)
        {
            Move(new Position(x, y));
        }

        public void Move(Position position)
        {
            if (PositionHistory == null)
                PositionHistory = new List<Position>();

            PositionHistory.Add(position);
            Position = position;
        }
    }

    public class Snake
    {
        public enum Difficulty
        {
            EASY,
            MEDIUM,
            HARD,
            EXPERT
        }

        private enum Tile
        {
            EMPTY,
            SNAKE,
            WALL,
            WORM
        }

        private Matrix Table { get; set; }
        private Player Player { get; set; }
        private Position Worm { get; set; }
        private Position NextMove { get; set; }
        private Position LastMove { get; set; }
        private Timer Timer { get; set; }
        private int Score { get; set; }
        private bool IsRunning { get; set; } 

        public Snake(int width, int height, Difficulty difficulty = Difficulty.MEDIUM)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Table = new Matrix(height, width);

            var xPosition = width / 2;
            var yPosition = height / 2;
            Player = new Player(xPosition, yPosition);

            CreateWalls();
            Table.Set(Player.Position.Y, Player.Position.X, (double)Tile.SNAKE);
            SpawnWorm();

            Timer = new Timer
            {
                Interval = DifficultyToTime(difficulty),
                AutoReset = true
            };
            IsRunning = true;

            DrawTable();
            Timer.Elapsed += OnTick;
            Timer.Start();
            Loop();
        }

        private int DifficultyToTime(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.EASY:
                    return 1000;
                case Difficulty.MEDIUM:
                    return 800;
                case Difficulty.HARD:
                    return 500;
                case Difficulty.EXPERT:
                    return 200;
                default:
                    return 1000;
            }
        }

        private void MovePlayer()
        {
            Player.Move(Player.Position.X - NextMove.X, Player.Position.Y - NextMove.Y);
            if (Player.Position.X < 0
                || Player.Position.X >= Table.Columns
                || Player.Position.Y < 0
                || Player.Position.Y >= Table.Rows
                || (Table[Player.Position.Y, Player.Position.X] != (double)Tile.EMPTY
                && Table[Player.Position.Y, Player.Position.X] != (double)Tile.WORM))
            {
                GameOver();
                return;
            }
            if (Table[Player.Position.Y, Player.Position.X] == (double)Tile.WORM)
            {
                Score += 100;
                Player.Size++;
                SpawnWorm();
            }
            DrawPlayer();
            LastMove = NextMove;
        }

        private void DrawPlayer()
        {
            Table = Table.Transform(x => x = x == (double)Tile.SNAKE ? (double)Tile.EMPTY : x); 
            var positionHistory = new Stack<Position>(Player.PositionHistory);
            for (var i = 0; i < Player.Size; i++)
                Table.Set(positionHistory.Peek().Y, positionHistory.Pop().X, (double)Tile.SNAKE);
        }

        private void GameOver()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("GAME OVER");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Final score: {Score}");
            StopGame();
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (NextMove == null) return;
            MovePlayer();
            DrawTable();
        }

        private void CreateWalls()
        {
            for (var row = 0; row < Table.Rows; row++)
                Table.Set(row, 0, (double)Tile.WALL);
            for (var row = 0; row < Table.Rows; row++)
                Table.Set(row, Table.Columns - 1, (double)Tile.WALL);
            for (var column = 0; column < Table.Columns; column++)
                Table.Set(0, column, (double)Tile.WALL);
            for (var column = 0; column < Table.Columns; column++)
                Table.Set(Table.Rows - 1, column, (double)Tile.WALL);
        }

        private void Loop()
        {
            while (IsRunning)
            {

                switch(Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        SetNextMove(0, 1);
                        break;
                    case ConsoleKey.LeftArrow:
                        SetNextMove(1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                        SetNextMove(0, -1);
                        break;
                    case ConsoleKey.RightArrow:
                        SetNextMove(-1, 0);
                        break;
                    case ConsoleKey.Escape:
                        IsRunning = false;
                        break;
                    default:
                        break;
                }
            }
            StopGame();
        }

        private void StopGame()
        {
            Timer.Stop();
            IsRunning = false;
        }

        private void SpawnWorm()
        {
            List<Position> availablePositions = new List<Position>();

            for (var row = 0; row < Table.Rows; row++)
                for (var column = 0; column < Table.Columns; column++)
                    if (Table[row, column] == (double)Tile.EMPTY)
                        availablePositions.Add(new Position(row, column));

            Worm = availablePositions.Random();

            Table.Set(Worm.X, Worm.Y, (double)Tile.WORM);
        }

        private void SetNextMove(int x, int y)
        {
            NextMove = NextMove == null || (LastMove.X - x != 0 && LastMove.Y - y != 0)
                ? new Position(x, y) : NextMove;
        }

        private void DrawTable()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            var tableString = Table.ToString();
            tableString = tableString
                .Replace($"{(int)Tile.WALL}", "■")
                .Replace($"{(int)Tile.SNAKE}", "©")
                .Replace($"{(int)Tile.WORM}", "¤")
                .Replace($"{(int)Tile.EMPTY}", " ");
            Console.WriteLine(tableString);
            Console.WriteLine($"Player position: x{Player.Position.X}, y{Player.Position.Y}");
            Console.WriteLine($"Score: {Score}");
        }
    }
}