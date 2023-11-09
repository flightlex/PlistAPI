using csFastFloat;
using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;

namespace PlistAPI
{
    public static class FastParsers
    {
        public static int ToInt(this string input)
        {
            var inputSpan = input.AsSpan();
            ref var start = ref MemoryMarshal.GetReference(inputSpan);
            ref var end = ref Unsafe.Add(ref start, inputSpan.Length);

            var result = 0;
            int i = 0;

            bool isNegative = false;
            if (Unsafe.IsAddressLessThan(ref start, ref end) && start == '-')
            {
                isNegative = true;
                start = ref Unsafe.Add(ref start, 1);
                i++;
            }

            while (Unsafe.IsAddressLessThan(ref start, ref end))
            {
                result = result * 10 + (Unsafe.Add(ref MemoryMarshal.GetReference(inputSpan), i) - '0');
                start = ref Unsafe.Add(ref start, 1);
                i++;
            }

            if (isNegative)
            {
                result = -result;
            }

            return result;
        }

        public static float ToFloat(this string input)
        {
            return FastFloatParser.ParseFloat(input, System.Globalization.NumberStyles.Any);
        }

        public static string ToString(this float input)
        {
            return input.ToString(Culture.FormatProvider);
        }
    }
}
