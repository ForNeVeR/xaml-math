using System.Globalization;
using static XamlMath.Utils.ColorHelpers;

namespace XamlMath.Colors;

internal sealed class FloatRgbColorParser : RgbColorParserBase<double>
{
    public FloatRgbColorParser(AlphaChannelMode alphaChannelMode) : base(alphaChannelMode)
    {
    }

    protected override double DefaultAlpha => 1.0;

    protected override double? ParseColorComponent(string component) =>
        ParseFloatColorComponent(component, NumberStyles.AllowDecimalPoint);

    protected override byte GetByteValue(double val) =>
        ConvertToByteRgbComponent(val);
}
