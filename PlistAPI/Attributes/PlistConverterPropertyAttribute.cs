using System;

namespace PlistAPI.Attributes
{
    /// <summary>
    /// Attribute that indicated that desired member is an additional member of a converter class, to make it work, reference it in the <see cref="PlistConverterAttribute.CustomMembers"/>
    /// </summary>
    /// <example>
    /// <code>
    /// // ... here should be a PlistPropertyAttribute
    /// [PlistConverter(typeof(ConverterType), CustomMembers: (nameof(ConverterType.MemberName), "string value as example"))]
    /// // ... here should be a property or a field
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class PlistConverterPropertyAttribute : Attribute
    {
        public PlistConverterPropertyAttribute()
        {
            DefaultValue = null;
        }

        public PlistConverterPropertyAttribute(object? defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Default value that will be applied once the external value used in <see cref="PlistConverterAttribute.CustomMembers"/> is null
        /// </summary>
        public object? DefaultValue { get; set; }
    }
}
