using System.Collections.Generic;
using Avalonia.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class CmykColorParser : IColorParser
    {
        public Color? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParseCmykColor(components, out var color)
                ? Color.FromArgb(color.a, color.r, color.g, color.b)
                : (Color?)null;
    }
}
