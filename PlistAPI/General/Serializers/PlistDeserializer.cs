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
        // Deserializes members
        public static T? DeserializeMembers<T>(Plist plist, IEnumerable<MemberInfo> members)
        {
            // creates T instance
            var instance = Activator.CreateInstance<T>();

            // enumerates each member
            foreach (var member in members)
            {
                // gets the value container type
                var type = PlistHelper.GetValueContainerType(member);

                // resulting value
                object? value;

                // everything excepts <dict /> and <array />
                if (type == PlistValueContainerType.Basic)
                    value = DeserializeBasic(plist, member);

                // <dict />
                else if (type == PlistValueContainerType.Dict)
                    value = DeserializeDict(plist, member);

                // <array />
                else if (type == PlistValueContainerType.Collection)
                    value = DeserializeCollection(plist, member);

                // checks setting selection
                else
                    return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new InvalidDataException(nameof(type)) : default;

                // boxes struct
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
            // gets member path from PlistPropertyAttribute
            var id = PlistHelper.GetMemberPathOrId(member);

            // gets Plist object
            var nestedPlist = DeserializeBasic(plist, member);

            // caches DeserializeMembers method
            var mathod = ReflectionContainers.GetOrCreateMethod(
                nameof(DeserializeMembers),
                delegate
                {
                    return typeof(PlistDeserializer).GetMethod(nameof(DeserializeMembers), BindingFlags.Static | BindingFlags.Public);
                });

            // invokes
            return mathod
                .MakeGenericMethod(member.GetFieldOrPropertyType())
                .Invoke(null, new object?[] { (Plist?)nestedPlist, PlistHelper.GetPlistPropertyMembers(member.GetFieldOrPropertyType()) });
        }

        private static object? DeserializeCollection(Plist plist, MemberInfo member)
        {
            // gets IEnumerable<object> object
            var value = DeserializeBasic(plist, member);

            // gets PropertyInfo.PropertyType or FieldInfo.FieldType
            var type = member.GetFieldOrPropertyType();

            // checks whether member is IEnumerable<object> or object[]
            Type elementType =
                type.IsArray ?
                type.GetElementType() :
                type.IsGenericType ?
                type.GetGenericArguments().First() :
                throw new InvalidDataException(nameof(member));

            // creates enumerable
            var enumerable = (IEnumerable?)value;

            // creates List<T> instance
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            // checks for null
            if (enumerable is null)
                return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new CorruptedPlistException(nameof(member)) : default;

            // enumerates each element
            foreach (var element in enumerable)
            {
                // checks for Plist object
                if (element is Plist plistElement)
                {
                    // caches DeserializeMembers method
                    var method = ReflectionContainers.GetOrCreateMethod(
                        nameof(DeserializeMembers),
                        delegate
                        {
                            return typeof(PlistDeserializer).GetMethod(nameof(DeserializeMembers), BindingFlags.Static | BindingFlags.Public);
                        });

                    // gets the value
                    var parsedElement = method
                        .MakeGenericMethod(elementType)
                        .Invoke(null, new object[] { plistElement, PlistHelper.GetPlistPropertyMembers(elementType) });

                    list.Add(parsedElement);
                }

                // basic value
                else
                {
                    list.Add(element);
                }
            }
            
            // converts to array object if its an array
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
            // gets member path from PlistPropertyAttribute
            var pathOrId = PlistHelper.GetMemberPathOrId(member);

            // initializes temp object that will be used for reaching certain path
            object? tempObject = plist;

            // enumerates each element
            foreach (var item in pathOrId)
            {
                // checks for Plist object
                if (tempObject as Plist is null)
                    return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new CorruptedPlistException(nameof(pathOrId)) : default;

                // tries to get a value
                (tempObject as Plist)?.TryGetValue(item, out tempObject);
            }

            // converts a value if it has PlistConverterAttribute
            tempObject = PlistHelper.ConvertValue(plist, member, tempObject, PlistOperation.Deserialization);

            return tempObject;
        }
    }
}
