namespace WpfMath.Colors
{
    public struct RgbaColor
    {
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
    }
}
