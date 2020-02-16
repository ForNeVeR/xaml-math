using System.Globalization;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class IntegerRgbColorParser : RgbColorParserBase<byte>
    {
        public IntegerRgbColorParser(AlphaChannelMode alphaChannelMode) : base(alphaChannelMode)
        {
        }

        protected override byte DefaultAlpha => 255;

        protected override byte? ParseColorComponent(string component)
            => ColorHelpers.ParseByteColorComponent(component, NumberStyles.None);

        protected override byte GetByteValue(byte val) => val;
    }
}
