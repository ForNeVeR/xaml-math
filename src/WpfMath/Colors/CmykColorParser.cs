using System.Collections.Generic;
using System.Windows.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class CmykColorParser : IColorParser
    {
        public Color? Parse(IEnumerable<string> components)
            => ColorHelpers.TryParseCmykColor(components, out var color)
                ? Color.FromArgb(color.a, color.r, color.g, color.b)
                : (Color?)null;
    }
}
