using System.Collections.Generic;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class GrayscaleColorParser : IColorParser
    {
        public RgbaColor? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParseGrayscaleColor(components, out var color)
                ? color
                : (RgbaColor?)null;
    }
}
