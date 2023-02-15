using System;
using static UnityEngine.Random;

namespace MobX.Utilities
{
    public static class RNG
    {
        public static bool Bool()
        {
            return value > .5f;
        }

        public static bool Bool(float percentage)
        {
            return value < percentage * .01f;
        }

        public static int Int(int min = int.MinValue, int max = int.MaxValue)
        {
            return Range(min, max);
        }

        public static long Int64()
        {
            return (long) Range(int.MinValue, int.MaxValue) + Range(int.MinValue, int.MaxValue);
        }

        public static int Int(Range range)
        {
            return Range(range.Start.Value, range.End.Value);
        }

        public static float Float(float min = float.MinValue, float max = float.MaxValue)
        {
            return Range(min, max);
        }
    }
}
