using System.Globalization;
using static XamlMath.Utils.ColorHelpers;

namespace XamlMath.Colors;

internal sealed class IntegerRgbColorParser : RgbColorParserBase<byte>
{
    public IntegerRgbColorParser(AlphaChannelMode alphaChannelMode) : base(alphaChannelMode)
    {
    }

    protected override byte DefaultAlpha => 255;

    protected override byte? ParseColorComponent(string component) =>
        ParseByteColorComponent(component, NumberStyles.None);

    protected override byte GetByteValue(byte val) => val;
}
