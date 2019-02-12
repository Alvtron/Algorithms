using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Library
{
    public class AnalyzedMove : Position
    {
        public int Ways { get; private set; }

        public AnalyzedMove(int x, int y, int ways) : base(x, y)
        {
            Ways = ways;
        }
    }

    public class ChessPiece
    {
        public Position Position { get; private set; }

        public List<Position> LegalMoves { get; private set; }

        public Stack<Position> PositionHistory { get; } = new Stack<Position>();

        public long Moves { get; private set; } = 0;

        public ChessPiece(List<Position> legalMoves)
        {
            LegalMoves = legalMoves;
        }

        /// <summary>
        /// Moves the chess piece to the provided xy-position.
        /// </summary>
        /// <param name="x">The new x-position.</param>
        /// <param name="y">The new y-position.</param>
        public void Move(int x, int y)
        {
            Move(new Position(x, y));
        }

        /// <summary>
        /// Moves the chess piece to the provided position.
        /// </summary>
        /// <param name="position"></param>
        public void Move(Position position)
        {
            PositionHistory.Push(position);
            Position = position;
            Moves++;
        }

        /// <summary>
        /// Moves the chess piece back to the previous position.
        /// </summary>
        public void MoveBack()
        {
            if (PositionHistory.Count < 2) throw new InvalidOperationException("No previous position to go back to.");
            PositionHistory.Pop();
            Position = PositionHistory.Peek();
            Moves++;
        }

        /// <summary>
        /// Prints the position history.
        /// </summary>
        public void PrintPositionHistory()
        {
            var positionHistory = new Stack<Position>(PositionHistory.Reverse());
            while (positionHistory.Count > 1) Debug.WriteLine($"({positionHistory.Pop()}) to ({positionHistory.Peek()})");
        }
    }

    /// <summary>
    /// A traditional chess piece with already-defined legal moves.
    /// </summary>
    /// <seealso cref="Library.ChessPiece" />
    public class Springer : ChessPiece
    {
        public Springer() : base(
            new List<Position>
            {
                new Position(+2, +1),
                new Position(+2, -1),
                new Position(+1, +2),
                new Position(-1, +2),
                new Position(-2, +1),
                new Position(-2, -1),
                new Position(+1, -2),
                new Position(-1, -2)
            })
        {
        }
    }

    public class KnightsTour
    {
        private const int EMPTY = 0, USED = 1;

        private int _size;
        public int Size
        {
            get => _size;
            private set
            {
                _size = value;
                Cells = value * value;
            }
        }

        public Matrix Table { get; private set; }

        public int Cells { get; private set; }

        public ChessPiece ChessPiece { get; private set; }

        /// <summary>
        /// Creates a new instance of the Knight's Tour and initalizes the table.
        /// </summary>
        /// <param name="size">The size of the table.</param>
        public KnightsTour(int size)
        {
            Debug.WriteLine($"Creating table with size {size}x{size}.");

            Size = size;
        }

        /// <summary>
        /// Finds the next 1-step way from current provided position.
        /// </summary>
        /// <param name="x">The x-position.</param>
        /// <param name="y">The y-position.</param>
        /// <returns>true if path is found, otherwise false.</returns>
        public bool FindWayRecursively(int x, int y)
        {
            // Save the position in the move list
            ChessPiece.Move(x, y);
            // Set cell at position (x,y) as visited
            Table.Set(x, y, USED);
            // Counter for all moves
            var numberOfMoves = ChessPiece.PositionHistory.Count;
            // If all the cells has been visited, return true;
            if (numberOfMoves == Cells) return true;
            // Get list of next moves with number of ways.
            var analyzedNextMoves = FindNextMoves(ChessPiece).ToList();
            if (analyzedNextMoves.Count == 0)
            {
                // The problem cannot be solved from this position.
                // Marking this position as 'EMPTY'.
                Table.Set(x, y, EMPTY);
                // If this is not the start position, remove last move from move list.
                if (ChessPiece.PositionHistory.Count != 0) ChessPiece.MoveBack();
                return false;
            }

            if (numberOfMoves != Cells - 1)
            {
                var orderedNextMoves = new Stack<AnalyzedMove>(analyzedNextMoves.Where(move => move.Ways > 0).OrderByDescending(move => move.Ways));

                while (orderedNextMoves.Count > 0)
                {
                    var nextMove = orderedNextMoves.Pop();
                    if (FindWayRecursively(nextMove.X, nextMove.Y))
                        return true;
                }
            }

            var lastMove = analyzedNextMoves.FirstOrDefault();
            return FindWayRecursively(lastMove.X, lastMove.Y);
        }

        /// <summary>
        /// Finds the next moves based on the legal ways of a chess piece.
        /// </summary>
        /// <param name="chessPiece">The chess piece.</param>
        /// <returns></returns>
        private IEnumerable<AnalyzedMove> FindNextMoves(ChessPiece chessPiece)
        {
            for (var index = 0; index < chessPiece.LegalMoves.Count; index++)
            {
                var suggestedMove = new Position(chessPiece.Position.X + chessPiece.LegalMoves[index].X, chessPiece.Position.Y + chessPiece.LegalMoves[index].Y);

                if (suggestedMove.X < 0 || suggestedMove.X >= Table.Columns || suggestedMove.Y < 0 || suggestedMove.Y >= Table.Rows || !IsAvailable(suggestedMove))
                    continue;

                yield return new AnalyzedMove(suggestedMove.X, suggestedMove.Y, CountWays(suggestedMove, ChessPiece.LegalMoves));
            }
        }

        public bool IsAvailable(Position position) => Table[position.X, position.Y] == EMPTY;

        /// <summary>
        /// Determines the number of available ways from provided xy-position.
        /// </summary>
        /// <param name="x">The x-position.</param>
        /// <param name="y">The y-position.</param>
        /// <returns>Number of available ways.</returns>
        public int CountWays(Position position, List<Position> legalMoves)
        {
            int ways = 0;

            foreach (var legalMove in ChessPiece.LegalMoves)
            {
                var newPosition = new Position(position.X + legalMove.X, position.Y + legalMove.Y);

                if (newPosition.X < 0 || newPosition.X >= Table.Columns || newPosition.Y < 0 || newPosition.Y >= Table.Rows || Table[newPosition.X, newPosition.Y] == USED)
                    continue;

                ways++;
            }

            return ways;
        }

        /// <summary>
        /// Solves the problem from the provided xy-position with a springer chess piece.
        /// </summary>
        /// <param name="x">The x-position.</param>
        /// <param name="y">The y-position.</param>
        public void Run(int x = 0, int y = 0)
        {
            Run(new Springer(), x, y);
        }

        /// <summary>
        /// Solves the problem from the provided xy-position with the specified chess piece.
        /// </summary>
        /// <param name="chessPiece">The chess piece.</param>
        /// <param name="x">The x-position.</param>
        /// <param name="y">The y-position.</param>
        /// <exception cref="InvalidOperationException">
        /// The xy-position cannot be negative.
        /// or
        /// The xy-position cannot exceed the table size: {Size}
        /// </exception>
        public void Run(ChessPiece chessPiece, int x = 0, int y = 0)
        {
            if (x < 0 || y < 0)
            {
                throw new InvalidOperationException($"The xy-position cannot be negative.");
            }
            if (x > Size || y > Size)
            {
                throw new InvalidOperationException($"The xy-position cannot exceed the table size: {Size}.");
            }

            Debug.WriteLine($"Looking for a path with starting position ({x}, {y})...");

            Table = new Matrix(Size, Size);
            ChessPiece = chessPiece;
            bool success = false;
            var watch = Stopwatch.StartNew();
            
            try
            {
                success = FindWayRecursively(x, y);
            }
            catch (StackOverflowException)
            {
                Debug.WriteLine("The recursion is too long. Try with a smaller table size.");
                watch.Reset();
                return;
            }

            watch.Stop();
            var time = watch.ElapsedMilliseconds;

            Debug.WriteLine($"Completed after {time} ms with {ChessPiece.Moves} moves.");

            if (!success)
            {
                Debug.WriteLine("Could not find a sollution.");
                return;
            }
            Debug.WriteLine("Found a solution to the problem.");
        }

        /// <summary>
        /// Prints the result.
        /// </summary>
        public void PrintResult()
        {
            var printMatrix = new Matrix(Table.Data);
            var step = 1;
            foreach (var position in ChessPiece.PositionHistory.Reverse())
            {
                printMatrix.Set(position.X, position.Y, step++);
            }

            Debug.WriteLine(printMatrix);
        }

        /// <summary>
        /// Prints the table.
        /// </summary>
        public void PrintTable()
        {
            Debug.WriteLine(new Matrix(Table.Data));
        }
    }
}
