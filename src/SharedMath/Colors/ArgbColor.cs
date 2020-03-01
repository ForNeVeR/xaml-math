using System;
using System.ComponentModel;
using System.Globalization;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    [TypeConverter(typeof(ArgbColorConverter))]
    public struct ArgbColor
    {
        public static ArgbColor Black { get; } = new ArgbColor(0, 0, 0);

        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public byte A { get; set; }

        public ArgbColor(byte a, byte r, byte g, byte b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public ArgbColor(byte r, byte g, byte b)
            : this(0xFF, r, g, b)
        { }

        public static ArgbColor Parse(string argbColor)
        {
            if (argbColor is null)
                throw new ArgumentNullException(nameof(argbColor));

            var isRgb = argbColor.Length == 7;
            var isArgb = argbColor.Length == 9;
            if (!isRgb && !isArgb)
                throw new FormatException($"Invalid ARGB color code: '{argbColor}'.");

            var colorCode = ColorHelpers.ParseUintColor(argbColor.Substring(1), NumberStyles.HexNumber);
            if (!colorCode.HasValue)
                throw new FormatException($"Invalid parse ARGB color code: '{argbColor}'.");

            var value = isRgb ? 0xFF000000 | colorCode.Value : colorCode.Value;
            return FromUInt32(value);
        }

        public static ArgbColor FromUInt32(uint value) =>
            new ArgbColor(
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF));

        public uint ToUint32() =>
            ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | (uint)B;

        public override string ToString()
        {
            var argb = ToUint32();
            return $"#{argb:X8}";
        }
    }
}
