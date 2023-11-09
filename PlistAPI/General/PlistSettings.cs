using PlistAPI.Enums;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace PlistAPI.General
{
    public sealed class PlistSettings
    {
        private static IReadOnlyDictionary<string, object> _defaultValues = new Dictionary<string, object>()
        {
            // default attributes
            { nameof(RootAttributes), new XAttribute[] { new XAttribute("version", "1.0") } },

            // default input data type
            { nameof(InputDataType), PlistDataType.Both },

            // default output data type
            { nameof(OutputDataType), PlistDataType.Full },

            // default save options
            { nameof(SaveOptions), SaveOptions.None },

            // default invalid data handling type
            { nameof(InvalidDataHandlingType), PlistInvalidDataHandlingType.ThrowException }
        };

        public PlistInvalidDataHandlingType InvalidDataHandlingType { get; set; } = (PlistInvalidDataHandlingType)_defaultValues[nameof(InvalidDataHandlingType)];
        public PlistDataType InputDataType { get; set; } = (PlistDataType)_defaultValues[nameof(InputDataType)];
        public PlistDataType OutputDataType { get; set; } = (PlistDataType)_defaultValues[nameof(OutputDataType)];
        public SaveOptions SaveOptions { get; set; } = (SaveOptions)_defaultValues[nameof(SaveOptions)];
        public XAttribute[] RootAttributes { get; set; } = (XAttribute[])_defaultValues[nameof(RootAttributes)];

        public PlistSettings()
        { }

        public PlistSettings(PlistDataType inputDataType, PlistDataType outputDataType)
            : this(inputDataType, outputDataType, (XAttribute[])_defaultValues[nameof(RootAttributes)])
        { }

        public PlistSettings(PlistDataType inputDataType, PlistDataType outputDataType, params XAttribute[] rootAttributes)
            : this(inputDataType, outputDataType, (SaveOptions)_defaultValues[nameof(SaveOptions)], (XAttribute[])_defaultValues[nameof(RootAttributes)])
        { }

        public PlistSettings(PlistDataType inputDataType, PlistDataType outputDataType, SaveOptions saveOptions)
            : this(inputDataType, outputDataType, saveOptions, (XAttribute[])_defaultValues[nameof(RootAttributes)])
        { }

        public PlistSettings(PlistDataType inputDataType, PlistDataType outputDataType, SaveOptions saveOptions, params XAttribute[] rootAttributes)
        {
            if (outputDataType == PlistDataType.Both)
                throw new InvalidOperationException("Cannot use 'Both' on output, strong type is supported only.");

            RootAttributes = rootAttributes;
            SaveOptions = saveOptions;
            InputDataType = inputDataType;
            OutputDataType = outputDataType;
        }

        public static PlistSettings DefaultSettings() => new();
    }
}
