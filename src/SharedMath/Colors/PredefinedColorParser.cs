using System.Collections.Generic;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    public class PredefinedColorParser : IColorParser
    {
        public static readonly PredefinedColorParser Instance = new PredefinedColorParser();

        public RgbaColor? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParsePredefinedColor(components, out var color)
                ? color
                : (RgbaColor?)null;
    }
}
