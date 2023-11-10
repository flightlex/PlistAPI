# PlistAPI
`Nuget Package:` https://www.nuget.org/packages/PlistAPI

PlistAPI - is a library that allows you not just `deserialize` and `serialize` the `.plist` format, but even `deserialize` and `serialize` the data as specific objects, here is an example:

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
```csharp
using var fileStream = File.Open("filePath");
Plist.Deserialize<SomeClass>(fileStream);

[PlistObject]
class SomeClass
{
    [PlistProperty("SomeKey")] public string SomeKey { get; set; }
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
    [PlistProperty("SomeKey")] public string SomeKey { get; set; }
}
```

By **NOT** providing the type in the generic of `Serialize()`, the serialization will be UNsegnifically slower, but anyways it will! *(because of reflection manipulations to obtain the type from the object instance)*

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
- You can 

## TODO
- A little refactor
- Add `PlistConverter` attribute for properties to automatically change the type when `serializing/deserializing` at the runtime
- Add `summeries`

### __Library is being completely maintenanced by me, so dont be shy and pull some issues :)__
