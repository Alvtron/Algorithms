using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Debug.WriteLine("Console initialized.\n");
            foreach (var arg in args) Debug.WriteLine(arg);

            TestSnake();
        }

        public static void TestSnake()
        {
            var snake = new Snake(10, 10, Snake.Difficulty.HARD);
        }

        public static void TestStatistics()
        {
            var generator = new Random();

            var randomNumbers = ListExtensions.RandomList(1000, generator);
            randomNumbers.PrintStatistics();

            var centered = randomNumbers.Center();
            centered.PrintStatistics();

            var scaled = randomNumbers.Scale();
            scaled.PrintStatistics();

            var xList = new double[] { 43, 21, 25, 42, 57, 59 };
            var yList = new double[] { 99, 65, 79, 75, 87, 81 };

            Debug.WriteLine($"Covariance: {ListExtensions.SampleCovariance(xList, yList)}");
            Debug.WriteLine($"Correlation Coefficient: {ListExtensions.SampleCorrelationCoefficient(xList, yList)}");
        }

        public static void TestRandomNumbersGenerator()
        {
            var min = -1.0;
            var max = 1.0;
            var sum = 0.0;
            uint values = 100000;
            var seed = 123;
            var generator = new Random(seed);

            var randomNumbers = ListExtensions.RandomList(values, min, max, sum, generator);

            randomNumbers.PrintStatistics();

            Debug.WriteLine("\nFirst 10 values:");
            randomNumbers.Take(10).ToList().ForEach(v => Debug.WriteLine(v));
        }

        public static void TestKnightsTour()
        {
            var size = 10;
            var x = size / 2;
            var y = size / 2;
            var knightsTour = new KnightsTour(size);
            var customChessPiece = new ChessPiece(new List<Position>
            {
                new Position(1,0),
                new Position(0,1),
                new Position(-1,0),
                new Position(0,-1)
            });
            Debug.WriteLine("TESTING WITH SPRINGER:");
            knightsTour.Run(x, y);
            knightsTour.PrintResult();
            Debug.WriteLine("TESTING WITH CUSTOM CHESS PIECE:");
            knightsTour.Run(customChessPiece, x, y);
            knightsTour.PrintResult();
        }

        public static void TestVector()
        {
            var random = new Random();
            var vector1 = new Vector(3);
            var vector2 = new Vector(3);

            for (var i = 0; i < vector1.Dimensions; i++)
            {
                vector1[i] = random.Next(-10, 10);
                vector2[i] = random.Next(-10, 10);
            }

            Debug.WriteLine("Matrix: \n" + vector1);
            Debug.WriteLine("Magnitude: \n" + vector1.Magnitude);
            Debug.WriteLine("Unit: \n" + vector1.Unit);
            Debug.WriteLine("Magnitude of unit: \n" + vector1.Unit.Magnitude);

            Debug.WriteLine($"({vector1}) + ({vector2}): \n" + (vector1 + vector2));
            Debug.WriteLine($"({vector1}) - ({vector2}): \n" + (vector1 - vector2));
            Debug.WriteLine($"({vector1}) * ({vector2}): \n" + (vector1 * vector2));
            Debug.WriteLine($"({vector1}) / 2: \n" + (vector1 / 2));
            Debug.WriteLine($"({vector1}) dot ({vector2}): \n" + Vector.Dot(vector1, vector2));
            Debug.WriteLine($"({vector1}) cross ({vector2}): \n" + Vector.Cross(vector1, vector2));
            Debug.WriteLine($"Angle between vector ({vector1}) and ({vector2}): \n");
            Debug.WriteLine($"Radians: {Vector.Angle(vector1, vector2)}");
            Debug.WriteLine($"Degrees: {Vector.Angle(vector1, vector2, true)}");
        }

        public static void TestMatrix()
        {
            var random = new Random();
            var matrix = new Matrix(3, 3);

            for (var i = 0; i < matrix.Rows; i++)
                for (var j = 0; j < matrix.Rows; j++)
                    matrix[i, j] = random.Next(-3, 10);

            Debug.WriteLine("Matrix: \n" + matrix);
            Debug.WriteLine("Sum: \n" + matrix.Sum());
            Debug.WriteLine("Determinant: \n" + matrix.Determinant());
            Debug.WriteLine("Minors: \n" + matrix.Minors());
            Debug.WriteLine("CoFactors: \n" + matrix.CoFactors());
            Debug.WriteLine("Adjugate: \n" + matrix.Adjugate());
            Debug.WriteLine("Inverse: \n" + matrix.Inverse());

            var matrixA = new Matrix(new[,]
            {
                {1.0, 2.0, 3.0},
                {4.0, 5.0, 6.0},
                {7.0, 8.0, 9.0}
            });

            var matrixB = new Matrix(new[,]
            {
                {1.0, 2.0, 3.0},
                {4.0, 5.0, 6.0},
                {7.0, 8.0, 9.0}
            });

            Debug.WriteLine("Matrix A dot Matrix B: \n" + Matrix.Dot(matrixA, matrixB));
        }

        public static void TestTree()
        {
            const string testWord1 = "cat";
            const string testWord2 = "remove me";

            var text = System.IO.File.ReadAllText(@"C:\Users\thoma\source\repos\AlgorithmsAndDataStructures\AlgorithmsAndDataStructures\words.txt");
            var words = text.Split('\n');
            var shuffledWords = words.OrderBy(a => Guid.NewGuid()).ToList();
            var wordList1 = shuffledWords.Take(shuffledWords.Count / 2).ToArray();
            var wordList2 = shuffledWords.Skip(shuffledWords.Count / 2).ToArray();

            var tree = new BinaryTree<string>();

            var stopwatch = new Stopwatch();

            Debug.WriteLine($"Inserting {wordList1.Length} words...");
            stopwatch.Start();
            tree.Add(wordList1);
            stopwatch.Stop();
            Debug.WriteLine($"Completed after {stopwatch.ElapsedMilliseconds} ms");

            Debug.WriteLine($"Inserting {wordList2.Length} words...");
            stopwatch.Restart();
            tree.Add(wordList2);
            stopwatch.Stop();
            Debug.WriteLine($"Completed after {stopwatch.ElapsedMilliseconds} ms");

            Debug.WriteLine($"Inserting {wordList2.Length} duplicate words...");
            stopwatch.Restart();
            tree.Add(wordList2);
            stopwatch.Stop();
            Debug.WriteLine($"Completed after {stopwatch.ElapsedMilliseconds} ms");

            Debug.WriteLine($"Inserting '{testWord1}'...");
            stopwatch.Restart();
            tree.Add(testWord1);
            tree.Add(testWord1);
            tree.Add(testWord1);
            tree.Add(testWord1);
            stopwatch.Stop();
            Debug.WriteLine($"Completed after {stopwatch.ElapsedMilliseconds} ms");

            Debug.WriteLine("\n\n----To List----\n");
            Debug.WriteLine("InOrder count : " + tree.ToList(Traversal.InOrder).Count);
            Debug.WriteLine("InOrder (unique) count : " + tree.ToList(Traversal.InOrder, true).Count);

            Debug.WriteLine("\n\n----Statistics----\n");
            Debug.WriteLine("Count: " + tree.Count);
            Debug.WriteLine("Unique count: " + tree.CountUnique);
            Debug.WriteLine("Balance: " + tree.Balance);
            Debug.WriteLine("Unique balance: " + tree.BalanceUnique);

            Debug.WriteLine($"Contains '{testWord1}'? {tree.Contains(testWord1)}");
            Debug.WriteLine($"{testWord1}' count: {tree.CountItem(testWord1)}");

            Debug.WriteLine($"Inserting '{testWord2}'...");
            tree.Add(testWord2);
            Debug.WriteLine($"Contains '{testWord2}'? {tree.Contains(testWord2)}");
            Debug.WriteLine($"Removing '{testWord2}'...");
            tree.Remove(testWord2);
            Debug.WriteLine($"Contains '{testWord2}'? {tree.Contains(testWord2)}");
        }
    }
}
