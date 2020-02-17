using System.Collections.Generic;
using Avalonia.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class GrayscaleColorParser : IColorParser
    {
        public Color? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParseGrayscaleColor(components, out var color)
                ? Color.FromArgb(color.A, color.R, color.G, color.B)
                : (Color?)null;
    }
}
