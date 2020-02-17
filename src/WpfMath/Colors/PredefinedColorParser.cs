using System.Collections.Generic;
using System.Windows.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    public class PredefinedColorParser : IColorParser
    {
        public static readonly PredefinedColorParser Instance = new PredefinedColorParser();

        public Color? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParsePredefinedColor(components, out var color)
                ? Color.FromArgb(color.A, color.R, color.G, color.B)
                : (Color?)null;
    }
}
