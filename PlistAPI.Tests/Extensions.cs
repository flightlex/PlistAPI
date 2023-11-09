using System.Text;

namespace PlistAPI.Tests
{
    internal static class Extensions
    {
        public static byte[] GetBytes(this string input)
            => Encoding.UTF8.GetBytes(input);

        public static string GetString(this byte[] input)
            => Encoding.UTF8.GetString(input);
    }
}
