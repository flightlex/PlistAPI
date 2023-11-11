using PlistAPI.Enums;

namespace PlistAPI.Extensions
{
    internal static class EnumExtensions
    {
        public static bool IsThrowException(this PlistInvalidDataHandlingType type)
        {
            return type == PlistInvalidDataHandlingType.ThrowException;
        }
    }
}
