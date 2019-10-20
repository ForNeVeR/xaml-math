using System.Globalization;
using System.Windows.Media;

namespace WpfMath.Colors
{
    internal class HtmlColorParser : SingleComponentColorParser
    {
        protected override Color? ParseSingleComponent(string component)
        {
            if (component.Length != 6)
                return null;

            var colorCode = int.Parse(component, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            var r = (byte) ((colorCode & 0xFF0000) >> 16);
            var g = (byte) ((colorCode & 0xFF00) >> 8);
            var b = (byte) (colorCode & 0xFF);
            return Color.FromRgb(r, g, b);
        }
    }
}
