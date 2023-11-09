using PlistAPI.Attributes;
using PlistAPI.Enums;
using PlistAPI.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Xml.Linq;

namespace PlistAPI.General
{
    internal static class PlistHelper
    {
        public static void CheckForObjectAssignation<T>()
        {
            if (typeof(T).GetCustomAttribute<PlistObjectAttribute>() is null)
                throw new PlistObjectNotAssignedException(nameof(T));
        }

        public static IEnumerable<PropertyInfo> GetPlistProperties<T>()
            => GetPlistProperties(typeof(T));

        public static IEnumerable<PropertyInfo> GetPlistProperties(Type type)
        {
            return type
                .GetProperties()
                .Where(x => x.GetCustomAttribute<PlistPropertyAttribute>() is not null);
        }

        public static bool IsPropertyNested(Type type)
        {
            return type.GetCustomAttribute<PlistObjectAttribute>() is not null;
        }

        public static bool IsCollection(Type type)
        {
            return type.IsAssignableTo(typeof(ICollection)) || type.IsAssignableTo(typeof(IEnumerable)) && type.IsGenericType;
        }

        public static string GetPropertyId(this PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<PlistPropertyAttribute>();
            var id = attr!.Id ?? prop.Name;

            return id;
        }

        public static PlistValueContainerType GetValueContainerType(this Type prop)
        {
            if (IsPropertyNested(prop))
                return PlistValueContainerType.Dict;

            else if (IsCollection(prop))
                return PlistValueContainerType.Collection;

            else
                return PlistValueContainerType.Basic;
        }

        public static PlistValueType GetValueType(string input, PlistDataType dataType)
        {
            if (dataType == PlistDataType.Full)
                return input switch
                {
                    "string" => PlistValueType.String,
                    "integer" => PlistValueType.Integer,
                    "real" => PlistValueType.Real,
                    "true" => PlistValueType.True,
                    "false" => PlistValueType.False,
                    "dict" => PlistValueType.Dict,
                    "array" => PlistValueType.Array,

                    _ => throw new InvalidDataException("Unsupported element type")
                };

            else if (dataType == PlistDataType.Short)
                return input switch
                {
                    "s" => PlistValueType.String,
                    "i" => PlistValueType.Integer,
                    "r" => PlistValueType.Real,
                    "t" => PlistValueType.True,
                    "f" => PlistValueType.False,
                    "d" => PlistValueType.Dict,
                    "a" => PlistValueType.Array,

                    _ => throw new InvalidDataException("Unsupported element type")
                };

            else if (dataType == PlistDataType.Both)
                return input switch
                {
                    "s" or "string" => PlistValueType.String,
                    "i" or "integer" => PlistValueType.Integer,
                    "r" or "real" => PlistValueType.Real,
                    "t" or "true" => PlistValueType.True,
                    "f" or "false" => PlistValueType.False,
                    "d" or "dict" => PlistValueType.Dict,
                    "a" or "array" => PlistValueType.Array,

                    _ => throw new InvalidDataException("Unsupported element type")
                };

            throw new InvalidOperationException(nameof(dataType));
        }
    }
}
