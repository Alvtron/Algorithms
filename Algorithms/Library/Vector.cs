using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Library
{
    public struct Vector : IEnumerable<double>
    {
        #region Private properties

        /// <summary>
        /// The data (array) that contains all the components of this vector, where the index of x is 0, y is 1, z is 2, and so on.
        /// </summary>
        private readonly double[] _data;

        #endregion

        #region Public properties

        /// <summary>
        /// The number of dimensions of this vector.
        /// </summary>
        /// <value>
        /// The number of dimensions.
        /// </value>
        public int Dimensions => _data.Length;

        /// <summary>
        /// The data type of the components of this vector.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public Type DataType => typeof(double);

        /// <summary>
        /// The x-component of this vector. This is null if the vector does not have this component.
        /// </summary>
        /// <value>
        /// The x-component.
        /// </value>
        public double? X => Dimensions > 0 ? new double?(_data[0]) : null;

        /// <summary>
        /// The y-component of this vector. This is null if the vector does not have this component.
        /// </summary>
        /// <value>
        /// The y-component.
        /// </value>
        public double? Y => Dimensions > 1 ? new double?(_data[1]) : null;

        /// <summary>
        /// The z-component of this vector. This is null if the vector does not have this component.
        /// </summary>
        /// <value>
        /// The z-component.
        /// </value>
        public double? Z => Dimensions > 2 ? new double?(_data[2]) : null;

        /// <summary>
        /// Returns the sum of all components. Every component is added.
        /// </summary>
        /// <value>
        /// The sum.
        /// </value>
        public double Sum => _data.Sum();

        /// <summary>
        /// Returns the product of all components. Every component is multiplied.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        public double Product => _data.Aggregate((a, x) => a* x);

        /// <summary>
        /// Returns the magnitude of this vector. The magnitude of a vector gives the length of the line segment. Usually dentoted as ||v||.
        /// </summary>
        /// <value>
        /// The magnitude.
        /// </value>
        public double Magnitude
        {
            get
            {
                var result = 0.0;
                for (var i = 0; i < _data.Length; i++)
                    result += Math.Pow(this[i], 2);
                return Math.Sqrt(result);
            }
        }

        /// <summary>
        /// Returns the unit vector of this vector. A unit vector is a vector of length 1.
        /// </summary>
        /// <value>
        /// The unit vector.
        /// </value>
        public Vector Unit => new Vector(_data) / Magnitude;

        public double this[int dimension]
        {
            get => _data[dimension];
            set => _data[dimension] = value;
        }

        /// <summary>
        /// Returns the vector, but normalized into a probability distribution.
        /// </summary>
        /// <value>
        /// The softmax.
        /// </value>
        public Vector Softmax
        {
            get
            {
                var sumZExp = 0.0;
                var vector = new Vector(this);
                for (var i = 0; i < vector.Dimensions; i++)
                    sumZExp += vector[i] = Math.Exp(vector[i]);
                for (var i = 0; i < vector.Dimensions; i++)
                    vector[i] = vector[i] / sumZExp;
                return vector;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="columns">The columns or dimension size</param>
        public Vector(int columns)
        {
            _data = new double[columns];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">The x-component.</param>
        public Vector(double x)
        {
            _data = new[] { x };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">The x-component.</param>
        /// <param name="y">The y-component.</param>
        public Vector(double x, double y)
        {
            _data = new[] { x, y };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">The x-component.</param>
        /// <param name="y">The y-component.</param>
        /// <param name="z">The z-component.</param>
        public Vector(double x, double y, double z)
        {
            _data = new[] { x, z, y };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct with an already initialized vector.
        /// </summary>
        /// <param name="vector">The already-initialized vector.</param>
        public Vector(Vector vector)
        {
            _data = vector._data;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct with a list of vector components.
        /// </summary>
        /// <param name="data">The list of vector components.</param>
        /// <exception cref="InvalidOperationException">Cannot create Vector with empty list.</exception>
        public Vector(IReadOnlyList<double> data)
        {
            var columns = data.Count;
            if (data == null || columns == 0)
                throw new InvalidOperationException("Cannot create Vector with empty list.");
            _data = new double[columns];
            for (var j = 0; j < Dimensions; j++)
                _data[j] = data[j];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes the specified dimension/column.
        /// </summary>
        /// <param name="dimension">The dimension.</param>
        public void Remove(int dimension) => _data.RemoveAt(dimension);

        /// <summary>
        /// Swaps the specified columns/dimensions.
        /// </summary>
        /// <param name="firstColumn">The c1.</param>
        /// <param name="secondColumn">The c2.</param>
        /// <exception cref="InvalidOperationException">Cannot create Vector with empty list.</exception>
        private void Swap(int firstColumn, int secondColumn)
        {
            if (firstColumn == secondColumn)
                throw new InvalidOperationException("Cannot swap equal columns.");
            var temp = _data[secondColumn];
            _data[secondColumn] = _data[firstColumn];
            _data[firstColumn] = temp;
        }
        
        #endregion

        #region Overrides

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var result = "";
                for (var column = 0; column < Dimensions; column++)
                {
                    if (column == 0)
                        result += _data[column] + " ";
                    else if (column != Dimensions - 1)
                        result += (_data[column] < 0 ? "" : " ") + _data[column] + " ";
                    else
                        result += (_data[column] < 0 ? "" : " ") + _data[column];
                }

            return result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<double> GetEnumerator()
        {
            for (var j = 0; j < Dimensions; j++)
                yield return this[j];
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector vector)
                return this == vector;

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return 0;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Returns a sub-vector of the specified vector, decided by the start- and end-dimension/column.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="start">The start-dimension/column.</param>
        /// <param name="end">The end-dimension/column.</param>
        /// <returns>A sub-vector of the specified vector</returns>
        /// <exception cref="InvalidOperationException">The start is higher than the end.</exception>
        public static Vector SubVector(Vector vector, int start, int end)
        {
            if (start > end)
                throw new InvalidOperationException($"The start: {start} is higher than the end: {end}.");
            return new Vector(vector._data.SubArray(start, end - start));
        }

        /// <summary>
        /// Transforms the specified vector by processing each component with the provided function.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static Vector Transform(Vector vector, Func<double, double> function)
        {
            for (var column = 0; column < vector.Dimensions; column++)
                vector[column] = function(vector[column]);
            return vector;
        }

        /// <summary>
        /// Returns the angle between the specified vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="convertToDegrees">If true, returns the angle in degrees.</param>
        /// <returns>The angle between the specified vectors</returns>
        /// <exception cref="InvalidOperationException">Can't find dot product of vectors with a magnitude of zero.</exception>
        public static double Angle(Vector a, Vector b, bool convertToDegrees = false)
        {
            if (a.Magnitude.Equals(0.00) || b.Magnitude.Equals(0.00))
                throw new InvalidOperationException("Can't find dot product of vectors with a magnitude of zero.");
            var angle = Math.Acos(Dot(a, b) / (a.Magnitude * b.Magnitude));
            return !convertToDegrees ? angle : angle * (180.00 / Math.PI);
        }

        public static Vector Concat(Vector a, Vector b)
        {
            var result = new Vector(a.Dimensions + b.Dimensions);
            for (var column = 0; column < a.Dimensions; column++)
                result[column] = a[column];
            for (var column = 0; column < b.Dimensions; column++)
                result[column + a.Dimensions] = b[column];
            return result;
        }

        public static Vector Random(int columns, int? seed = null)
        {
            var random = seed.HasValue ? new Random(seed.Value) : new Random();
            var vector = new Vector(columns);
            for (var j = 0; j < columns; j++)
                vector[j] = random.NextDouble();
            return vector;
        }

        // ADDITION

        public static Vector operator +(Vector a, Vector b)
        {
            if (a.Dimensions != b.Dimensions)
            throw new InvalidOperationException($"Illegal vector dimensions: {a.Dimensions} != {b.Dimensions}");
            for (var j = 0; j < a.Dimensions; j++)
                a[j] += b[j];
            return a;
        }

        public static Vector operator +(Vector a, double scalar)
        {
            for (var column = 0; column < a.Dimensions; column++)
                a[column] += scalar;
            return a;
        }

        public static Vector operator +(double scalar, Vector a) => a + scalar;

        public static Vector Add(Vector a, Vector b) => a + b;


        // SUBTRACTION

        public static Vector operator -(Vector a, Vector b)
        {
            if (a.Dimensions != b.Dimensions)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Dimensions} != {b.Dimensions}");
            for (var j = 0; j < a.Dimensions; j++)
                a[j] -= b[j];
            return a;
        }

        public static Vector operator -(Vector a, double scalar)
        {
            for (var column = 0; column < a.Dimensions; column++)
                a[column] -= scalar;
            return a;
        }

        public static Vector Subtract(Vector a, Vector b) => a - b;

        // MULTIPLICATION

        public static Vector operator *(Vector a, Vector b)
        {
            if (b.Dimensions != a.Dimensions)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Dimensions} != {b.Dimensions}");
            var vector = new Vector(a.Dimensions);
                for (var j = 0; j < a.Dimensions; j++)
                    vector[j] = a[j] * b[j];
            return vector;
        }

        public static Vector operator *(Vector a, double scalar)
        {
            for (var column = 0; column < a.Dimensions; column++)
                a[column] *= scalar;
            return a;
        }

        public static Vector operator *(double scalar, Vector a) => a * scalar;

        public static double Dot(Vector a, Vector b)
        {
            if (b.Dimensions != a.Dimensions)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Dimensions} != {b.Dimensions}");
            return a.Select((t, j) => t * b[j]).Sum();
        }

        public static Vector Cross(Vector a, Vector b)
        {
            if (a.Dimensions != b.Dimensions)
                throw new InvalidOperationException("Illegal vector dimensions. Vectors need to be of same dimension.");
            if (a.Dimensions != 3)
                throw new InvalidOperationException("Illegal vector dimensions. To find the cross product, the vector needs to be of the third dimension (length = 3).");
            return new Vector(3)
            {
                [0] = a[1] * b[2] - a[2] * b[1],
                [1] = a[2] * b[0] - a[0] * b[2],
                [2] = a[0] * b[1] - a[1] * b[0]
            };
        }

        // DIVISION

        public static Vector operator /(Vector a, double scalar)
        {
            for (var column = 0; column < a.Dimensions; column++)
                a[column] /= scalar;
            return a;
        }

        // LOGICAL OPERATORS

        public static bool operator ==(Vector a, Vector b)
        {
            if (b.Dimensions != a.Dimensions)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Dimensions} != {b.Dimensions}");
            return !a.Where((t, j) => !t.Equals(b[j])).Any();
        }

        public static bool operator !=(Vector a, Vector b)
        {
            if (b.Dimensions != a.Dimensions)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Dimensions} != {b.Dimensions}");
            return a.Where((t, j) => !t.Equals(b[j])).Any();
        }
        
        #endregion
    }
}
