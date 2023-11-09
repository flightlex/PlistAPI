using PlistAPI.Attributes;
using PlistAPI.Enums;
using System;
using System.Collections.Generic;

namespace PlistAPI
{
    public static class Extensions
    {
        // taken from GeometryDashAPI, little refactored
        internal static IEnumerable<KeyValuePair<T, T>> Pairs<T>(this IEnumerable<T> source)
        {
            var first = false;
            T firstItem = default;

            foreach (var item in source)
            {
                first = !first;

                if (first)
                    firstItem = item;
                else
                    yield return new KeyValuePair<T, T>(firstItem, item);
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (first)
                yield return new KeyValuePair<T, T>(firstItem, default);
        }

        public static bool IsAssignableTo(this Type type, Type assignType)
        {
            return assignType.IsAssignableFrom(type);
        }

        public static bool ThrowException(this PlistInvalidDataHandlingType type)
            => type == PlistInvalidDataHandlingType.ThrowException;
    }
}
