using System;

namespace PlistAPI.Attributes
{
    /// <summary>
    /// Attribute that indicated thats the certain class is a Plist object and can be used for serialization and deserialization, attribute is necessary
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class PlistObjectAttribute : Attribute
    {
    }
}
