using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace PlistAPI
{
    internal static class ReflectionContainers
    {
        private static readonly Dictionary<string, MethodInfo> _cachedMethods = new();

        public static MethodInfo GetOrCreateMethod(string methodName, Func<MethodInfo> getMethodFunc)
            => GetOrCreate<MethodInfo>(methodName, getMethodFunc, _cachedMethods);

        private static T GetOrCreate<T>(string memberName, Func<T> getMemberFunc, IDictionary dictionary, bool forceContains = false)
        {
            if (forceContains || dictionary.Contains(memberName))
                return (T)dictionary[memberName];

            dictionary.Add(memberName, getMemberFunc());
            return GetOrCreate<T>(memberName, getMemberFunc, dictionary, true);
        }
    }
}
