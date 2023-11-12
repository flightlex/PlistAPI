namespace PlistAPI.Enums
{
    /// <summary>
    /// Specifies whether to throw exceptions on serialization/deserialization mismatches or return <see cref="default"/> or provided original value
    /// </summary>
    public enum PlistInvalidDataHandlingType : byte
    {
        ThrowException, ReturnDefaultOrOrigin
    }
}
