using System.Collections.Generic;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class CmykColorParser : IColorParser
    {
        public RgbaColor? Parse(IReadOnlyList<string> components)
            => ColorHelpers.TryParseCmykColor(components, out var color)
                ? color
                : (RgbaColor?)null;
    }
}
