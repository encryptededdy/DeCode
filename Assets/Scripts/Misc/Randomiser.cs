using System;
using System.Collections.Generic;

namespace Misc
{
    public static class RandomNumberGenerator
    {
        public static int GetNext(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static List<T> Shuffle<T>(List<T> list)
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
    }
}