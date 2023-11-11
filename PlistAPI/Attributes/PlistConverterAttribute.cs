using PlistAPI.Enums;
using System;

namespace PlistAPI.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public sealed class PlistConverterAttribute : Attribute
    {
        public PlistConverterAttribute(Type converterType)
        {
            DefaultValue = default;
            ConverterType = converterType;
        }

        public PlistConverterAttribute(Type converterType, object? defaultValue)
        {
            DefaultValue = defaultValue;
            ConverterType = converterType;
        }

        public object? DefaultValue { get; set; }
        public Type ConverterType { get; set; }
        public PlistConverterUsage ConverterUsage { get; set; } = PlistConverterUsage.BothTypes;
    }
}
