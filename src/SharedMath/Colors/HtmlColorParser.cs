using System.Globalization;

namespace WpfMath.Colors
{
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

            var color = new RgbaColor();
            if (isRgb)
            {
                color.R = (byte) ((colorCode & 0xFF0000) >> 16);
                color.G = (byte) ((colorCode & 0xFF00) >> 8);
                color.B = (byte) (colorCode & 0xFF);
                color.A = 0xFF;
            }
            else
            {
                color.R = (byte) ((colorCode & 0xFF0000) >> 24);
                color.G = (byte) ((colorCode & 0xFF00) >> 16);
                color.B = (byte) ((colorCode & 0xFF) >> 8);
                color.A = (byte) (colorCode & 0xFF);
            }
            return color;
        }
    }
}
