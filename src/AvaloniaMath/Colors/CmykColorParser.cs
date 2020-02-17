using System.Collections.Generic;
using Avalonia.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class CmykColorParser : IColorParser
    {
        public Color? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParseCmykColor(components, out var color)
                ? Color.FromArgb(color.A, color.R, color.G, color.B)
                : (Color?)null;
    }
}
