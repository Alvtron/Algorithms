using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X}x, {Y}y";

        public override bool Equals(object obj)
        {
            if (!(obj is Position))
            {
                return false;
            }

            var position = (Position)obj;
            return X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
