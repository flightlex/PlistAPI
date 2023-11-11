using PlistAPI.Enums;
using PlistAPI.Exceptions;
using PlistAPI.Extensions;
using PlistAPI.General.Serializers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
// for cancellation tokens
#if NETSTANDARD2_1
using System.Threading;
#endif

namespace PlistAPI.General
{
    public class Plist : Dictionary<string, object?>
    {
        public static Plist Empty { get => new(); }

        // private properties
        private static readonly XmlReaderSettings _xmlSettings = new()
        {
            DtdProcessing = DtdProcessing.Ignore,
            IgnoreComments = true,
            Async = true,
            IgnoreWhitespace = true,
            XmlResolver = null
        };

        // public properties
        public PlistSettings Settings { get; }

        // constructors
        public Plist() : this(PlistSettings.DefaultSettings())
        { }

        public Plist(PlistSettings settings)
        {
            Settings = settings;
        }

        #region Loadings
        public async Task<Plist> LoadAsync(Stream stream)
        {
            XDocument document;
            var xmlReader = XmlReader.Create(stream, _xmlSettings);

#if NETSTANDARD2_1
            document = await XDocument.LoadAsync(xmlReader, LoadOptions.None, CancellationToken.None);
#else
            document = await Task.Run(() => XDocument.Load(xmlReader, LoadOptions.None));
#endif
            var rootDict = document.Root.Element("dict");

            if (rootDict is null)
                throw new CorruptedPlistException(nameof(rootDict));

            return LoadPlist(rootDict.Elements());
        }

        public Plist Load(Stream stream)
            => LoadAsync(stream).GetAwaiter().GetResult();

        public Plist Load(byte[] data)
            => Load(new MemoryStream(data));

        public async Task<Plist> LoadAsync(byte[] data)
            => await LoadAsync(new MemoryStream(data));

        private Plist LoadPlist(IEnumerable<XElement> elements)
        {
            foreach (var item in elements.Pairs())
                this[item.Key.Value] = GetValue(item.Value);

            return this;
        }

        private object?[] LoadArray(IEnumerable<XElement> elements)
        {
            var count = elements.Count();
            object?[] arrayElements = new object[count];

            for (int i = 0; i < count; i++)
                arrayElements[i] = GetValue(elements.ElementAt(i));

            return arrayElements;
        }

        private object? GetValue(XElement element)
        {
            var dataType = PlistHelper.GetValueType(element.Name.LocalName, Settings.InputDataType);

            return dataType switch
            {
                PlistValueType.String => element.Value,
                PlistValueType.Integer => element.Value.ToInt(),
                PlistValueType.Real => element.Value.ToFloat(),
                PlistValueType.True => true,
                PlistValueType.False => false,
                PlistValueType.Dict => new Plist(this.Settings).LoadPlist(element.Elements()),
                PlistValueType.Array => LoadArray(element.Elements()),

                _ => Settings.InvalidDataHandlingType.IsThrowException() ? throw new InvalidDataException("Unsupported element type") : default
            };
        }
#endregion
        #region Save
        public async Task SaveToStreamAsync(Stream stream)
        {
            var document = CreateDocument();
#if NETSTANDARD2_1
            await document.SaveAsync(stream, Settings.SaveOptions, CancellationToken.None);
#else
            await Task.Run(delegate { document.Save(stream, Settings.SaveOptions); });
#endif
        }

        public async Task<byte[]> SaveAsync()
        {
            var stream = new MemoryStream();
            await SaveToStreamAsync(stream);

            return stream.ToArray();
        }

        public void SaveToStream(Stream stream)
            => SaveToStreamAsync(stream).GetAwaiter().GetResult();

        public byte[] Save()
            => SaveAsync().GetAwaiter().GetResult();

        public string SaveToString()
            => Encoding.UTF8.GetString(Save());

        public async Task<string> SaveToStringAsync()
            => Encoding.UTF8.GetString(await SaveAsync());

        private XDocument CreateDocument()
        {
            var document = new XDocument();
            var root = new XElement("plist");

            foreach (var att in Settings.RootAttributes)
                root.Add(att);

            root.Add(RecursivePlistToString(this, "dict"));

            document.Add(root);
            return document;
        }

        private static XElement RecursivePlistToString(Plist plist, string dictName)
        {
            var dict = new XElement(dictName);
            bool isFull = plist.Settings.OutputDataType == PlistDataType.Full;

            var key = isFull ? "key" : "k";

            foreach (var element in plist)
            {
                if (element.Value is null)
                    continue;

                // key
                dict.Add(new XElement(key, element.Key));

                // value
                dict.Add(FromValue(plist, element.Value, isFull));
            }

            return dict;
        }

        private static XElement RecursiveArrayToString(Plist plist, ICollection collection, string arrayName, bool isFull)
        {
            var array = new XElement(arrayName);

            foreach (var item in collection)
            {
                array.Add(FromValue(plist, item, isFull));
            }

            return array;
        }

        private static XElement? FromValue(Plist plist, object value, bool isFull)
        {
            // string
            if (value is string s)
                return new XElement(isFull ? "string" : "s", s);

            // int
            else if (value is int i)
                return new XElement(isFull ? "integer" : "i", i);

            // float
            else if (value is float f)
                return new XElement(isFull ? "real" : "r", FastParsers.ToString(f));

            // bool
            else if (value is bool b)
                return new XElement(b ? (isFull ? "true" : "t") : (isFull ? "false" : "f"));

            // dict
            else if (value is Plist dict)
            {
                var d = isFull ? "dict" : "d";

                if (dict.Values.Count < 1)
                    return new XElement(d);

                return RecursivePlistToString(dict, d);
            }

            // array
            else if (value is ICollection collection)
            {
                var a = isFull ? "array" : "a";

                if (collection.OfType<object>().Count() < 1)
                    return new XElement(a);

                return RecursiveArrayToString(plist, collection, a, isFull);
            }

            else
                return plist.Settings.InvalidDataHandlingType.IsThrowException() ? throw new InvalidDataException("Unsupported element type") : default;
        }
#endregion

        // static methods
        #region statics
        #region Deserialize
        public static T? Deserialize<T>(byte[] data)
            => Deserialize<T>(data, PlistSettings.DefaultSettings());

        public static T? Deserialize<T>(byte[] data, PlistSettings settings)
            => Deserialize<T>(new MemoryStream(data), settings);

        public static T? Deserialize<T>(Stream stream)
            => Deserialize<T>(stream, PlistSettings.DefaultSettings());

        public static T? Deserialize<T>(Stream stream, PlistSettings settings)
        {
            PlistHelper.CheckForObjectAssignation<T>();

            var members = PlistHelper.GetPlistPropertyMembers<T>();

            if (members.Count() < 1)
                throw new PlistPropertiesNotFoundException(nameof(T));

            return PlistDeserializer.DeserializeProperties<T>(new Plist(settings).Load(stream), members);
        }
        #endregion
        #region Serialize
        public static byte[] Serialize(object instance)
            => Serialize(instance, PlistSettings.DefaultSettings());

        public static byte[] Serialize(object instance, PlistSettings settings)
        {
            return (byte[])typeof(Plist)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.ContainsGenericParameters && x.GetParameters().Length == 2)
                .MakeGenericMethod(instance.GetType())
                .Invoke(null, new object[] { instance, settings });
        }

        public static byte[] Serialize<T>(T instance)
            => Serialize<T>(instance, PlistSettings.DefaultSettings());

        public static byte[] Serialize<T>(T instance, PlistSettings settings)
        {
            PlistHelper.CheckForObjectAssignation<T>();

            var members = PlistHelper.GetPlistPropertyMembers<T>();

            if (members.Count() < 1)
                throw new PlistPropertiesNotFoundException(nameof(T));

            return PlistSerializer.SerializeProperties<T>(instance, new Plist(settings), members).Save();
        }
        #endregion
        #endregion
    }
}
