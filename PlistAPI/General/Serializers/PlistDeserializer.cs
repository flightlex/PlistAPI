using PlistAPI.Enums;
using PlistAPI.Exceptions;
using PlistAPI.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PlistAPI.General.Serializers
{
    internal static class PlistDeserializer
    {
        public static T? DeserializeProperties<T>(Plist plist, IEnumerable<MemberInfo> members)
        {
            var instance = Activator.CreateInstance<T>();

            foreach (var member in members)
            {
                var type = PlistHelper.GetValueContainerType(member);

                object? value;

                if (type == PlistValueContainerType.Basic)
                    value = DeserializeBasic(plist, member);

                else if (type == PlistValueContainerType.Dict)
                    value = DeserializeDict(plist, member);

                else if (type == PlistValueContainerType.Collection)
                    value = DeserializeCollection(plist, member);

                else
                    return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new InvalidDataException(nameof(type)) : default;

                // boxing struct
                if (typeof(T).IsValueType)
                {
                    object? boxed = instance;
                    member.SetValue(boxed, value);
                    instance = (T?)boxed;
                }

                // setting value
                else
                    member.SetValue(instance, value);
            }

            return instance;
        }

        private static object? DeserializeDict(Plist plist, MemberInfo member)
        {
            var id = PlistHelper.GetMemberId(member);
            var nestedPlist = DeserializeBasic(plist, member);

            return typeof(PlistDeserializer)
                .GetMethod(nameof(DeserializeProperties), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(member.GetFieldOrPropertyType())
                .Invoke(null, new object?[] { (Plist?)nestedPlist, PlistHelper.GetPlistPropertyMembers(member.GetFieldOrPropertyType()) });
        }

        private static object? DeserializeCollection(Plist plist, MemberInfo member)
        {
            var value = DeserializeBasic(plist, member);
            var type = member.GetFieldOrPropertyType();

            Type elementType =
                type.IsArray ?
                type.GetElementType() :
                type.IsGenericType ?
                type.GetGenericArguments().First() :
                throw new InvalidDataException(nameof(member));

            var enumerable = (IEnumerable?)value;
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            if (enumerable is null)
                return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new CorruptedPlistException(nameof(member)) : default;

            foreach (var element in enumerable)
            {
                if (element is Plist plistElement)
                {
                    var parsedElement = typeof(PlistDeserializer)
                        .GetMethod(nameof(DeserializeProperties), BindingFlags.Static | BindingFlags.Public)
                        .MakeGenericMethod(elementType)
                        .Invoke(null, new object[] { plistElement, PlistHelper.GetPlistPropertyMembers(elementType) });

                    list.Add(parsedElement);
                }
                else
                {
                    list.Add(element);
                }
            }

            if (member.GetFieldOrPropertyType().IsArray)
            {
                var array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);
                value = array;
            }
            else
                value = list;

            return value;
        }

        private static object? DeserializeBasic(Plist plist, MemberInfo member)
        {
            var id = PlistHelper.GetMemberId(member);
            plist.TryGetValue(id, out object? value);

            if (value is null)
                return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new CorruptedPlistException(nameof(id)) : default;

            value = PlistHelper.ConvertValue(plist, member, value, PlistOperation.Deserialization);

            return value;
        }
    }
}
