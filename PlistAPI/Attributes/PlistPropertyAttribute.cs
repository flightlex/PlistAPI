using System;

namespace PlistAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class PlistPropertyAttribute : Attribute
    {
        public PlistPropertyAttribute()
        {
            Id = null;
        }

        public PlistPropertyAttribute(string id)
        {
            Id = id;
        }

        public string? Id { get; }
    }
}
