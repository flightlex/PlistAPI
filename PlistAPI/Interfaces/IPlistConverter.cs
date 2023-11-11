using PlistAPI.Interfaces.BaseInterfaces;

namespace PlistAPI.Interfaces
{
    public interface IPlistConverter<TInput, TOutput> : IPlistConverter
    {
        TOutput ReadValue(TInput value);
        TInput WriteValue(TOutput value);
    }
}
