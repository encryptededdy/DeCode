using System;
using System.Collections.Generic;
using System.Linq;

namespace Misc
{
    public static class Randomiser
    {
        public static int GetNext(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static List<T> ShuffleList<T>(List<T> list)
        {
            Random random = new Random();
            for (int i = 0; i < list.Count; i++)
            {
                int k = random.Next(0, i);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }

            return list;
        }

        public static bool RandomValuesFromDict<TKey, TValue>(IDictionary<TKey, TValue> dict, out TKey value)
        {
            Random rand = new Random();
            List<TKey> values = dict.Keys.ToList();
            var size = dict.Count;

            if (size == 0)
            {
                value = default;
                return false;
            }

            value = values[rand.Next(size)];
            return true;
        }
    }
}