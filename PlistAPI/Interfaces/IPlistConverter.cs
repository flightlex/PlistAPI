using PlistAPI.Interfaces.BaseInterfaces;

namespace PlistAPI.Interfaces
{
    /// <summary>
    /// Interface that declares methods needed for converter classes
    /// </summary>
    /// <typeparam name="TInput">Input data type, data type that is comming from the plist stream</typeparam>
    /// <typeparam name="TOutput">Output data type, data type that is member using</typeparam>
    public interface IPlistConverter<TInput, TOutput> : IPlistConverter
    {
        TOutput ReadValue(TInput value);
        TInput WriteValue(TOutput value);
    }
}
