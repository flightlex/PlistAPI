using PlistAPI.Attributes;

namespace PlistAPI.Tests.DeserializerObjects
{
    [PlistObject]
    public sealed class SimplePlistClass
    {
        [PlistProperty("Simple")] public string SimpleProperty { get; set; }
        [PlistProperty] public int IntValue { get; set; }
    }
}
