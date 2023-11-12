using PlistAPI.Enums;

namespace PlistAPI.Extensions
{
    internal static class EnumExtensions
    {
        // Easier checking
        public static bool IsThrowException(this PlistInvalidDataHandlingType type)
        {
            return type == PlistInvalidDataHandlingType.ThrowException;
        }
    }
}
