using System.Globalization;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class HtmlColorParser : SingleComponentColorParser
    {
        protected override ArgbColor? ParseSingleComponent(string component)
        {
            var isRgb = component.Length == 6;
            var isRgba = component.Length == 8;
            if (!isRgb && !isRgba)
                return null;

            var colorCode = ColorHelpers.ParseUintColor(component, NumberStyles.HexNumber);
            if (!colorCode.HasValue)
                return null;

            return isRgb
                ? ArgbColor.FromUInt32(0xFF000000 | colorCode.Value)
                : new ArgbColor(
                    (byte) (colorCode.Value & 0xFF),
                    (byte) ((colorCode.Value & 0xFF0000) >> 24),
                    (byte) ((colorCode.Value & 0xFF00) >> 16),
                    (byte) ((colorCode.Value & 0xFF) >> 8));
        }
    }
}
