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
using System;
// for cancellation tokens
#if NETSTANDARD2_1
using System.Threading;
#endif

namespace PlistAPI.General
{
    public class Plist : Dictionary<string, object?>
    {
        /// <summary>
        /// Is just an empty instance of Plist. You probably wont use it, but its used on the internal side.
        /// </summary>
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

        /// <summary>
        /// Plist settings
        /// </summary>
        public PlistSettings Settings { get; }

        /// <summary>
        /// Creates new instance of Plist with default settings
        /// </summary>
        public Plist() : this(PlistSettings.DefaultSettings())
        { }

        /// <summary>
        /// Creates new instance of Plist
        /// </summary>
        /// <param name="settings">Settings that will be used in further</param>
        public Plist(PlistSettings settings)
        {
            Settings = settings;
        }

        #region Loadings
        /// <summary>
        /// Asynchronously parses and loads the plist data
        /// </summary>
        /// <param name="stream">Stream that will provide the data</param>
        /// <returns></returns>
        /// <exception cref="CorruptedPlistException"></exception>
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

        /// <summary>
        /// Synchronously parses and loads the plist data
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Plist Load(Stream stream)
            => LoadAsync(stream).GetAwaiter().GetResult();

        /// <summary>
        /// Synchronously parses and loads the plist data
        /// </summary>
        /// <param name="data">Byte array of plist data</param>
        /// <returns></returns>
        public Plist Load(byte[] data)
            => Load(new MemoryStream(data));

        /// <summary>
        /// Asynchronously parses and loads the plist data
        /// </summary>
        /// <param name="data">Byte array of plist data</param>
        /// <returns></returns>
        public async Task<Plist> LoadAsync(byte[] data)
            => await LoadAsync(new MemoryStream(data));

        /// <summary>
        /// Synchronously parses and loads the plist data
        /// </summary>
        /// <param name="data">Plist string data</param>
        /// <returns></returns>
        public Plist Load(string data)
            => Load(new MemoryStream(Encoding.UTF8.GetBytes(data)));

        /// <summary>
        /// Asynchronously parses and loads the plist data
        /// </summary>
        /// <param name="data">Plist string data</param>
        /// <returns></returns>
        public async Task<Plist> LoadAsync(string data)
            => await LoadAsync(new MemoryStream(Encoding.UTF8.GetBytes(data)));

        // loads new plist
        private Plist LoadPlist(IEnumerable<XElement> elements)
        {
            var pairsedElements = elements.Pairs();

            for (var i = 0; i < pairsedElements.Count(); i++)
            {
                var element = pairsedElements.ElementAt(i);

                this[element.Key.Value] = GetValue(element.Value);
            }

            return this;
        }

        // loads new array
        private object?[] LoadArray(IEnumerable<XElement> elements)
        {
            var count = elements.Count();
            object?[] arrayElements = new object[count];

            for (int i = 0; i < count; i++)
                arrayElements[i] = GetValue(elements.ElementAt(i));

            return arrayElements;
        }

        // gets the basic value
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
        /// <summary>
        /// Asynchronously saves the plist data
        /// </summary>
        /// <param name="stream">Stream that will be used to save data to</param>
        /// <returns></returns>
        public async Task SaveToStreamAsync(Stream stream)
        {
            var document = CreateDocument();
#if NETSTANDARD2_1
            await document.SaveAsync(stream, Settings.SaveOptions, CancellationToken.None);
#else
            await Task.Run(delegate { document.Save(stream, Settings.SaveOptions); });
#endif
        }

        /// <summary>
        /// Synchronously saves the plist data
        /// </summary>
        /// <param name="stream">Stream that will be used to save data to</param>
        /// <returns></returns>
        public void SaveToStream(Stream stream)
            => SaveToStreamAsync(stream).GetAwaiter().GetResult();


        /// <summary>
        /// Asynchronously saves and returns the plist data as byte array
        /// </summary>
        /// <returns></returns>
        public async Task<byte[]> SaveAsync()
        {
            var stream = new MemoryStream();
            await SaveToStreamAsync(stream);

            return stream.ToArray();
        }

        /// <summary>
        /// Synchronously saves and returns the plist data as byte array
        /// </summary>
        /// <returns></returns>
        public byte[] Save()
            => SaveAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Asynchronously saves and returns the plist data as string
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveToStringAsync()
            => Encoding.UTF8.GetString(await SaveAsync());

        /// <summary>
        /// Synchronously saves and returns the plist data as string
        /// </summary>
        /// <returns></returns>
        public string SaveToString()
            => Encoding.UTF8.GetString(Save());

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
        /// <summary>
        /// Deserializes the data to a selected class
        /// </summary>
        /// <typeparam name="T">Class type to be created on deserialization</typeparam>
        /// <param name="data">Input data</param>
        /// <returns></returns>
        public static T? Deserialize<T>(string data)
            => Deserialize<T>(Encoding.UTF8.GetBytes(data), PlistSettings.DefaultSettings());

        /// <summary>
        /// Deserializes the data to a selected class
        /// </summary>
        /// <typeparam name="T">Class type to be created on deserialization</typeparam>
        /// <param name="settings">Plist settings to be used furtherly</param>
        /// <param name="data">Input data</param>
        /// <returns></returns>
        public static T? Deserialize<T>(string data, PlistSettings settings)
            => Deserialize<T>(Encoding.UTF8.GetBytes(data), settings);

        /// <summary>
        /// Deserializes the data to a selected class
        /// </summary>
        /// <typeparam name="T">Class type to be created on deserialization</typeparam>
        /// <param name="data">Input data</param>
        /// <returns></returns>
        public static T? Deserialize<T>(byte[] data)
            => Deserialize<T>(data, PlistSettings.DefaultSettings());

        /// <summary>
        /// Deserializes the data to a selected class
        /// </summary>
        /// <typeparam name="T">Class type to be created on deserialization</typeparam>
        /// <param name="settings">Plist settings to be used furtherly</param>
        /// <param name="data">Input data</param>
        /// <returns></returns>
        public static T? Deserialize<T>(byte[] data, PlistSettings settings)
            => Deserialize<T>(new MemoryStream(data), settings);

        /// <summary>
        /// Deserializes the data to a selected class
        /// </summary>
        /// <typeparam name="T">Class type to be created on deserialization</typeparam>
        /// <param name="stream">Input data</param>
        /// <returns></returns>
        public static T? Deserialize<T>(Stream stream)
            => Deserialize<T>(stream, PlistSettings.DefaultSettings());

        /// <summary>
        /// Deserializes the data to a selected class
        /// </summary>
        /// <typeparam name="T">Class type to be created on deserialization</typeparam>
        /// <param name="settings">Plist settings to be used furtherly</param>
        /// <param name="stream">Input data</param>
        /// <returns></returns>
        public static T? Deserialize<T>(Stream stream, PlistSettings settings)
        {
            PlistHelper.CheckForObjectAssignation<T>();

            var members = PlistHelper.GetPlistPropertyMembers<T>();

            if (members.Count() < 1)
                throw new PlistPropertiesNotFoundException(nameof(T));

            return PlistDeserializer.DeserializeMembers<T>(new Plist(settings).Load(stream), members);
        }
        #endregion
        #region Serialize
        /// <summary>
        /// Serializes objects from anonymous object type
        /// </summary>
        /// <param name="instance">Object instance</param>
        /// <returns></returns>
        public static byte[] Serialize(object instance)
            => Serialize(instance, PlistSettings.DefaultSettings());

        /// <summary>
        /// Serializes objects from anonymous object type
        /// </summary>
        /// <param name="settings">Plist settings to be used furtherly</param>
        /// <param name="instance">Object instance</param>
        /// <returns></returns>
        public static byte[] Serialize(object instance, PlistSettings settings)
        {
            var method = ReflectionContainers.GetOrCreateMethod(
                nameof(SerializeInternal),
                () => {
                    return typeof(Plist)
                    .GetMethod(nameof(SerializeInternal), BindingFlags.Static | BindingFlags.NonPublic)
                    .MakeGenericMethod(instance.GetType());
                });

            return (byte[])method.Invoke(null, new object[] { instance, settings });
        }

        /// <summary>
        /// Serializes objects from known object type
        /// </summary>
        /// <typeparam name="T">Class type to be serialized from</typeparam>
        /// <param name="instance">Object instance</param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T instance)
            => SerializeInternal<T>(instance, PlistSettings.DefaultSettings());

        /// <summary>
        /// Serializes objects from known object type
        /// </summary>
        /// <typeparam name="T">Class type to be serialized from</typeparam>
        /// <param name="settings">Plist settings to be used furtherly</param>
        /// <param name="instance">Object instance</param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T instance, PlistSettings settings)
            => SerializeInternal<T>(instance, settings);

        private static byte[] SerializeInternal<T>(T instance, PlistSettings settings)
        {
            PlistHelper.CheckForObjectAssignation<T>();

            var members = PlistHelper.GetPlistPropertyMembers<T>();

            if (members.Count() < 1)
                throw new PlistPropertiesNotFoundException(nameof(T));

            return PlistSerializer.SerializeMembers<T>(instance, new Plist(settings), members).Save();
        }
        #endregion
        #endregion
    }
}
