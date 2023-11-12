using PlistAPI.General;
using PlistAPI.Tests.DeserializerObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlistAPI.Tests
{
    public class DifficultSerializationTests
    {
        [Fact]
        public void ComplicatedPlistDeserializationUsingBytes_EqualTest()
        {
            var plist = new Plist();
            plist.Load(Properties.Resources.ComplicatedPlist.GetBytes());

            var person = (Plist)plist["Person"];
            var address = (Plist)person["Address"];
            var phoneNumbers = (IEnumerable<object>)person["PhoneNumbers"];

            Assert.NotNull(person);
            Assert.Equal("John", person["FirstName"]);
            Assert.Equal("Some Town", address["City"]);
            Assert.Equal("555-555-5555", ((Plist)phoneNumbers.First())["Number"]);
            Assert.Equal(2, phoneNumbers.Count());
        }

        [Fact]
        public async Task ComplicatedPlistAsyncDeserializationUsingBytes_EqualTest()
        {
            var plist = new Plist();
            await plist.LoadAsync(Properties.Resources.ComplicatedPlist.GetBytes());

            var person = (Plist)plist["Person"];
            var address = (Plist)person["Address"];
            var phoneNumbers = (IEnumerable<object>)person["PhoneNumbers"];

            Assert.NotNull(person);
            Assert.Equal("John", person["FirstName"]);
            Assert.Equal("Some Town", address["City"]);
            Assert.Equal("555-555-5555", ((Plist)phoneNumbers.First())["Number"]);
            Assert.Equal(2, phoneNumbers.Count());
        }

        [Fact]
        public void ComplicatedPlistSerializationToString_ContainsTest()
        {
            var plist = new Plist();
            plist.Load(Properties.Resources.ComplicatedPlist.GetBytes());

            var serialized = plist.SaveToString();

            Assert.Contains("<key>FirstName</key>", serialized);
            Assert.Contains("<string>John</string>", serialized);

            Assert.Contains("<key>Number</key>", serialized);
            Assert.Contains("<string>555-555-5555</string>", serialized);

            Assert.Contains("<key>City</key>", serialized);
            Assert.Contains("<string>Some Town</string>", serialized);

            Assert.Contains("<key>SomeInt</key>", serialized);
            Assert.Contains("<integer>13371337</integer>", serialized);

            Assert.Contains("<key>LastName</key>", serialized);
            Assert.Contains("<string>Public</string>", serialized);
        }

        [Fact]
        public void ComplicatedPlistSerializationToStringUsingShortType_ContainsTest()
        {
            var plist = new Plist(new() { InputDataType = Enums.PlistDataType.Both, OutputDataType = Enums.PlistDataType.Short });
            plist.Load(Properties.Resources.ComplicatedPlist.GetBytes());

            var serialized = plist.SaveToString();

            Assert.Contains("<k>FirstName</k>", serialized);
            Assert.Contains("<s>John</s>", serialized);

            Assert.Contains("<k>Number</k>", serialized);
            Assert.Contains("<s>555-555-5555</s>", serialized);

            Assert.Contains("<k>City</k>", serialized);
            Assert.Contains("<s>Some Town</s>", serialized);

            Assert.Contains("<k>SomeInt</k>", serialized);
            Assert.Contains("<i>13371337</i>", serialized);

            Assert.Contains("<k>LastName</k>", serialized);
            Assert.Contains("<s>Public</s>", serialized);
        }

        [Fact]
        public void ComplicatedPlistDeserializationUsingObjectClass_EqualTest()
        {
            var deserializedClass = Plist.Deserialize<ComplicatedPlistClass>(Properties.Resources.ComplicatedPlist.GetBytes());

            Assert.Equal("John", deserializedClass.FirstPerson.FirstName);
            Assert.Equal("555-555-5555", deserializedClass.FirstPerson.Numbers[0].Number);
            Assert.Equal("Some Town", deserializedClass.FirstPerson.Address.CityAddress);
            Assert.Equal(13371337, deserializedClass.SomeInt);
            Assert.Equal("Public", deserializedClass.FirstPerson.LastName);
        }

        [Fact]
        public void ComplicatedPlistDeserializationUsingObjectStruct_EqualTest()
        {
            var deserializedClass = Plist.Deserialize<ComplicatedPlistStruct>(Properties.Resources.ComplicatedPlist.GetBytes());

            Assert.Equal("John", deserializedClass.FirstPerson.FirstName);
            Assert.Equal("555-555-5555", deserializedClass.FirstPerson.Numbers[0].Number);
            Assert.Equal("Some Town", deserializedClass.FirstPerson.Address.CityAddress);
            Assert.Equal(13371337, deserializedClass.SomeInt);
            Assert.Equal("Public", deserializedClass.FirstPerson.LastName);
        }

        [Fact]
        public void ComplicatedPlistSerializationUsingObjectClass_ContainsTest()
        {
            var deserializedClass = Plist.Deserialize<ComplicatedPlistClass>(Properties.Resources.ComplicatedPlist.GetBytes());
            var serialized = Plist.Serialize(deserializedClass).GetString();

            Assert.Contains("<key>FirstName</key>", serialized);
            Assert.Contains("<string>John</string>", serialized);

            Assert.Contains("<key>Number</key>", serialized);
            Assert.Contains("<string>555-555-5555</string>", serialized);

            Assert.Contains("<key>City</key>", serialized);
            Assert.Contains("<string>Some Town</string>", serialized);

            Assert.Contains("<key>SomeInt</key>", serialized);
            Assert.Contains("<integer>13371337</integer>", serialized);

            Assert.Contains("<key>LastName</key>", serialized);
            Assert.Contains("<string>Public</string>", serialized);
        }

        [Fact]
        public void ComplicatedPlistSerializationUsingObjectStruct_ContainsTest()
        {
            var deserializedClass = Plist.Deserialize<ComplicatedPlistStruct>(Properties.Resources.ComplicatedPlist.GetBytes());
            var serialized = Plist.Serialize(deserializedClass).GetString();

            Assert.Contains("<key>FirstName</key>", serialized);
            Assert.Contains("<string>John</string>", serialized);

            Assert.Contains("<key>Number</key>", serialized);
            Assert.Contains("<string>555-555-5555</string>", serialized);

            Assert.Contains("<key>City</key>", serialized);
            Assert.Contains("<string>Some Town</string>", serialized);

            Assert.Contains("<key>SomeInt</key>", serialized);
            Assert.Contains("<integer>13371337</integer>", serialized);

            Assert.Contains("<key>LastName</key>", serialized);
            Assert.Contains("<string>Public</string>", serialized);
        }
    }
}
