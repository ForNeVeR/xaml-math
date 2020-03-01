using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class CmykColorParser : IColorParser
    {
        public ArgbColor? Parse(IReadOnlyList<string> components)
        {
            var hasAlpha = components.Count == 5;
            if (components.Count != 4 && !hasAlpha)
                return null;

            var cmyk = components
                .Select(x=> ColorHelpers.ParseFloatColorComponent(x, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint))
                .ToArray();
            var c = cmyk[0];
            var m = cmyk[1];
            var y = cmyk[2];
            var k = cmyk[3];
            var aFraction = hasAlpha ? cmyk[4] : 1.0;
            if (!(c.HasValue && m.HasValue && y.HasValue && k.HasValue && aFraction.HasValue))
                return null;

            var color = new ArgbColor
            {
                R = ColorHelpers.ConvertToByteRgbComponent((1.0 - c.Value) * (1.0 - k.Value)),
                G = ColorHelpers.ConvertToByteRgbComponent((1.0 - m.Value) * (1.0 - k.Value)),
                B = ColorHelpers.ConvertToByteRgbComponent((1.0 - y.Value) * (1.0 - k.Value)),
                A = ColorHelpers.ConvertToByteRgbComponent(aFraction.Value),
            };
            return color;
        }
    }
}
