using System;

namespace PlistAPI.Attributes
{
    /// <summary>
    /// Attribute that indicated that the current property or field is a Plist property and can be used to store value, should be used in a class with <see cref="PlistObjectAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class PlistPropertyAttribute : Attribute
    {
        public PlistPropertyAttribute()
        {
            PathOrId = null;
        }

        /// <summary>
        /// Id is a name of a Key in a .plist data, its case-sensitive
        /// </summary>
        /// <param name="id"></param>
        public PlistPropertyAttribute(string id)
        {
            PathOrId = new string[1] { id };
        }

        /// <summary>
        /// Path is generally the same as Id, but it will be trying to reach the value using the path even if it will need to go output the Parent Plist. Each param is case-sensitivty
        /// </summary>
        /// <param name="path"></param>
        public PlistPropertyAttribute(params string[] path)
        {
            PathOrId = path;
        }

        /// <summary>
        /// The path to the desired value
        /// </summary>
        public string[]? PathOrId { get; }
    }
}
