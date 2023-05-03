using MobX.Utilities.Types;
using System;
using System.Runtime.CompilerServices;

namespace MobX.Utilities.Extensions
{
    public static class ConvertExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seconds Seconds(this float value)
        {
            return new Seconds(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seconds Seconds(this int value)
        {
            return new Seconds(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Seconds Seconds(this TimeSpan value)
        {
            return new Seconds(value);
        }
    }
}