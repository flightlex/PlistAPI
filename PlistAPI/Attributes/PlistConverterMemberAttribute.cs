using System;

namespace PlistAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public sealed class PlistConverterMemberAttribute : Attribute
    {
        public PlistConverterMemberAttribute(string memberName, object? value)
        {
            MemberName = memberName;
            Value = value;
        }

        public string MemberName { get; set; }
        public object? Value { get; set; }
    }
}
