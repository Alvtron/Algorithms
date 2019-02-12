using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public static class MathExtensions
    {
        public static double Map(this double value, double sourceMinimum, double sourceMaximum, double targetMinimum, double targetMaximum)
        {
            return (value - sourceMinimum) * (targetMaximum - targetMinimum) / (sourceMaximum - sourceMinimum) + targetMinimum;
        }

        public static double NextDouble(this Random generator, double minimum, double maximum)
        {
            if (minimum > maximum)
                throw new InvalidOperationException($"The maximum value ({maximum}) is lower than the minimum value ({minimum}).");

            return generator.NextDouble() * (maximum - minimum) + minimum;
        }

        public static int NextInt32(this Random generator)
        {
            int firstBits = generator.Next(0, 1 << 4) << 28;
            int lastBits = generator.Next(0, 1 << 28);
            return firstBits | lastBits;
        }
    }
}
