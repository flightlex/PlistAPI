using PlistAPI.Enums;
using System;

namespace PlistAPI.Attributes
{
    /// <summary>
    /// Attribute to declare that the desired member will give or take converted value, converter have to be implemented by creating class with inheriting <see cref="Interfaces.IPlistConverter{TInput, TOutput}"/>
    /// Also you can use internal converters that are written by the author <seealso cref="InternalConverters"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class PlistConverterAttribute : Attribute
    {
        /// <summary>
        /// PlistConverter selectively converts input or output data when serializing or deserializing
        /// </summary>
        /// <param name="converterType">Type of converter class that inherites <see cref="Interfaces.IPlistConverter{TInput, TOutput}" /></param>
        public PlistConverterAttribute(Type converterType)
        {
            DefaultValue = default;
            ConverterType = converterType;
        }

        /// <summary>
        /// PlistConverter selectively converts input or output data when serializing or deserializing
        /// </summary>
        /// <param name="converterType">Type of converter class that inherites <see cref="Interfaces.IPlistConverter{TInput, TOutput}" /></param>
        /// <param name="defaultValue">Value that will be applied if the input value is null or if the expected type mismatches actual one</param>
        public PlistConverterAttribute(Type converterType, object? defaultValue)
        {
            DefaultValue = defaultValue;
            ConverterType = converterType;
        }

        /// <summary>
        /// Value that will be applied if the input value is null or if the expected type mismatches actual one
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// Type of converter class that inherites <see cref="Interfaces.IPlistConverter{TInput, TOutput}" />
        /// </summary>
        public Type ConverterType { get; set; }

        /// <summary>
        /// ConverterUsage determines whether convert input data (from the raw data), or output data (from code objects to data)
        /// </summary>
        public PlistConverterUsage ConverterUsage { get; set; } = PlistConverterUsage.ConvertBothTypes;
    }
}
