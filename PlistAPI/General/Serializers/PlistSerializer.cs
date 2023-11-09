using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PlistAPI.General.Serializers
{
    internal static class PlistSerializer
    {
        public static Plist SerializeProperties<T>(T instance, Plist plist, IEnumerable<PropertyInfo> properties)
        {
            foreach (var prop in properties)
            {
                var id = prop.GetPropertyId();
                var type = prop.PropertyType.GetValueContainerType();
                var value = prop.GetValue(instance);

                if (type == Enums.PlistValueContainerType.Basic)
                    plist[id] = value;

                else if (type == Enums.PlistValueContainerType.Dict)
                    plist[id] = SerializeDict(prop, plist, value);

                else if (type == Enums.PlistValueContainerType.Collection)
                    plist[id] = SerializeCollection(prop, plist, value);

                else
                    return plist.Settings.InvalidDataHandlingType.ThrowException() ? throw new InvalidDataException(nameof(type)) : Plist.Empty;
            }

            return plist;
        }

        private static object SerializeDict(PropertyInfo property, Plist plist, object value)
        { 
            return typeof(PlistSerializer)
                .GetMethod(nameof(SerializeProperties), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(property.PropertyType)
                .Invoke(null, new object[] { value, new Plist(plist.Settings), PlistHelper.GetPlistProperties(property.PropertyType) });
        }

        private static object SerializeCollection(PropertyInfo property, Plist plist, object value)
        {
            Type elementType =
                property.PropertyType.IsArray ?
                property.PropertyType.GetElementType() :
                property.PropertyType.IsGenericType ?
                property.PropertyType.GetGenericArguments().First() :
                throw new InvalidDataException(nameof(property.PropertyType));

            var enumerable = (IEnumerable)value;
            var list = new List<object>();

            foreach (var element in enumerable)
            {
                if (PlistHelper.IsPropertyNested(elementType))
                {
                    var serializedElement = typeof(PlistSerializer)
                        .GetMethod(nameof(SerializeProperties), BindingFlags.Static | BindingFlags.Public)
                        .MakeGenericMethod(elementType)
                        .Invoke(null, new object[] { element, new Plist(plist.Settings), PlistHelper.GetPlistProperties(elementType) });

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
