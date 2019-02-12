using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Library
{
    public struct Matrix :  IEnumerable<double>
    {
        #region Properties

        public enum Orientation
        {
            Column,
            Row
        };

        public readonly double[,] Data;

        public int Rows => Data?.GetLength(0) ?? 0;
        public int Columns => Data?.GetLength(1) ?? 0;
        public Type DataType => typeof(double);
        public bool IsSquare => Rows == Columns;
        public bool IsEmpty => Data == null;

        public Matrix Transpose
        {
            get
            {
                var transposedMatrix = new Matrix(Columns, Rows);

                for (var row = 0; row < Rows; row++)
                    for (var column = 0; column < Columns; column++)
                        transposedMatrix[column, row] = Data[row, column];

                return transposedMatrix;
            }
        }

        #endregion

        #region Constructors

        public Matrix(int rows, int columns)
        {
            Data = new double[rows, columns];
        }

        public Matrix(double value)
        {
            Data = new double[1, 1];
            Data[0, 0] = value;
        }

        public Matrix(double[,] data)
        {
            var row = data.GetLength(0);
            var column = data.GetLength(1);
            if (data == null || row == 0 || column == 0)
                throw new InvalidOperationException("Cannot create Matrix with empty list.");
            Data = new double[row, column];
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    Data[i,j] = data[i,j];
        }

        public Matrix(IReadOnlyList<double> data, Orientation orientation = Orientation.Column)
        {
            switch(orientation)
            {
                case Orientation.Column:
                    Data = new double[data.Count, 1];
                    for (var row = 0; row < Rows; row++)
                        Data[row, 0] = data[row];
                    break;
                case Orientation.Row:
                    Data = new double[1, data.Count];
                    for (var column = 0; column < Columns; column++)
                        Data[0, column] = data[column];
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public Matrix CreateColumnMatrix(IReadOnlyList<double> data, bool transpose)
        {
            return new Matrix(data);
        }

        public Matrix CreateRowMatrix(IReadOnlyList<double> data)
        {
            var matrix = new Matrix(0, data.Count);

            for (var column = 0; column < Columns; column++)
                matrix[0, column] = data[column];

            return matrix;
        }

        public Matrix(Vector vector)
        {
            Data = new double[vector.Dimensions, 1];

            for (var row = 0; row < Rows; row++)
                Data[row, 0] = vector[row];
        }

        public Matrix(IReadOnlyList<Vector> vectors)
        {
            if (vectors == null || vectors.Count == 0)
                throw new InvalidOperationException("Cannot create Matrix with empty list of vectors.");

            Data = new double[vectors[0].Dimensions, vectors.Count];

            for (var row = 0; row < Rows; row++)
            for (var column = 0; column < Columns; column++)
                Data[row, column] = vectors[column][row];
        }

        #endregion

        #region Methods

        public double this[int row, int column]
        {
            get => Data[row, column];
            set => Data[row, column] = value;
        }

        public void Set(int row, int column, double value) => Data[row, column] = value;

        public double Get(int row, int column) => Data[row, column];

        public double Sum()
        {
            var result = 0.0;
            for (var row = 0; row < Rows; row++)
                for (var column = 0; column < Columns; column++)
                    result += this[row, column];
            return result;
        }

        public void Swap(int firstRow, int secondRow)
        {
            for (var i = 0; i < Columns; ++i)
            {
                var temp = Data[secondRow, i];
                Data[secondRow, i] = Data[firstRow, i];
                Data[firstRow, i] = temp;
            }
        }

        public Matrix Transform(Func<double, double> function)
        {
            var matrix = new Matrix(Rows, Columns);

            for (var i = 0; i < matrix.Rows; i++)
                for (var j = 0; j < matrix.Columns; j++)
                    matrix[i, j] = function(this[i, j]);

            return matrix;
        }

        public Matrix Minors()
        {
            var minors = new Matrix(Columns, Rows);

            for (var row = 0; row < Rows; row++)
                for (var column = 0; column < Columns; column++)
                    minors[row, column] = Exclude(row, column).Determinant();

            return minors;
        }

        public Matrix CoFactors()
        {
            var coFactors = new Matrix(Columns, Rows);
            var minors = Minors();

            for (var row = 0; row < Rows; row++)
                for (var column = 0; column < Columns; column++)
                    coFactors[row, column] = column + row % 2 == 0 ? +minors[row, column] : -minors[row, column];

            return coFactors;
        }

        public Matrix Adjugate() => CoFactors().Transpose;

        public Matrix Inverse() => (1.0 / Determinant()) * Adjugate();

        public Matrix Normalize()
        {
            var sum = Sum();

            return new Matrix(this.Select(v => v / sum).ToArray());
        }

        public double Determinant()
        {
            if (!IsSquare)
                throw new InvalidOperationException("Cannot find the Determinant of a non-square matrix.");

            if (Rows == 2)
                return this[0, 0] * this[1, 1] - this[1, 0] * this[0, 1];

            var determinant = 0.0;

            for (var column = 0; column < Columns; column++)
            {
                var determinantMatrix = Exclude(0, column);
                determinant += this[0, column] * (column % 2 == 0 ? + determinantMatrix.Determinant() : - determinantMatrix.Determinant());
            }

            return determinant;
        }

        public Matrix Absolute()
        {
            var matrix = new Matrix(Data);

            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    matrix[i, j] = Math.Abs(matrix[i, j]);

            return matrix;
        }

        public Matrix Exclude(int excludeRow, int excludeColumn)
        {
            if (Rows < 1 || Columns < 1)
                throw new InvalidOperationException("Cannot find sub-matrix of matrices with just one row or column.");
            if (excludeRow < 0 || excludeColumn < 0)
                throw new InvalidOperationException("Negative row/columns number cannot be excluded.");
            var subMatrix = new Matrix(Rows - 1, Columns - 1);
            for (var row = 0; row < excludeRow; row++)
                for (var column = 0; column < excludeColumn; column++)
                    subMatrix[row, column] = this[row, column];
            for (var row = 0; row < excludeRow; row++)
                for (var column = excludeColumn + 1; column < Columns; column++)
                    subMatrix[row, column - 1] = this[row, column];
            for (var row = excludeRow + 1; row < Rows; row++)
                for (var column = 0; column < excludeColumn; column++)
                    subMatrix[row - 1, column] = this[row, column];
            for (var row = excludeRow + 1; row < Rows; row++)
                for (var column = excludeColumn + 1; column < Columns; column++)
                    subMatrix[row - 1, column - 1] = this[row, column];
            return subMatrix;
        }

        public Matrix SubMatrix(int startRow, int startColumn)
        {
            if (Rows < 1 || Columns < 1)
                throw new InvalidOperationException("Cannot find sub-matrix of matrices with just one row or column.");
            if (startRow < 0 || startColumn < 0)
                throw new InvalidOperationException("Can't find sub-matrix with a negative start row/column number.");
            var subMatrix = new Matrix(Rows - startRow, Columns - startColumn);
            for (var row = startRow; row < Rows; row++)
                for (var column = startColumn; column < Columns; column++)
                    subMatrix[row - startRow, column - startColumn] = this[row, column];
            return subMatrix;
        }

        /// <summary>
        /// Returns A^-1 vector, assuming A is square and has full rank
        /// </summary>
        /// <param name="rhs">The RHS.</param>
        /// <returns>x = A^-1 vector, assuming A is square and has full rank.</returns>
        /// <exception cref="InvalidOperationException">
        /// Illegal matrix dimensions: {Rows} != {rhs.Rows} || {Columns} != {rhs.Columns}
        /// or
        /// Illegal matrix dimensions.
        /// </exception>
        public Matrix Solve(Matrix rhs)
        {
            if (IsSquare || rhs.IsSquare || rhs.Columns != 1)
                throw new InvalidOperationException($"Illegal matrix dimensions: {Rows} != {rhs.Rows} || {Columns} != {rhs.Columns}");

            // create copies of the matrix
            var a = new Matrix(Data);
            var b = new Matrix(rhs.Data);

            // Gaussian elimination with partial pivoting
            for (var i = 0; i < Columns; i++)
            {
                // find pivot row and swap
                var max = i;
                for (var j = i + 1; j < Columns; j++)
                    if (Math.Abs(a[j, i]) > Math.Abs(a[max, i]))
                        max = j;
                a.Swap(i, max);
                b.Swap(i, max);

                // singular
                if (a[i, i].Equals(0.0))
                    throw new InvalidOperationException("Illegal matrix dimensions.");

                // pivot within vector
                for (var j = i + 1; j < Columns; j++)
                    b[j, 0] -= b[i, 0] * a[j, i] / a[i, i];

                // pivot within A
                for (var j = i + 1; j < Columns; j++)
                {
                    var m = a[j, i] / a[i, i];
                    for (var k = i + 1; k < Columns; k++)
                        a[j, k] -= a[i, k] * m;

                    a[j, i] = 0.0;
                }
            }

            // back substitution
            var x = new Matrix(Columns, 1);
            for (var j = Columns - 1; j >= 0; j--)
            {
                var t = 0.0;
                for (var k = j + 1; k < Columns; k++)
                    t += a[j, k] * x[k, 0];
                x[j, 0] = (b[j, 0] - t) / a[j, j];
            }

            return x;

        }

        public override string ToString()
        {
            var result = "";
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Columns; j++)
                {
                    if (j == 0)
                        result += Data[i, j] + " ";
                    else if (j != Columns - 1)
                        result += (Data[i, j] < 0 ? "" : " ") + Data[i, j] + " ";
                    else
                        result += (Data[i, j] < 0 ? "" : " ") + Data[i, j];
                }
                result += "\n";
            }

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<double> GetEnumerator()
        {
            for (var i = 0; i < Rows; i++)
                for (var j = 0; j < Columns; j++)
                    yield return this[i, j];
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix matrix)
                return this == matrix;

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -1357208469;
            hashCode *= -1521134295 + EqualityComparer<double[,]>.Default.GetHashCode(Data);
            hashCode *= -1521134295 + Rows.GetHashCode();
            hashCode *= -1521134295 + Columns.GetHashCode();
            return hashCode;
        }

        #endregion

        #region Static Methods

        public static Matrix ConcatHorizontally(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows)
                throw new InvalidOperationException("Illegal matrix dimensions: Rows must be equal!");
            var result = new Matrix(a.Rows, a.Columns + b.Columns);
            for (var row = 0; row < result.Rows; row++)
            {
                for (var column = 0; column < a.Columns; column++)
                    result[row, column] = a[row, column];
                for (var column = a.Columns; column < result.Columns; column++)
                    result[row, column] = b[row, column];
            }
            return result;
        }

        public static Matrix ConcatVertically(Matrix a, Matrix b)
        {
            if (a.Columns != b.Columns)
                throw new InvalidOperationException("Illegal matrix dimensions: Dimensions must be equal!");
            var result = new Matrix(a.Rows + b.Rows, a.Columns);
            for (var row = 0; row < a.Rows; row++)
                for (var column = 0; column < result.Columns; column++)
                    result[row, column] = a[row, column];
            for (var row = a.Rows; row < result.Rows; row++)
                for (var column = 0; column < result.Columns; column++)
                    result[row, column] = b[row - a.Rows, column];
            return result;
        }

        public static Matrix Random(int rows, int columns, int? seed = null)
        {
            var generator = seed.HasValue ? new Random(seed.Value) : new Random();
            var matrix = new Matrix(rows, columns);
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < columns; j++)
                    matrix[i, j] = generator.NextDouble();
            return matrix;
        }

        public static Matrix Random(int rows, int columns, double minimum, double maximum, int? seed = null)
        {
            var generator = seed.HasValue ? new Random(seed.Value) : new Random();
            var matrix = new Matrix(rows, columns);
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < columns; j++)
                    matrix[i, j] = generator.NextDouble(minimum, maximum);
            return matrix;
        }

        public static Matrix Random(int rows, int columns, double minimum, double maximum, double sum, int? seed = null)
        {
            var generator = seed.HasValue ? new Random(seed.Value) : new Random();
            var matrix = new Matrix(rows, columns);
            var randomList = ListExtensions.RandomList((uint)rows * (uint)columns, minimum, maximum, sum, generator).ToArray();
            var index = 0;
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < columns; j++)
                    matrix[i, j] = randomList[index++];
            return matrix;
        }

        public static Matrix Identity(int size)
        {
            if (size < 2)
                throw new InvalidOperationException("Illegal matrix dimension for identity matrix. Identity matrix must be larger than 1 row/column.");
            var identity = new Matrix(size,size);
            for (var i = 0; i < size; i++)
                identity[i, i] = 1;
            return identity;
        }

        public static Matrix Constant(int rows, int columns, double value)
        {
            if (rows < 0 || columns < 0)
                throw new InvalidOperationException("Illegal matrix dimension for constant matrix. Identity matrix must be larger than 0 rows/or 0 columns.");
            var matrix = new Matrix(rows, columns);
            return matrix + value;
        }

        public static double Similarity(Matrix a, Matrix b)
        {
            return (new Matrix(a.Data) - new Matrix(b.Data)).Absolute().Sum();
        }

        // ADDITION

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Rows == b.Rows && b.Columns == 1)
                return a + new Vector(b.ToArray());
            if (a.Columns == b.Columns && b.Rows == 1)
                return a + new Vector(b.ToArray());
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new InvalidOperationException($"Illegal matrix dimensions: {a.Rows}X{a.Columns} != {b.Rows}X{b.Columns}");

            var result = new Matrix(a.Data);

            for (var i = 0; i < a.Rows; i++)
                for (var j = 0; j < a.Columns; j++)
                    result[i, j] += b[i, j];

            return result;
        }

        public static Matrix operator +(Matrix matrix, Vector vector)
        {
            var result = new Matrix(matrix.Data);

            if (matrix.Rows == vector.Dimensions)
            {
                for (var i = 0; i < matrix.Rows; i++)
                    result[i, 0] += vector[i];
            }
            else if (matrix.Columns == vector.Dimensions)
            {
                for (var i = 0; i < matrix.Columns; i++)
                    result[0, i] += vector[i];
            }
            else
            {
                throw new InvalidOperationException($"Illegal matrix dimensions.");
            }
            return result;
        }

        public static Matrix operator +(Matrix matrix, double scalar)
        {
            var result = new Matrix(matrix.Data);

            for (var row = 0; row < result.Rows; row++)
            for (var column = 0; column < result.Columns; column++)
                result[row, column] += scalar;
            return result;
        }

        public static Matrix operator +(Vector vector, Matrix matrix) => matrix + vector;

        public static Matrix operator +(double scalar, Matrix a) => a + scalar;

        public static Matrix Add(Matrix a, Matrix b) => a + b;


        // SUBTRACTION

        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Rows == b.Rows && b.Columns == 1)
                return a + new Vector(b.ToArray());
            if (a.Columns == b.Columns && b.Rows == 1)
                return a + new Vector(b.ToArray());
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new InvalidOperationException($"Illegal matrix dimensions: {a.Rows}X{a.Columns} != {b.Rows}X{b.Columns}");

            var result = new Matrix(a.Data);

            for (var i = 0; i < a.Rows; i++)
                for (var j = 0; j < a.Columns; j++)
                    result[i, j] -= b[i, j];

            return result;
        }

        public static Matrix operator -(Matrix matrix, Vector vector)
        {
            var result = new Matrix(matrix.Data);

            if (matrix.Rows == vector.Dimensions)
            {
                for (var i = 0; i < matrix.Rows; i++)
                    result[i, 0] -= vector[i];
            }
            else if (matrix.Columns == vector.Dimensions)
            {
                for (var i = 0; i < matrix.Columns; i++)
                    result[0, i] -= vector[i];
            }
            else
            {
                throw new InvalidOperationException($"Illegal matrix dimensions.");
            }
            return result;
        }

        public static Matrix operator -(Matrix matrix, double scalar)
        {
            var result = new Matrix(matrix.Data);

            for (var row = 0; row < result.Rows; row++)
                for (var column = 0; column < result.Columns; column++)
                    result[row, column] -= scalar;
            return result;
        }

        public static Matrix Subtract(Matrix a, Matrix b) => a - b;

        // MULTIPLICATION

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (b.Rows != a.Rows || b.Columns != a.Columns)
                throw new InvalidOperationException($"Illegal matrix dimensions: {a.Rows}x{a.Columns} != {b.Rows}x{b.Columns}");

            var matrix = new Matrix(a.Rows, a.Columns);

            for (var i = 0; i < a.Rows; i++)
                for (var j = 0; j < a.Columns; j++)
                    matrix[i, j] = a[i, j] * b[i, j];

            return matrix;
        }

        public static Matrix operator *(Matrix matrix, double scalar)
        {
            var result = new Matrix(matrix.Data);

            for (var row = 0; row < result.Rows; row++)
                for (var column = 0; column < result.Columns; column++)
                    result[row, column] = matrix[row, column] * scalar;

            return result;
        }

        public static Matrix operator *(Matrix a, Vector b) => a * new Matrix(b);

        public static Matrix operator *(double scalar, Matrix matrix) => matrix * scalar;

        public static Matrix Multiply(Matrix a, Matrix b) => a * b;

        public static Matrix Dot(Matrix a, Matrix b)
        {
            if (a.Columns != b.Rows)
                throw new InvalidOperationException($"Illegal matrix dimensions: A columns and B rows must be equal ({a.Columns} != {b.Rows}).");

            var result = new Matrix(a.Rows, b.Columns);

            for (var i = 0; i < result.Rows; i++)
                for (var j = 0; j < result.Columns; j++)
                    for (var k = 0; k < a.Columns; k++)
                        result[i, j] = a[i, k] * b[k, j];

            return result;
        }

        // LOGICAL OPERATORS

        public static bool operator ==(Matrix a, Matrix b)
        {
            if (b.Rows != a.Rows || b.Columns != a.Columns)
                throw new InvalidOperationException($"Illegal matrix dimensions: {a.Rows}x{a.Columns} != {b.Rows}x{b.Columns}");
            for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Columns; j++)
                if (!a[i, j].Equals(b[i, j]))
                    return false;
            return true;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            if (b.Rows != a.Rows || b.Columns != a.Columns)
                throw new InvalidOperationException($"Illegal matrix dimensions: {a.Rows}x{a.Columns} != {b.Rows}x{b.Columns}");
            for (var i = 0; i < a.Rows; i++)
            for (var j = 0; j < a.Columns; j++)
                if (!a[i, j].Equals(b[i, j]))
                    return true;
            return false;
        }

        #endregion
    }
}
