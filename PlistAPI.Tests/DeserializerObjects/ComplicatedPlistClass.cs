using PlistAPI.Attributes;

namespace PlistAPI.Tests.DeserializerObjects
{
    [PlistObject]
    public sealed class ComplicatedPlistClass
    {
        [PlistProperty] public int SomeInt { get; set; }
        [PlistProperty("Person")] public PersonClass FirstPerson { get; set; }
    }

    [PlistObject]
    public sealed class PersonClass
    {
        [PlistProperty] public string FirstName { get; set; }
        [PlistProperty("LastName")] public string LastName { get; set; }
        [PlistProperty] public AddressClass Address { get; set; }
        [PlistProperty("PhoneNumbers")] public PhoneNumber[] Numbers { get; set; }
    }

    [PlistObject]
    public sealed class AddressClass
    {
        [PlistProperty("City")] public string CityAddress { get; set; }
    }

    [PlistObject]
    public sealed class PhoneNumber
    {
        [PlistProperty] public string Number { get; set; }
    }
}
