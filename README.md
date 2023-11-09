# PlistAPI
 
PlistAPI - is a library that allows you not just `deserialize` and `serialize` the `.plist` format, but even `deserialize` and `serialize` the data as specific objects, there is an example;

### .plist (basically a little changed .xml) format
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

### deserialization
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

### serialization
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

### TODO
- A little refactor
- Add `PlistConverter` attribute for properties to automatically change the type when `serializing/deserializing` at the runtime
- Add null-returns instead of throwing exceptions on each mismatch *(selectively)*
