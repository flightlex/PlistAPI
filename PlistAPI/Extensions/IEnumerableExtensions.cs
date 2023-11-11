using System;
using System.Collections.Generic;
using System.Text;

namespace PlistAPI.Extensions
{
    internal static class IEnumerableExtensions
    {
        // taken from https://github.com/Folleach/GeometryDashAPI/blob/master/GeometryDashAPI/Extensions.cs#L78
        public static IEnumerable<KeyValuePair<T, T>> Pairs<T>(this IEnumerable<T> source)
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
    }
}
