using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace WpfMath.Colors
{
    internal class CmykColorParser : IColorParser
    {
        public Color? Parse(IEnumerable<string> components)
        {
            var componentList = components.ToList();
            var hasAlpha = componentList.Count == 5;
            if (componentList.Count != 4 && !hasAlpha)
                return null;

            var cmyk = componentList.Select(x =>
            {
                var success = double.TryParse(
                    x,
                    NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out var value);
                return success && value >= 0 && value <= 1.0 ? (double?) value : null;
            }).ToArray();
            var c = cmyk[0];
            var m = cmyk[1];
            var y = cmyk[2];
            var k = cmyk[3];
            var aFraction = hasAlpha ? cmyk[4] : 1.0;
            if (c == null || m == null || y == null || k == null || aFraction == null)
                return null;

            var r = (byte) Math.Round(255.0 * (1.0 - c.Value) * (1.0 - k.Value), MidpointRounding.AwayFromZero);
            var g = (byte) Math.Round(255.0 * (1.0 - m.Value) * (1.0 - k.Value), MidpointRounding.AwayFromZero);
            var b = (byte) Math.Round(255.0 * (1.0 - y.Value) * (1.0 - k.Value), MidpointRounding.AwayFromZero);
            var a = (byte) Math.Round(255.0 * aFraction.Value, MidpointRounding.AwayFromZero);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
