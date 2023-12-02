﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MobX.Utilities
{
    public static partial class ReflectionExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetCustomAttribute<T>(this ICustomAttributeProvider provider, out T attribute,
            bool inherited = false)
            where T : Attribute
        {
            var attributes = provider.GetCustomAttributes(inherited);

            foreach (var element in attributes)
            {
                if (element is T customAttribute)
                {
                    attribute = customAttribute;
                    return true;
                }
            }

            attribute = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> TryGetCustomAttributes<T>(this ICustomAttributeProvider provider,
            bool inherited = false)
            where T : Attribute
        {
            var attributes = provider.GetCustomAttributes(inherited);
            foreach (var item in attributes)
            {
                if (item is T attribute)
                {
                    yield return attribute;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, out T attribute, bool inherited = false)
            where T : Attribute
        {
            var found = memberInfo.GetCustomAttribute<T>(inherited);

            if (found != null)
            {
                attribute = found;
                return true;
            }

            attribute = null;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this ICustomAttributeProvider provider, bool inherited = true)
            where T : Attribute
        {
            try
            {
                return provider.IsDefined(typeof(T), inherited);
            }
            catch (MissingMethodException)
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttributes<T>().Any();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LacksAttribute<T>(this ICustomAttributeProvider provider, bool inherited = true)
            where T : Attribute
        {
            try
            {
                return !provider.IsDefined(typeof(T), inherited);
            }
            catch (MissingMethodException)
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LacksAttribute<T>(this MemberInfo memberInfo) where T : Attribute
        {
            return memberInfo.GetCustomAttribute<T>() == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType
                .GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}