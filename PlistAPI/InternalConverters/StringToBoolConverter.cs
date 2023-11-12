using PlistAPI.Attributes;
using PlistAPI.Interfaces;

namespace PlistAPI.InternalConverters
{
    /// <summary>
    /// Internal converter that is basically the same as <see cref="IntToBoolConverter"/>, but it accepts strings instead of integers
    /// </summary>
    public sealed class StringToBoolConverter : IPlistConverter<string, bool>
    {
        [PlistConverterProperty(false)]
        public bool ReverseValue { get; set; }

        public bool ReadValue(string value)
        {
            return (value.ToInt() > 0) ^ ReverseValue;
        }

        public string WriteValue(bool value)
        {
            return (value ^ ReverseValue ? 1 : 0).ToString();
        }
    }
}
