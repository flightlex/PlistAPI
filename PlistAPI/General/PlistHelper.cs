using PlistAPI.Attributes;
using PlistAPI.Enums;
using PlistAPI.Exceptions;
using PlistAPI.Extensions;
using PlistAPI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlistAPI.General
{
    internal static class PlistHelper
    {
        public static void CheckForObjectAssignation<T>()
        {
            if (typeof(T).GetCustomAttribute<PlistObjectAttribute>() is null)
                throw new PlistObjectNotAssignedException(nameof(T));
        }

        public static IEnumerable<MemberInfo> GetPlistPropertyMembers<T>()
            => GetPlistPropertyMembers(typeof(T));

        public static IEnumerable<MemberInfo> GetPlistPropertyMembers(Type type)
        {
            return type
                .GetMembers()
                .Where(x => (x.MemberType == MemberTypes.Field || x.MemberType == MemberTypes.Property)
                && x.GetCustomAttribute<PlistPropertyAttribute>() is not null);
        }

        public static bool IsPlist(Type type)
        {
            return type.GetCustomAttribute<PlistObjectAttribute>() is not null;
        }

        public static bool IsCollection(Type type)
        {
            return type.IsAssignableTo(typeof(ICollection)) || (type.IsAssignableTo(typeof(IEnumerable)) && type.IsGenericType);
        }

        public static string[] GetMemberPathOrId(MemberInfo member)
        {
            var attr = member.GetCustomAttribute<PlistPropertyAttribute>();

            return attr!.PathOrId ?? new string[1] { member.Name };
        }

        public static PlistValueContainerType GetValueContainerType(MemberInfo member)
        {
            var type = member.GetFieldOrPropertyType();

            if (IsPlist(type))
                return PlistValueContainerType.Dict;

            else if (IsCollection(type))
                return PlistValueContainerType.Collection;

            else
                return PlistValueContainerType.Basic;
        }

        public static PlistValueType? GetValueType(string input, PlistDataType dataType)
        {
            PlistValueType? result = null;
            if (dataType.HasFlag(PlistDataType.Full))
                switch (input)
                {
                    case "string": result = PlistValueType.String; break;
                    case "integer": result = PlistValueType.Integer; break;
                    case "real": result = PlistValueType.Real; break;
                    case "true": result = PlistValueType.True; break;
                    case "false": result = PlistValueType.False; break;
                    case "dict": result = PlistValueType.Dict; break;
                    case "array": result = PlistValueType.Array; break;
                }

            if (result is not null)
                return result;

            if (dataType.HasFlag(PlistDataType.Short))
                switch (input)
                {
                    case "s": result = PlistValueType.String; break;
                    case "i": result = PlistValueType.Integer; break;
                    case "r": result = PlistValueType.Real; break;
                    case "t": result = PlistValueType.True; break;
                    case "f": result = PlistValueType.False; break;
                    case "d": result = PlistValueType.Dict; break;
                    case "a": result = PlistValueType.Array; break;
                }

            return result;
        }

        public static object? ConvertValue(Plist plist, MemberInfo member, object? value, PlistOperation operation)
        {
            var attr = member.GetCustomAttribute<PlistConverterAttribute>();
            var converterType = attr?.ConverterType;
            var defaultValue = attr?.DefaultValue;

            var customMembers = member.GetCustomAttributes(false)?
                .AsQueryable()
                .Where(x => x is PlistConverterMemberAttribute)
                .Select(x => (PlistConverterMemberAttribute)x)
                .AsEnumerable();

            bool throwException = plist.Settings.InvalidDataHandlingType.IsThrowException();
            bool useCustomMembers = customMembers is not null && customMembers!.Count() > 0;

            // no converter
            if (attr is null)
                return value;

            // value is null
            if (value is null)
                return throwException ? throw new InvalidOperationException(nameof(value)) : defaultValue;

            // input data
            if (operation == PlistOperation.Deserialization)
                if (!attr.ConverterUsage.HasFlag(PlistConverterUsage.ConvertInputType))
                    return defaultValue;

            // output data
            if (operation == PlistOperation.Serialization)
                if (!attr.ConverterUsage.HasFlag(PlistConverterUsage.ConvertOutputType))
                    return defaultValue;

            // type is null
            if (converterType is null)
                return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new InvalidOperationException(nameof(converterType)) : value;

            var converterInterface = converterType.GetInterface(typeof(IPlistConverter<,>).FullName);

            // doesnt implement the interface
            if (converterInterface is null)
                throw new InvalidOperationException(nameof(value)); // exception only, because the person uses the converter attribute but doesnt perform the requirements

            var method = converterInterface.GetMethod(
                operation == PlistOperation.Deserialization
                    ? nameof(IPlistConverter<object, object>.ReadValue)
                    : nameof(IPlistConverter<object, object>.WriteValue));

            var genericArguments = converterInterface.GetGenericArguments();

            // cached converters' instances from a cached method (double cache huh)
            var cachedConverterMethod = ReflectionContainers.GetOrCreateMethod(
                nameof(Converters.GetConverter),
                delegate
                {
                    return typeof(Converters).GetMethod(nameof(Converters.GetConverter));
                });

            var converterInstance = cachedConverterMethod
                .MakeGenericMethod(
                    genericArguments[(int)PlistOperation.Deserialization],
                    genericArguments[(int)PlistOperation.Serialization]
                ).Invoke(null, new object[] { converterType, useCustomMembers });

            // setting values for addition
            if (useCustomMembers)
                SetValueForConverterMembers(converterInstance, converterType!, customMembers);

            if (value.GetType() != genericArguments[(int)operation])
                return throwException ? throw new InvalidOperationException(nameof(value)) : defaultValue;

            var outValue = method.Invoke(converterInstance, new object[] { value });
            return outValue;
        }

        private static void SetValueForConverterMembers(object? converter, Type converterType, IEnumerable<PlistConverterMemberAttribute> customMembers)
        {
            var members = converterType
                .GetMembers()
                .Where(x => 
                    (x.MemberType == MemberTypes.Property || x.MemberType == MemberTypes.Field)
                    && x.GetCustomAttribute<PlistConverterPropertyAttribute>() is not null);

            if (members.Count() < 1)
                return;

            foreach (var customMember in customMembers!)
            {
                var member = converterType.GetMember(customMember.MemberName).First();
                var attr = member.GetCustomAttribute<PlistConverterPropertyAttribute>();
                var defaultValue = attr.DefaultValue;

                var value = customMember.Value ?? defaultValue;

                member.SetValue(converter, value);
            }
        }
    }
}
