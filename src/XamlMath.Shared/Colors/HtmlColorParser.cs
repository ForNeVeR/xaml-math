using System.Globalization;

namespace XamlMath.Colors;

internal sealed class HtmlColorParser : SingleComponentColorParser
{
    const int RGB_LENGTH = 6;
    const int RGBA_LENGTH = 8;

    protected override RgbaColor? ParseSingleComponent(string component)
    {
        var isRgb = component.Length == RGB_LENGTH;
        var isRgba = component.Length == RGBA_LENGTH;

        if (!isRgb && !isRgba)
            return null;

        if (!int.TryParse(component, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var colorCode))
            return null;

        return isRgb ? InterpretRGB(colorCode) : InterpretRGBA(colorCode);
    }

    private static RgbaColor InterpretRGBA(int colorCode)
    {
        var r = (byte)((colorCode & 0xFF0000) >> 24);
        var g = (byte)((colorCode & 0xFF00) >> 16);
        var b = (byte)((colorCode & 0xFF) >> 8);
        var a = (byte)(colorCode & 0xFF);
        return RgbaColor.FromArgb(a, r, g, b);
    }

    private static RgbaColor InterpretRGB(int colorCode)
    {
        var r = (byte)((colorCode & 0xFF0000) >> 16);
        var g = (byte)((colorCode & 0xFF00) >> 8);
        var b = (byte)(colorCode & 0xFF);
        return RgbaColor.FromRgb(r, g, b);
    }
}
