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
    internal static class PlistSerializer
    {
        public static Plist SerializeProperties<T>(T instance, Plist plist, IEnumerable<MemberInfo> members)
        {
            foreach (var member in members)
            {
                var id = PlistHelper.GetMemberId(member);
                var type = PlistHelper.GetValueContainerType(member);
                var value = member.GetValue(instance);

                if (type == PlistValueContainerType.Basic)
                    plist[id] = value;

                else if (type == PlistValueContainerType.Dict)
                    plist[id] = SerializeDict(plist, member, value);

                else if (type == PlistValueContainerType.Collection)
                    plist[id] = SerializeCollection(plist, member, value);

                else
                    return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new InvalidDataException(nameof(type)) : Plist.Empty;

                plist[id] = PlistHelper.ConvertValue(plist, member, plist[id], PlistOperation.Serialization);
            }

            return plist;
        }

        private static object SerializeDict(Plist plist, MemberInfo member, object? value)
        {
            return typeof(PlistSerializer)
                .GetMethod(nameof(SerializeProperties), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(member.GetFieldOrPropertyType())
                .Invoke(null, new object?[] { value, new Plist(plist.Settings), PlistHelper.GetPlistPropertyMembers(member.GetFieldOrPropertyType()) });
        }

        private static object? SerializeCollection(Plist plist, MemberInfo member, object? value)
        {
            var type = member.GetFieldOrPropertyType();

            Type elementType =
                type.IsArray ?
                type.GetElementType() :
                type.IsGenericType ?
                type.GetGenericArguments().First() :
                throw new InvalidDataException(nameof(member));

            var enumerable = (IEnumerable?)value;
            var list = new List<object?>();

            if (enumerable is null)
                return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new CorruptedPlistException(nameof(member)) : default;

            foreach (var element in enumerable)
            {
                if (PlistHelper.IsPlist(elementType))
                {
                    var serializedElement = typeof(PlistSerializer)
                        .GetMethod(nameof(SerializeProperties), BindingFlags.Static | BindingFlags.Public)
                        .MakeGenericMethod(elementType)
                        .Invoke(null, new object?[] { element, new Plist(plist.Settings), PlistHelper.GetPlistPropertyMembers(elementType) });

                    list.Add(serializedElement);
                }
                else
                {
                    list.Add(element);
                }
            }

            return list;
        }
    }
}
