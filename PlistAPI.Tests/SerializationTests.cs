using PlistAPI.General;
using PlistAPI.Tests.DeserializerObjects;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace PlistAPI.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void SimplePlistDeserializationUsingBytes_EqualTest()
        {
            var plist = new Plist();
            plist.Load(Properties.Resources.SimplePlist.GetBytes());

            Assert.Equal("Plist", (string)plist["Simple"]);
            Assert.Equal(696969, (int)plist["IntValue"]);
            Assert.Equal(2, plist.Count);
        }

        [Fact]
        public async Task SimplePlistAsyncDeserializationUsingBytes_EqualTest()
        {
            var plist = new Plist();
            await plist.LoadAsync(Properties.Resources.SimplePlist.GetBytes());

            Assert.Equal("Plist", (string)plist["Simple"]);
            Assert.Equal(696969, (int)plist["IntValue"]);
            Assert.Equal(2, plist.Count);
        }

        [Fact]
        public void SimplePlistSerializationToString_ContainsTest()
        {
            var plist = new Plist();
            plist.Load(Properties.Resources.SimplePlist.GetBytes());

            var serialized = plist.SaveToString();

            Assert.Contains("<key>Simple</key>", serialized);
            Assert.Contains("<string>Plist</string>", serialized);

            Assert.Contains("<key>IntValue</key>", serialized);
            Assert.Contains("<integer>696969</integer>", serialized);
        }

        [Fact]
        public void SimplePlistSerializationToStringUsingShortType_ContainsTest()
        {
            var plist = new Plist(new() { InputDataType = Enums.PlistDataType.Both, OutputDataType = Enums.PlistDataType.Short });
            plist.Load(Properties.Resources.SimplePlist.GetBytes());

            var serialized = plist.SaveToString();

            Assert.Contains("<k>Simple</k>", serialized);
            Assert.Contains("<s>Plist</s>", serialized);

            Assert.Contains("<k>IntValue</k>", serialized);
            Assert.Contains("<i>696969</i>", serialized);
        }

        [Fact]
        public void SimplePlistDeserializationUsingObjectClass_EqualTest()
        {
            var deserializedClass = Plist.Deserialize<SimplePlistClass>(Properties.Resources.SimplePlist.GetBytes());

            Assert.Equal("Plist", deserializedClass.SimpleProperty);
            Assert.Equal(696969, deserializedClass.IntValue);
        }

        [Fact]
        public void SimplePlistDeserializationUsingObjectStruct_EqualTest()
        {
            var deserializedClass = Plist.Deserialize<SimplePlistStruct>(Properties.Resources.SimplePlist.GetBytes());

            Assert.Equal("Plist", deserializedClass.Simple);
            Assert.Equal(696969, deserializedClass.IntValue);
        }

        [Fact]
        public void SimplePlistSerializationUsingObjectClass_ContainsTest()
        {
            var deserializedClass = Plist.Deserialize<SimplePlistClass>(Properties.Resources.SimplePlist.GetBytes());
            var serialized = Plist.Serialize(deserializedClass).GetString();

            Assert.Contains("<key>Simple</key>", serialized);
            Assert.Contains("<string>Plist</string>", serialized);

            Assert.Contains("<key>IntValue</key>", serialized);
            Assert.Contains("<integer>696969</integer>", serialized);
        }

        [Fact]
        public void SimplePlistSerializationUsingObjectStruct_ContainsTest()
        {
            var deserializedClass = Plist.Deserialize<SimplePlistStruct>(Properties.Resources.SimplePlist.GetBytes());
            var serialized = Plist.Serialize(deserializedClass).GetString();

            Assert.Contains("<key>Simple</key>", serialized);
            Assert.Contains("<string>Plist</string>", serialized);

            Assert.Contains("<key>IntValue</key>", serialized);
            Assert.Contains("<integer>696969</integer>", serialized);
        }
    }
}
