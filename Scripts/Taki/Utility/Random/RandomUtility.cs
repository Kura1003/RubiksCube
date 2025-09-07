using UnityEngine;

namespace Taki.Utility
{
    internal static class RandomUtility
    {
        internal static Color GetColor(float alpha = 1.0f)
        {
            var random = SeedGenerator.GetRandom();
            return new Color(
                random.Next(0, 2),
                random.Next(0, 2),
                random.Next(0, 2),
                alpha);
        }

        internal static int Range(int min, int max)
        {
            var random = SeedGenerator.GetRandom();
            return random.Next(min, max);
        }

        internal static int Range(int max)
        {
            var random = SeedGenerator.GetRandom();
            return random.Next(0, max);
        }

        internal static double Range(double min, double max)
        {
            var random = SeedGenerator.GetRandom();
            return random.NextDouble() * (max - min) + min;
        }

        internal static bool CoinToss()
        {
            var random = SeedGenerator.GetRandom();
            return random.Next(0, 2) == 1;
        }
    }
}