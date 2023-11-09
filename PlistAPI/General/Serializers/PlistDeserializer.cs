using PlistAPI.Exceptions;
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
        public static T? DeserializeProperties<T>(Plist plist, IEnumerable<PropertyInfo> properties)
        {
            var instance = Activator.CreateInstance<T>();

            foreach (var prop in properties)
            {
                var id = prop.GetPropertyId();
                var type = prop.PropertyType.GetValueContainerType();

                object? value;

                if (type == Enums.PlistValueContainerType.Basic)
                    value = DeserializeBasic(plist, id);

                else if (type == Enums.PlistValueContainerType.Dict)
                    value = DeserializeDict(prop, plist);

                else if (type == Enums.PlistValueContainerType.Collection)
                    value = DeserializeCollection(prop, plist);

                else
                    return plist.Settings.InvalidDataHandlingType.ThrowException() ? throw new InvalidDataException(nameof(type)) : default(T?);

                // boxing struct
                if (typeof(T).IsValueType)
                {
                    object? boxed = instance;
                    prop.SetValue(boxed, value);
                    instance = (T?)boxed;
                }

                // setting value
                else
                    prop.SetValue(instance, value);
            }

            return instance;
        }

        private static object DeserializeDict(PropertyInfo property, Plist plist)
        {
            var id = property.GetPropertyId();
            var nestedPlist = DeserializeBasic(plist, id);

            return typeof(PlistDeserializer)
                .GetMethod(nameof(DeserializeProperties), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(property.PropertyType)
                .Invoke(null, new object[] { (Plist)nestedPlist, PlistHelper.GetPlistProperties(property.PropertyType) });
        }

        private static object DeserializeCollection(PropertyInfo property, Plist plist)
        {
            var id = property.GetPropertyId();
            var value = DeserializeBasic(plist, id);

            Type elementType =
                property.PropertyType.IsArray ?
                property.PropertyType.GetElementType() :
                property.PropertyType.IsGenericType ?
                property.PropertyType.GetGenericArguments().First() :
                throw new InvalidDataException(nameof(property.PropertyType));

            var enumerable = (IEnumerable)value;
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType);

            foreach (var element in enumerable)
            {
                if (element is Plist plistElement)
                {
                    var parsedElement = typeof(PlistDeserializer)
                        .GetMethod(nameof(DeserializeProperties), BindingFlags.Static | BindingFlags.Public)
                        .MakeGenericMethod(elementType)
                        .Invoke(null, new object[] { plistElement, PlistHelper.GetPlistProperties(elementType) });

                    list.Add(parsedElement);
                }
                else
                {
                    list.Add(element);
                }
            }

            if (property.PropertyType.IsArray)
            {
                var array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);
                value = array;
            }
            else
                value = list;

            return value;
        }

        private static object? DeserializeBasic(Plist plist, string id)
        {
            plist.TryGetValue(id, out object? value);

            if (value is null)
                return plist.Settings.InvalidDataHandlingType.ThrowException() ? throw new CorruptedPlistException(nameof(id)) : null;

            return value;
        }
    }
}
