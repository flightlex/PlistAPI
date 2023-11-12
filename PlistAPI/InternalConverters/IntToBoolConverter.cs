using PlistAPI.Attributes;
using PlistAPI.Interfaces;

namespace PlistAPI.InternalConverters
{
    /// <summary>
    /// Internal converter that converts value greater than <see cref="0"/> to <see cref="true"/>, and less than <see cref="1"/> to <see cref="false"/>
    /// </summary>
    public sealed class IntToBoolConverter : IPlistConverter<int, bool>
    {
        [PlistConverterProperty(false)]
        public bool ReverseValue { get; set; }

        public bool ReadValue(int value)
        {
            return (value > 0) ^ ReverseValue;
        }

        public int WriteValue(bool value)
        {
            return value ^ ReverseValue ? 1 : 0;
        }
    }
}
