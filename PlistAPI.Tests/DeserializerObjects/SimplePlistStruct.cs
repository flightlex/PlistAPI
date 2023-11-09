using PlistAPI.Attributes;

namespace PlistAPI.Tests.DeserializerObjects
{
    [PlistObject]
    public struct SimplePlistStruct
    {
        [PlistProperty] public string Simple { get; set; }
        [PlistProperty("IntValue")] public int IntValue { get; set; }
    }
}
