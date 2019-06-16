using System;

namespace Misc
{
    public class RandomNumberGenerator
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Generates a random integer between min (inclusive) and max (exclusive)
        /// </summary>
        public static int GetNext(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}