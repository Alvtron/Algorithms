using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Diagnostics;

namespace Library
{
    public static class ListExtensions
    {
        public static void PrintStatistics(this IEnumerable<double> list)
        {
            Debug.WriteLine("\nSTATISTICS:\n");
            Debug.WriteLine($"Distinct Values: {list.Distinct().Count()}");
            Debug.WriteLine($"Min: {list.Min()}");
            Debug.WriteLine($"Max: {list.Max()}");
            Debug.WriteLine($"Average: {list.Average()}");
            Debug.WriteLine($"Median: {list.Median()}");
            Debug.WriteLine($"Sum: {list.Sum()}");
            Debug.WriteLine($"Standard Deviation: {list.SampleStandardDeviation()}");
            Debug.WriteLine($"Skewness: {list.Skewness()}");
        }

        private static double Covariance(IEnumerable<double> X, IEnumerable<double> Y, int divisor)
        {
            if (X.Count() != Y.Count())
                throw new InvalidOperationException("Different array sizes!");

            var xMean = X.Average();
            var yMean = Y.Average();

            var sum = X.Zip(Y, (x1, y1) => (x1 - xMean) * (y1 - yMean)).Sum();
            return sum * divisor;
        }

        public static double SampleCovariance(IEnumerable<double> X, IEnumerable<double> Y)
        {
            return Covariance(X, Y, X.Count() - 1);
        }

        public static double PopulationCovariance(IEnumerable<double> X, IEnumerable<double> Y)
        {
            return Covariance(X, Y, X.Count());
        }

        public static double SampleCorrelationCoefficient(IEnumerable<double> X, IEnumerable<double> Y)
        {
            if (X.Count() != Y.Count())
                throw new InvalidOperationException("Different array sizes!");

            var xMean = X.Average();
            var yMean = Y.Average();

            var sum = X.Zip(Y, (x1, y1) => (x1 - xMean) * (y1 - yMean)).Sum();

            var sumSqr1 = X.Sum(x => Math.Pow(x - xMean, 2.0));
            var sumSqr2 = Y.Sum(y => Math.Pow(y - yMean, 2.0));

            return sum / Math.Sqrt(sumSqr1 * sumSqr2);
        }

        public static double PopulationCorrelationCoefficient(IEnumerable<double> X, IEnumerable<double> Y)
        {
            if (X.Count() != Y.Count())
                throw new InvalidOperationException("Different array sizes!");

            var covariance = PopulationCovariance(X, Y);

            var XSD = X.PopulationStandardDeviation();
            var YSD = Y.PopulationStandardDeviation();

            return covariance / (XSD * YSD);
        }

        public static double Skewness(this IEnumerable<double> numberSet)
        {
            var numbers = numberSet.Count();
            var mean = numberSet.Average();
            var variance = numberSet.SampleVariance();
            return numberSet.Sum(x => Math.Pow(x - mean, 3)) / ((numbers - 1) * Math.Pow(variance, 3.0/2.0));
        }

        public static IEnumerable<double> Scale(this IEnumerable<double> numberSet)
        {
            var numbers = numberSet.Count();
            var sd = numberSet.SampleStandardDeviation();
            return numberSet.Select(x => x / sd);
        }

        public static IEnumerable<double> Center(this IEnumerable<double> numberSet, double offset = 0.0)
        {
            var mean = numberSet.Average();
            return numberSet.Select(x => x - mean + offset);
        }

        private static double Variance(this IEnumerable<double> numberSet, double divisor)
        {
            var mean = numberSet.Average();
            return numberSet.Sum(x => Math.Pow(x - mean, 2)) / divisor;
        }

        public static double PopulationVariance(this IEnumerable<double> numberSet)
        {
            return numberSet.Variance(numberSet.Count());
        }

        public static double SampleVariance(this IEnumerable<double> numberSet)
        {
            return numberSet.Variance(numberSet.Count() - 1);
        }

        private static double StandardDeviation(this IEnumerable<double> numberSet, double divisor)
        {
            var mean = numberSet.Average();
            return Math.Sqrt(numberSet.Variance(divisor));
        }

        public static double PopulationStandardDeviation(this IEnumerable<double> numberSet)
        {
            return numberSet.StandardDeviation(numberSet.Count());
        }

        public static double SampleStandardDeviation(this IEnumerable<double> numberSet)
        {
            return numberSet.StandardDeviation(numberSet.Count() - 1);
        }

        public static IEnumerable<double> RandomList(uint values, Random generator = null)
        {
            if (values == 0)
                throw new InvalidOperationException($"Cannot create list of zero numbers.");
            if (generator == null)
                generator = new Random();

            for (var i = 0; i < values; i++)
            {
                yield return generator.NextDouble();
            }
        }

        public static IEnumerable<double> RandomList(double minimum, double maximum, int numberOfValues, int? seed = null)
        {
            if (minimum > maximum)
                throw new InvalidOperationException($"The maximum value ({maximum}) is lower than the minimum value ({minimum}).");

            var generator = seed.HasValue ? new Random(seed.Value) : new Random();
            for (var index = 0; index < numberOfValues; index++)
                yield return generator.NextDouble(minimum, maximum);
        }


        public static IEnumerable<double> RandomList(uint values, double minimum, double maximum, double sum, Random generator = null)
        {
            if (values == 0)
                throw new InvalidOperationException($"Cannot create list of zero numbers.");
            if (minimum * values > sum)
                throw new InvalidOperationException($"The minimum value ({minimum}) is too high.");
            if (maximum * values < sum)
                throw new InvalidOperationException($"The maximum value ({maximum}) is too low.");
            if (minimum > maximum)
                throw new InvalidOperationException($"The maximum value ({maximum}) is lower than the minimum value ({minimum}).");
            if (generator == null)
                generator = new Random();

            var numberList = new double[values];

            for (var index = 0; index < values - 1; index++)
            {
                var rest = numberList.Length - (index + 1);

                var restMinimum = minimum * rest;
                var restMaximum = maximum * rest;

                minimum = Math.Max(minimum, sum - restMaximum);
                maximum = Math.Min(maximum, sum - restMinimum);

                var newRandomValue = generator.NextDouble(minimum, maximum);
                numberList[index] = newRandomValue;
                sum -= newRandomValue;
            }

            numberList[values - 1] = sum;

            return numberList;
        }

        public static T Median<T>(this IEnumerable<T> items)
        {
            var i = (int)Math.Ceiling((double)(items.Count() - 1) / 2);
            if (i >= 0)
            {
                var values = items.ToList();
                values.Sort();
                return values[i];
            }

            return default(T);
        }

        public static int? Mode(this IEnumerable<double> items)
        {
            return items
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count()).ThenBy(x => x.Key)
                .Select(x => (int?)x.Key)
                .FirstOrDefault(); 
        }

        public static IEnumerable<double> Cast(this IEnumerable<double> doubleValues)
        {
            foreach (var doubleValue in doubleValues)
                yield return Convert.ToDouble(doubleValue);
        }

        public static IEnumerable<double> Map(this IEnumerable<double> source, double targetMinimum, double targetMaximum)
        {
            var sourceMinimum = source.Min();
            var sourceMaximum = source.Max();

            foreach (var value in source)
            {
                yield return value.Map(sourceMinimum, sourceMaximum, targetMinimum, targetMaximum);
            }
        }

        public static T Random<T>(this IEnumerable<T> enumerable, Random generator = null)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            if (generator == null) generator = new Random();

            var list = enumerable as IList<T> ?? enumerable.ToList();
            return list.Count == 0 ? default(T) : list[generator.Next(0, list.Count)];
        }

        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (s == null)
                throw new ArgumentNullException();
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.");

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }

        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            if (source.Length < 1) throw new InvalidOperationException("Cannot find sub-point with just one column.");
            if (index < 0) throw new InvalidOperationException("Index cannot be negative.");
            var dest = new T[source.Length - 1];
            if (index > 0)
                Array.Copy(source, 0, dest, 0, index);
            if (index < source.Length - 1)
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);
            return dest;
        }

        public static T[] SubArray<T>(this T[] source, int index, int length)
        {
            if (source.Length < 1) throw new InvalidOperationException("Cannot find sub-point with just one column.");
            if (index < 0 || length < 0) throw new InvalidOperationException("Invalid start or end index.");
            var result = new T[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }

        public static bool IsEqual<T>(this T[] a, T[] b)
        {
            if (a == null || b == null)
                throw new InvalidOperationException("Can't compare null arrays.");
            return a.SequenceEqual(b);
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list, Random generator = null)
        {
            generator = generator == null ? new Random() : generator;
            var items = list.ToList();

            while (items.Count > 0)
            {
                var k = generator.Next(0, items.Count);
                yield return items.ElementAt(k);
                items.RemoveAt(k);
            }
        }
    }
}
