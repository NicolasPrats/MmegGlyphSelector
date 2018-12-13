using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mmeg.Utils
{
    public static class RandomUtils
    {
        private static Random Rng = new Random();
        public static void RandomizeSort<T>(T[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                int j = Rng.Next(i, array.Length - 1);
                T tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
        }

        public static T GetNextWeightedRandomValue<T>(IEnumerable<T> values, Func<T, double> getWeight)
        {
            double total = values.Sum(v => getWeight(v));
            var index = Rng.NextDouble() * total;
            foreach (var value in values)
            {
                index -= getWeight(value);
                if (index <= 0)
                {
                    return value;
                }
            }
            throw new ApplicationException($"Bug: {index}/{total}");
        }
    }
}
