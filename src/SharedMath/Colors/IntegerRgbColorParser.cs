using System;
using System.Globalization;

namespace WpfMath.Colors
{
    internal class IntegerRgbColorParser : RgbColorParserBase<byte>
    {
        public IntegerRgbColorParser(AlphaChannelMode alphaChannelMode) : base(alphaChannelMode)
        {
        }

        protected override byte DefaultAlpha => 255;

        protected override Tuple<bool, byte> TryParseComponent(string component)
        {
            var success = byte.TryParse(component, NumberStyles.None, CultureInfo.InvariantCulture, out var val);
            return Tuple.Create(success, val);
        }

        protected override byte GetByteValue(byte val) => val;
    }
}
