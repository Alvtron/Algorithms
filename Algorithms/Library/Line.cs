using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Line
    {
        public Vector A { get; set; }

        public Vector B { get; set; }

        public Vector Anchor { get; set; }

        public int Dimensions { get; }

        public double Length
        {
            get
            {
                var lengthSquared = 0.0;
                for (var i = 0; i < Dimensions; i++)
                    lengthSquared += Math.Pow(B[i] - A[i], 2);
                return Math.Sqrt(lengthSquared);
            }
        }

        public double AngleInRadians => Vector.Angle(A, B, false);

        public double AngleInDegrees => Vector.Angle(A, B, true);

        public Line(Vector a, Vector b)
        {
            if (a.Dimensions != b.Dimensions) throw new InvalidOperationException($"{typeof(Vector).Name} A and B must have the same dimensions");
            A = a;
            B = b;
            Dimensions = a.Dimensions;
            Anchor = new Vector(Dimensions);
        }

        public Line(Vector a, Vector b, Vector anchor)
        {
            if (a.Dimensions != b.Dimensions) throw new InvalidOperationException($"{typeof(Vector).Name} A and B must have the same dimensions");
            A = a;
            B = b;
            Dimensions = a.Dimensions;
            Anchor = anchor;
        }
    }
}
