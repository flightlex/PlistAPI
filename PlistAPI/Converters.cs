using PlistAPI.Interfaces;
using PlistAPI.Interfaces.BaseInterfaces;
using System;
using System.Collections.Generic;

namespace PlistAPI
{
    internal static class Converters
    {
        private static readonly Dictionary<Type, IPlistConverter> _cachedConverters = new();

        public static IPlistConverter<TInput, TOutput> GetConverter<TInput, TOutput>(Type type)
        {
            if (_cachedConverters.ContainsKey(type))
                return (IPlistConverter<TInput, TOutput>)_cachedConverters[type];

            var converter = (IPlistConverter<TInput, TOutput>)Activator.CreateInstance(type);
            _cachedConverters.Add(type, converter);

            return converter;
        }
    }
}
