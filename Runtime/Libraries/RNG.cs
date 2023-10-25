using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;
using static UnityEngine.Random;

namespace MobX.Utilities.Libraries
{
    public static class RNG
    {
        #region Methods

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

        public static Vector3 Vector()
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1));
        }

        public static Vector3 Vector(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            return new Vector3(Float(minX, maxX), Float(minY, maxY), Float(minZ, maxZ));
        }

        public static Vector3 Vector(float x, float y, float z)
        {
            return new Vector3(Float(-x, x), Float(-y, y), Float(-z, z));
        }

        public static Vector3 Vector(float value)
        {
            return new Vector3(Float(-value, value), Float(-value, value), Float(-value, value));
        }

        public static Vector3 VectorNormalized()
        {
            return new Vector3(Float(-1, 1), Float(-1, 1), Float(-1, 1)).normalized;
        }

        public static T FromSelection<T>(T[] selection)
        {
            var index = Int(0, selection.Length);
            return selection[index];
        }

        #endregion


        #region Extensions

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomItem<T>(this IReadOnlyList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.Count == 1)
            {
                return source[0];
            }

            var randomIndex = Range(0, source.Count);
            return source[randomIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomItemRemove<T>(this IList<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var randomIndex = Range(0, source.Count);
            var item = source[randomIndex];
            source.RemoveAt(randomIndex);
            return item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] RandomItems<T>(this IList<T> source, int count)
        {
            Assert.IsNotNull(source);
            Assert.IsTrue(source.Count >= count,
                $"Cannot select {count} random items from a collection with {source.Count} items!");

            var buffer = HashSetPool<T>.Get();

            while (buffer.Count < count)
            {
                buffer.Add(source[Range(0, source.Count)]);
            }

            var selection = buffer.ToArray();
            HashSetPool<T>.Release(buffer);
            return selection;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomItemsNoAlloc<T>(this IList<T> source, int count, ref List<T> results)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(results);
            Assert.IsTrue(source.Count >= count,
                $"Cannot select {count} random items from a collection with {source.Count} items!");

            results.Clear();

            while (results.Count < count)
            {
                results.Add(source[Range(0, source.Count)]);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomUniqueItemsNoAlloc<T>(this IList<T> source, ref List<T> results, int count)
        {
            Assert.IsNotNull(source);
            Assert.IsNotNull(results);
            Assert.IsTrue(source.Count >= count,
                $"Cannot select {count} random items from a collection with {source.Count} items!");

            results.Clear();
            var buffer = ListPool<T>.Get();
            buffer.AddRange(source);

            // Fisher-Yates shuffle
            for (var index = buffer.Count - 1; index > 0; index--)
            {
                var randomIndex = Range(0, index + 1);

                (buffer[index], buffer[randomIndex]) = (buffer[randomIndex], buffer[index]);
            }

            for (var i = 0; i < count; i++)
            {
                results.Add(buffer[i]);
            }

            ListPool<T>.Release(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T RandomItemWeighted<T>(this IReadOnlyList<T> source) where T : IWeighted
        {
            var totalWeight = 0f;
            foreach (var weightedElement in source)
            {
                totalWeight += weightedElement.Weight;
            }

            var randomWeight = Float(0, totalWeight);
            var weight = 0f;
            foreach (var weightedElement in source)
            {
                weight += weightedElement.Weight;
                if (weight >= randomWeight)
                {
                    return weightedElement;
                }
            }

            return source.RandomItem();
        }

        #endregion
    }
}
