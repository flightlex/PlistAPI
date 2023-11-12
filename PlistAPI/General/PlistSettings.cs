using PlistAPI.Enums;
using System;
using System.Xml.Linq;

namespace PlistAPI.General
{
    public sealed class PlistSettings
    {
        private static XAttribute[] _defaultAttributes = new XAttribute[] { new XAttribute("version", "1.0") };

        /// <summary>
        /// Data handling type. Throw exception on serialization or deserialization mismatches or just return <see cref="default"/> or original provided object
        /// </summary>
        public PlistInvalidDataHandlingType InvalidDataHandlingType { get; set; } = PlistInvalidDataHandlingType.ThrowException;

        /// <summary>
        /// Input data type. Full, Short or Both at the same time
        /// </summary>
        public PlistDataType InputDataType { get; set; } = PlistDataType.Both;

        private PlistDataType _outputDataType = PlistDataType.Full;
        /// <summary>
        /// Output data type. Full or Short only
        /// </summary>
        public PlistDataType OutputDataType
        {
            get => _outputDataType;
            set
            {
                if (value == PlistDataType.Both)
                    throw new NotImplementedException("Strong type is supported only. Can't write both type at the same time.");
            }
        }

        /// <summary>
        /// Save options are the serialization formatting
        /// </summary>
        public SaveOptions SaveOptions { get; set; } = SaveOptions.None;

        /// <summary>
        /// Attributes that will be applied in a main Plist element
        /// </summary>
        public XAttribute[] RootAttributes { get; set; } = _defaultAttributes;

        /// <summary>
        /// Creates a new instance of settings with default values
        /// </summary>
        /// <returns></returns>
        public static PlistSettings DefaultSettings() => new();
    }
}
