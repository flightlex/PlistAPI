using System;
using System.Reflection;

namespace PlistAPI.Extensions
{
    internal static class ReflectionExtensions
    {
        // Reversed method of <Type.IsAssignableFrom(Type, Type)>
        public static bool IsAssignableTo(this Type type, Type type2)
            => type2.IsAssignableFrom(type);

        // Extension method for <MemberInfo> to set value
        public static void SetValue(this MemberInfo member, object? obj, object? value)
        {
            if (member.MemberType == MemberTypes.Field)
                ((FieldInfo)member).SetValue(obj, value);

            else if (member.MemberType == MemberTypes.Property)
                ((PropertyInfo)member).SetValue(obj, value);

            else
                throw new InvalidOperationException(nameof(member));
        }

        // Extension method for <MemberInfo> to get value
        public static object GetValue(this MemberInfo member, object? obj)
        {
            if (member.MemberType == MemberTypes.Field)
                return ((FieldInfo)member).GetValue(obj);

            else if (member.MemberType == MemberTypes.Property)
                return ((PropertyInfo)member).GetValue(obj);

            throw new InvalidOperationException(nameof(member));
        }

        // Extension method for <MemberInfo> to get type
        public static Type GetFieldOrPropertyType(this MemberInfo member)
        {
            return member.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)member).FieldType,
                MemberTypes.Property => ((PropertyInfo)member).PropertyType,

                _ => throw new NotSupportedException(nameof(member))
            };
        }
    }
}
