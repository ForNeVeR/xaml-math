using System.Collections.Generic;
using System.Globalization;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class GrayscaleColorParser : IColorParser
    {
        public ArgbColor? Parse(IReadOnlyList<string> components)
        {
            var hasAlpha = components.Count == 2;
            if (components.Count != 1 && !hasAlpha)
                return null;

            var gradation = ColorHelpers.ParseFloatColorComponent(components[0], NumberStyles.AllowDecimalPoint);
            if (!gradation.HasValue)
                return null;

            var alpha = hasAlpha
                ? ColorHelpers.ParseFloatColorComponent(components[1], NumberStyles.AllowDecimalPoint)
                : 1.0;
            if (!alpha.HasValue)
                return null;

            var colorValue = ColorHelpers.ConvertToByteRgbComponent(gradation.Value);
            var color = new ArgbColor
            {
                R = colorValue,
                G = colorValue,
                B = colorValue,
                A = ColorHelpers.ConvertToByteRgbComponent(alpha.Value),
            };
            return color;
        }
    }
}
