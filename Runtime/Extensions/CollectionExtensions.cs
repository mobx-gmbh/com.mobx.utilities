using MobX.Utilities.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Pool;

namespace MobX.Utilities
{
    public static class CollectionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<T>(this T[] array, Action<T> action)
        {
            for (var i = 0; i < array.Length; i++)
            {
                action(array[i]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array is not { Length: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>(this T[] array)
        {
            return array is { Length: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list is not { Count: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNullOrEmpty<T>(this IList<T> list)
        {
            return list is { Count: > 0 };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementAt<T>(this T[] array, int index, out T element)
        {
            if (array.Length > index)
            {
                element = array[index];
                return true;
            }
            element = default(T);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetElementAt<T>(this IList<T> list, int index, out T element)
        {
            if (list.Count > index)
            {
                element = list[index];
                return true;
            }
            element = default(T);
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveDuplicates<T>(this IList<T> list)
        {
            HashSet<T> set = HashSetPool<T>.Get();

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (!set.Add(list[i]))
                {
                    list.RemoveAt(i);
                }
            }

            HashSetPool<T>.Release(set);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveNull<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].IsNull())
                {
                    list.RemoveAt(i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddNullChecked<T>(this IList<T> list, T value) where T : class
        {
            if (value.IsNull())
            {
                return false;
            }

            list.Add(value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUnique<T>(this IList<T> list, T value, bool nullCheck = false)
        {
            if (nullCheck && value == null)
            {
                return false;
            }

            if (list.Contains(value))
            {
                return false;
            }

            list.Add(value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUnique<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key, TValue value)
        {
            if (target.ContainsKey(key))
            {
                return false;
            }

            target.Add(key, value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> target, TKey key, TValue value)
        {
            if (target == null)
            {
                return;
            }

            if (target.ContainsKey(key))
            {
                target[key] = value;
                return;
            }

            target.Add(key, value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> updateFunc)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = updateFunc(dictionary[key]);
            }
            else
            {
                dictionary.Add(key, updateFunc(default(TValue)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddTyped<TValue>(this IDictionary<Type, TValue> target, TValue value)
        {
            Type key = value.GetType();
            if (target.ContainsKey(key))
            {
                target[key] = value;
            }

            target.Add(key, value);
        }

        /// <summary>
        ///     Remove all null objects from a dictionary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveNullItems<TKey, TValue>(this IDictionary<TKey, TValue> target) where TValue : class
        {
            List<TKey> invalidKeys = ListPool<TKey>.Get();
            foreach ((TKey key, TValue value) in target)
            {
                if (value.IsNull())
                {
                    invalidKeys.Add(key);
                }
            }

            foreach (TKey invalidKey in invalidKeys)
            {
                target.Remove(invalidKey);
            }

            ListPool<TKey>.Release(invalidKeys);
        }

        /// <summary>
        ///     Adds an object to a list and returns the same list. This allows method chaining.
        ///     <example>list.Append(item1).Append(item2);</example>
        /// </summary>
        public static List<T> Append<T>(this List<T> list, T value)
        {
            list.Add(value);
            return list;
        }

        /// <summary>
        ///     Returns true if the enumeration contains no elements.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool None<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        /// <summary>
        ///     Returns a string, appending string representation of every element in the collection.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToCollectionString<T>(this IEnumerable<T> enumerable)
        {
            StringBuilder stringBuilder = StringBuilderPool.Get();
            foreach (T item in enumerable)
            {
                var text = item.ToString();
                stringBuilder.Append(text);
                stringBuilder.Append('\n');
            }

            return StringBuilderPool.Release(stringBuilder);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : defaultValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] array, T element)
        {
            if (array == null)
            {
                return -1;
            }
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < array.Length; i++)
            {
                if (comparer.Equals(array[i], element))
                {
                    return i;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }

            return false;
        }
    }
}
