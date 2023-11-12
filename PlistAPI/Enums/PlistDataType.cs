namespace PlistAPI.Enums
{
    /// <summary>
    /// Specifies the data type of a plist
    /// </summary>
    /// <example>
    /// <see cref="PlistDataType.Full"/> : <key>SomeKey</key>, <string>Some string</string>, <real>6969.6969</real>
    /// <see cref="PlistDataType.Short"/> : <k>SomeKey</k>, <s>Some string</s>, <r>6969.6969</r>
    /// </example>
    public enum PlistDataType : byte
    {
        Short = 1, Full, Both
    }
}
