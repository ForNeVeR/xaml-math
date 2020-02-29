using System.ComponentModel;

namespace WpfMath.Colors
{
    [TypeConverter(typeof(RgbaColorConverter))]
    public struct RgbaColor
    {
        public static RgbaColor Black { get; } = new RgbaColor(0, 0, 0);

        public byte R { get; set; }

        public byte G { get; set; }

        public byte B { get; set; }

        public byte A { get; set; }

        public RgbaColor(byte r, byte g, byte b, byte a = 0xFF)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        public uint ToUint32() =>
            ((uint)A << 24) | ((uint)R << 16) | ((uint)G << 8) | (uint)B;

        public override string ToString()
        {
            var argb = ToUint32();
            return $"#{argb:X8}";
        }
    }
}
