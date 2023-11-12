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
        // Serializes the members
        public static Plist SerializeMembers<T>(T instance, Plist plist, IEnumerable<MemberInfo> members)
        {
            // enumerates each member
            foreach (var member in members)
            {
                // gets member path from PlistPropertyAttribute
                var pathOrId = PlistHelper.GetMemberPathOrId(member);

                // gets value container type
                var type = PlistHelper.GetValueContainerType(member);

                // obtains value from the object instance
                var value = member.GetValue(instance);

                // temp object
                object? lastObject = plist;

                // writing this long path
                foreach (var id in pathOrId)
                {
                    lastObject = ((Plist)lastObject).TryGetValue(id, out lastObject);

                    if (lastObject is null)
                        break;

                    // checking if next object is not plist, not to go too far
                    ((Plist)lastObject).TryGetValue(id, out var tempObject);
                    if (tempObject is not Plist)
                        break;
                }

                // resulting plist
                Plist? lastPlist = (Plist?)lastObject;

                // everything excepts <dict /> and <array />
                if (type == PlistValueContainerType.Basic)
                    lastPlist[pathOrId.Last()] = value;

                // <dict /> type
                else if (type == PlistValueContainerType.Dict)
                    lastPlist[pathOrId.Last()] = SerializeDict(plist, member, value);

                // <array /> type
                else if (type == PlistValueContainerType.Collection)
                    lastPlist[pathOrId.Last()] = SerializeCollection(plist, member, value);

                // checks setting selection
                else
                    return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new InvalidDataException(nameof(type)) : Plist.Empty;

                // converts value if it has PlistConverterAttribute
                lastPlist[pathOrId.Last()] = PlistHelper.ConvertValue(plist, member, lastPlist[pathOrId.Last()], PlistOperation.Serialization);
            }

            return plist;
        }

        private static object SerializeDict(Plist plist, MemberInfo member, object? value)
        {
            // caches SerializeMembers method
            var method = ReflectionContainers.GetOrCreateMethod(
                nameof(SerializeMembers),
                delegate
                {
                    return typeof(PlistSerializer).GetMethod(nameof(SerializeMembers), BindingFlags.Static | BindingFlags.Public);
                });

            return method
                .MakeGenericMethod(member.GetFieldOrPropertyType())
                .Invoke(null, new object?[] { value, new Plist(plist.Settings), PlistHelper.GetPlistPropertyMembers(member.GetFieldOrPropertyType()) });
        }

        private static object? SerializeCollection(Plist plist, MemberInfo member, object? value)
        {
            // gets PropertyInfo.PropertyType or FieldInfo.FieldType
            var type = member.GetFieldOrPropertyType();

            // checks whether member is IEnumerable<object> or object[]
            Type elementType =
                type.IsArray ?
                type.GetElementType() :
                type.IsGenericType ?
                type.GetGenericArguments().First() :
                throw new InvalidDataException(nameof(member));

            // creates List<object?> instance
            var enumerable = (IEnumerable?)value;
            var list = new List<object?>();

            // checks for null
            if (enumerable is null)
                return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new CorruptedPlistException(nameof(member)) : default;

            // enumerates each element
            foreach (var element in enumerable)
            {
                // checks for Plist object
                if (PlistHelper.IsPlist(elementType))
                {
                    // caches SerializeMembers method
                    var method = ReflectionContainers.GetOrCreateMethod(
                        nameof(SerializeMembers),
                        delegate
                        {
                            return typeof(PlistSerializer).GetMethod(nameof(SerializeMembers), BindingFlags.Static | BindingFlags.Public);
                        });

                    var serializedElement = method
                        .MakeGenericMethod(elementType)
                        .Invoke(null, new object?[] { element, new Plist(plist.Settings), PlistHelper.GetPlistPropertyMembers(elementType) });

                    list.Add(serializedElement);
                }

                // basic value
                else
                {
                    list.Add(element);
                }
            }

            return list;
        }
    }
}
        