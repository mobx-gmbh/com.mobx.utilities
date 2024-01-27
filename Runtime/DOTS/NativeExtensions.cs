using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace MobX.Utilities.DOTS
{
    public static class NativeExtensions
    {
        public static ref T GetRef<T>(this NativeArray<T> array, int index)
            where T : struct
        {
            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            unsafe
            {
                return ref UnsafeUtility.ArrayElementAsRef<T>(array.GetUnsafePtr(), index);
            }
        }

        public static ref T GetRef<T>(this NativeList<T> list, int index)
            where T : unmanaged
        {
            if (index < 0 || index >= list.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            unsafe
            {
                return ref UnsafeUtility.ArrayElementAsRef<T>(list.GetUnsafePtr(), index);
            }
        }
    }
}