using System;

namespace PlistAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class PlistObjectAttribute : Attribute
    {
    }
}
