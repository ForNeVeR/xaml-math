using System.Collections.Generic;
using System.Windows.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class GrayscaleColorParser : IColorParser
    {
        public Color? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParseGrayscaleColor(components, out var color)
                ? Color.FromArgb(color.a, color.r, color.g, color.b)
                : (Color?)null;
    }
}
