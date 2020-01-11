using System;
using System.Globalization;

namespace WpfMath.Colors
{
    internal class FloatRgbColorParser : RgbColorParserBase<double>
    {
        public FloatRgbColorParser(AlphaChannelMode alphaChannelMode) : base(alphaChannelMode)
        {
        }

        protected override double DefaultAlpha => 1.0;

        protected override Tuple<bool, double> TryParseComponent(string component)
        {
            var success = double.TryParse(
                component,
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var value);
            return Tuple.Create(success && value >= 0.0 && value <= 1.0, value);
        }

        protected override byte GetByteValue(double val) =>
            (byte) Math.Round(255.0 * val, MidpointRounding.AwayFromZero);
    }
}
