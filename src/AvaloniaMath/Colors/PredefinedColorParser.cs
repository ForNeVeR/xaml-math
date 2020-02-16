using System.Collections.Generic;
using Avalonia.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    public class PredefinedColorParser : IColorParser
    {
        public static readonly PredefinedColorParser Instance = new PredefinedColorParser();

        public Color? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParsePredefinedColor(components, out var color)
                ? Color.FromArgb(color.a, color.r, color.g, color.b)
                : (Color?)null;
    }
}
