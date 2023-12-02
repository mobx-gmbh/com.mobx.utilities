using System;
using System.Collections.Generic;
using System.Linq;

namespace MobX.Utilities
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadAttribute]
#endif
    public static class EnumUtility<T> where T : unmanaged, Enum
    {
        static EnumUtility()
        {
            values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        private static readonly T[] values;

        public static T[] GetValueArray()
        {
            return values;
        }

        public static T[] GetFlagsValueArray(T flagsEnum)
        {
            var buffer = new List<T>();

            foreach (var flag in GetValueArray())
            {
                if (flagsEnum.HasFlagUnsafe(flag))
                {
                    buffer.Add(flag);
                }
            }

            return buffer.ToArray();
        }
    }
}