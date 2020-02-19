using System.Globalization;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class FloatRgbColorParser : RgbColorParserBase<double>
    {
        public FloatRgbColorParser(AlphaChannelMode alphaChannelMode) : base(alphaChannelMode)
        {
        }

        protected override double DefaultAlpha => 1.0;

        protected override double? ParseColorComponent(string component) =>
            ColorHelpers.ParseFloatColorComponent(component, NumberStyles.AllowDecimalPoint);

        protected override byte GetByteValue(double val) =>
            ColorHelpers.ConvertToByteRgbComponent(val);
    }
}
