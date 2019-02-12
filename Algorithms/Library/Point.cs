using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Linq;

namespace Library
{
    public struct Point : IEnumerable<double?>
    {
        private readonly double?[] _data;
        public int Components => _data.Length;
        public double? X => Components > 0 ? _data[0] : null;
        public double? Y => Components > 1 ? _data[1] : null;
        public double? Z => Components > 2 ? _data[2] : null;

        public double? this[int dimension]
        {
            get => _data[dimension];
            set => _data[dimension] = value;
        }

        #region Constructors

        public Point(int dimensions)
        {
            _data = new double?[dimensions];
        }

        public Point(double x)
        {
            _data = new[] { new double?(x) };
        }

        public Point(double x, double y)
        {
            _data = new[] { x, new double?(y) };
        }

        public Point(double x, double y, double z)
        {
            _data = new[] { x, z, new double?(y) };
        }

        #endregion

        #region Methods

        public static Point Rotate(Point point, double angle)
        {
            return Rotate(point, new Point(0.0, 0.0), angle);
        }

        public static Point Rotate(Point point, Point anchor, double angle)
        {
            angle *= Math.PI / 180;

            var rotatedX = Math.Cos(angle) * (point.X - anchor.X) - Math.Sin(angle) * (point.Y - anchor.Y) + anchor.X;
            var rotatedY = Math.Sin(angle) * (point.X - anchor.X) + Math.Cos(angle) * (point.Y - anchor.Y) + anchor.Y;

            return new Point(rotatedX.Value, rotatedY.Value);
        }

        #endregion

        #region Operators

        public static Point operator +(Point a, Point b)
        {
            if (a.Components != b.Components)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Components} != {b.Components}");
            var point = new Point(a.Components);
            for (var i = 0; i < a.Components; i++)
                point[i] = a[i] + b[i];
            return point;
        }

        public static Point operator +(Point a, double scalar)
        {
            var point = new Point(a.Components);
            for (var i = 0; i < a.Components; i++)
                point[i] = a[i] + scalar;
            return point;
        }

        public static Point operator +(double scalar, Point a) => a + scalar;

        public static Point operator -(Point a, Point b)
        {
            if (a.Components != b.Components)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Components} != {b.Components}");
            var point = new Point(a.Components);
            for (var i = 0; i < a.Components; i++)
                point[i] = a[i] - b[i];
            return point;
        }

        public static Point operator -(Point a, double scalar)
        {
            var point = new Point(a.Components);
            for (var i = 0; i < a.Components; i++)
                point[i] = a[i] - scalar;
            return point;
        }

        public static Point Subtract(Point a, Point b) => a - b;

        public static Point operator *(Point a, Point b)
        {
            if (b.Components != a.Components)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Components} != {b.Components}");
            var point = new Point(a.Components);
            for (var i = 0; i < a.Components; i++)
                point[i] = a[i] * b[i];
            return point;
        }

        public static Point operator *(Point a, double scalar)
        {
            var point = new Point(a.Components);
            for (var i = 0; i < a.Components; i++)
                point[i] = a[i] * scalar;
            return point;
        }

        public static Point operator *(double scalar, Point a) => a * scalar;

        public static Point operator /(Point a, double scalar)
        {
            var point = new Point(a.Components);
            for (var i = 0; i < a.Components; i++)
                point[i] = a[i] / scalar;
            return point;
        }

        public static bool operator ==(Point a, Point b)
        {
            if (b.Components != a.Components)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Components} != {b.Components}");
            return a.Where((t, j) => !t.Equals(b[j])).Any();
        }

        public static bool operator !=(Point a, Point b)
        {
            if (b.Components != a.Components)
                throw new InvalidOperationException($"Illegal vector dimensions: {a.Components} != {b.Components}");
            return a.Where((t, j) => !t.Equals(b[j])).Any();
        }

        #endregion

        #region Overrides

        public IEnumerator<double?> GetEnumerator()
        {
            for (var i = 0; i < Components; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Equals(Point other)
        {
            return Equals(_data, other._data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Point other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (_data != null ? _data.GetHashCode() : 0);
        }

        public override string ToString()
        {
            switch (Components)
            {
                case 1:
                    return $"{X}x";
                case 2:
                    return $"{X}x,{Y}y";
                case 3:
                    return $"{X}x,{Y}y,{Z}z";
                default:
                    return "";
            }
        }

        #endregion
    }
}
