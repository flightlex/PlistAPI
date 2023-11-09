using PlistAPI.Attributes;

namespace PlistAPI.Tests.DeserializerObjects
{
    [PlistObject]
    public struct ComplicatedPlistStruct
    {
        [PlistProperty] public int SomeInt { get; set; }
        [PlistProperty("Person")] public PersonStruct FirstPerson { get; set; }
    }

    [PlistObject]
    public struct PersonStruct
    {
        [PlistProperty] public string FirstName { get; set; }
        [PlistProperty("LastName")] public string LastName { get; set; }
        [PlistProperty] public AddressStruct Address { get; set; }
        [PlistProperty("PhoneNumbers")] public PhoneNumberStruct[] Numbers { get; set; }
    }

    [PlistObject]
    public struct AddressStruct
    {
        [PlistProperty("City")] public string CityAddress { get; set; }
    }

    [PlistObject]
    public struct PhoneNumberStruct
    {
        [PlistProperty] public string Number { get; set; }
    }
}
