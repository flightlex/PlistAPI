# PlistAPI
`NuGet Package Link:` https://www.nuget.org/packages/PlistAPI

PlistAPI - is a library that allows you `deserialize` and `serialize` the data as specific objects, and probably the best Plist library you will ever find in the github for `C#`, so let me show you an example of it's usage:

## `.plist` (basically a little changed .xml) format
```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
     <dict>
          <key>SomeKey</key>
          <string>SomeValue</string>
     </dict>
</plist>
```

## Deserialization
You may provide `Id` in the attribute argument, or even a complete path to the key (allowed to get outside parent `Plist`)

```csharp
using var fileStream = File.Open("filePath");
Plist.Deserialize<SomeClass>(fileStream);

[PlistObject]
class SomeClass
{
    [PlistProperty("SomeKey")]
    // [PlistProperty("SomeDict", "SomeAnotherDict", "SomeValue")] // complete path
    public string SomeKey { get; set; }
}
```

Also you are able not to provide `PlistProperty.Id` *( [PlistProperty(string: Id)] )*, but the property name **must** match the `Key` **(Be careful, its case-sensitive, by providing `id` either)**.

## Serialization
```csharp
// deserialize
using var fileStream = File.Open("filePath");
var deserialized = Plist.Deserialize<SomeClass>(fileStream);

// serialize
Plist.Serialize<SomeClass>(deserialized);

// OR
// Plist.Serialize(deserialized);

[PlistObject]
class SomeClass
{
    [PlistProperty("SomeKey")]
    public string SomeKey { get; set; }
}
```

By **NOT** providing the type in the generic of `Serialize()`, the serialization will be slightly slower, but anyways it will! *(because of reflection manipulations to obtain the type from the object instance)*

## Invalid Data Handling
You can use `PlistInvalidDataHandlingType` to handle invalid data, example:
```csharp
var settings = new PlistSettings() { InvalidDataHandlingType = PlistInvalidDataHandlingType.ReturnNull };

// then use this settings

/*
    * Plist.Deserialize<T>(data, settings);
    * Plist.Serialize<T>(data, settings);
    * new Plist(settings).Load(data);
    * await new Plist(settings).LoadAsync(data);

    etc...
*/
```

## Other features
- This library can `deserialize` and full-named (`<key></key>`, `<string></string>` ...), and short-named (`<k></k>`, `<s></s>` ...) key-values, and both at the same time **(check details in the `PlistAPI.Tests` project)**, but serialize obviously strong-type only *(full-named or short-named)*

- You can choose whether the serialized output will be indent-formatted or not
- Also the library uses faster value parsers *(faster float parser, and faster int parser)*

## Converters
Library supports converters that can manipulate `input / output` values to get better experience with serializers.

You might attach `Internal Converters` (only 2 yet), or create your one!

You can use converters' classes by attaching `[PlistConverter]` attribute to the certain object member, and also you can specify whether the converter converts `input-data-only`, `output-data-only` or everything at once!

#### Internal Converter Usages
There are 2 internal converters, one of them is `IntToBoolConverter` that convertes `integers` greater than `0` to `true`, otherwise `false`
```csharp
string plistData = """
    <plist version="1.0">
        <dict>
                <key>IntProperty</key>
                <integer>1</integer>
        </dict>
    </plist>
    """;

Plist.Deserialize<SomeClass(plistData);

[PlistObject]
class SomeClass
{
    [PlistProperty("IntProperty")]
    [PlistConverter(typeof(StringToBoolConverter))]
    // [PlistConverter(typeof(StringToBoolConverter), ConverterUsage = PlistAPI.Enums.PlistConverterUsage.ConvertInputType)] // with specifications
    public bool ConvertedInt { get; set; }
}
```

#### Creating your own converter
To create your own converter you have to create a class that inherites `IPlistConverter<TInput, TOutput>`, here is an example:

```csharp
// converter class
class StringToFileConverter : IPlistConverter<string, FileInfo>
{
    public FileInfo ReadValue(string value)
    {
        if (File.Exists(value))
            return new FileInfo(value);

        return null;
    }

    public string WriteValue(FileInfo value)
    {
        return value.FullName;
    }
}

// usage
class SomeClass
{
    [PlistProperty("SomeFilePath")]
    [PlistConverter(typeof(StringToFileConverter))] // declaring the converter usage
    public FileInfo File { get; set; }
}
```

this converter returns `FileInfo` instance when deserializing if the representing string as a file path exists, and returns `FileInfo.FullName` when serializing

#### Other important feature about converters: Custom converter property
Custom converter property is a **public** member (field or property) inside the converter class, that you can use too, here is an example how to create custom property, how to use it, and how to declare the specific member that uses the converter **should encount that Custom converter property**

i will use the same converter class as above
```csharp
class StringToFileConverter : IPlistConverter<string, FileInfo>
{
    [PlistConverterProperty(defaultValue: false)] // "false" is a default value
    public bool CheckIfFileIsReadOnly { get; set; }

    public FileInfo ReadValue(string value)
    {
        if (!File.Exists(value))
            return null;

        var file = new FileInfo(value);

        if (CheckIfFileIsReadOnly && file.Attributes.HasFlag(FileAttributes.ReadOnly))
            return file;

        return null;
    }

    public string WriteValue(FileInfo value)
    {
        return value.FullName;
    }
}

// usage
class SomeClass
{
    [PlistProperty("SomeFilePath")]
    [PlistConverter(typeof(StringToFileConverter))] // declaring the converter usage
    [PlistConverterMember(nameof(StringToFileConverter.CheckIfFileIsReadOnly), true)] // declaring that we should use this custom property and use the value "true" for it
    public FileInfo File { get; set; }

    // Also you can use multiple members
    // [PlistConverterMember(nameof(ClassName.SomeMember1), true)]
    // [PlistConverterMember(nameof(ClassName.SomeMember2), "hello")]
    // [PlistConverterMember(nameof(ClassName.SomeMember3), 69)]
    // public object? SomeProperty { get; set; }
}
```

#### Note
Converters usually `cache` themselves in the code internally, but **if they have Custom converter property** they won't be cached, because every serializable property that uses this custom property may and probably will have distinct values, so caching will make the converters return invalid values 

### __Library is being completely maintenanced by me, so dont be shy and pull some issues :)__

## Tests
They are not updated (LET ME BREATH PLEASE)
