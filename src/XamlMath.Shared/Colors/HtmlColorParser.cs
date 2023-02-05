using System.Globalization;

namespace WpfMath.Colors;

internal class HtmlColorParser : SingleComponentColorParser
{
    protected override RgbaColor? ParseSingleComponent(string component)
    {
        var isRgb = component.Length == 6;
        var isRgba = component.Length == 8;
        if (!isRgb && !isRgba)
            return null;

        if (!int.TryParse(component, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var colorCode))
            return null;

        if (isRgb)
        {
            var r = (byte) ((colorCode & 0xFF0000) >> 16);
            var g = (byte) ((colorCode & 0xFF00) >> 8);
            var b = (byte) (colorCode & 0xFF);
            return RgbaColor.FromRgb(r, g, b);
        }
        else
        {
            var r = (byte) ((colorCode & 0xFF0000) >> 24);
            var g = (byte) ((colorCode & 0xFF00) >> 16);
            var b = (byte) ((colorCode & 0xFF) >> 8);
            var a = (byte) (colorCode & 0xFF);
            return RgbaColor.FromArgb(a, r, g, b);
        }
    }
}
