using MobX.Utilities.Types;
using System;
using System.Runtime.CompilerServices;
using static Cysharp.Threading.Tasks.UniTask;

namespace MobX.Utilities.UniTask
{
    public static class UniTaskExtensions
    {
        #region Ducktyping

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Awaiter GetAwaiter(this TimeSpan timeSpan)
        {
            return Delay(timeSpan).GetAwaiter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Awaiter GetAwaiter(this Seconds seconds)
        {
            return Delay(seconds).GetAwaiter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Awaiter GetAwaiter(this float seconds)
        {
            return Delay(TimeSpan.FromSeconds(seconds)).GetAwaiter();
        }

        #endregion
    }
}