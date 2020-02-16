using System.Collections.Generic;
using System.Windows.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    public class PredefinedColorParser : IColorParser
    {
        public static readonly PredefinedColorParser Instance = new PredefinedColorParser();

        public Color? Parse(IEnumerable<string> components)
            => ColorHelpers.TryPredefinedColorParse(components, out var color)
                ? Color.FromArgb(color.a, color.r, color.g, color.b)
                : (Color?)null;
    }
}
