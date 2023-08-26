using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using static XamlMath.Utils.ColorHelpers;

namespace XamlMath.Colors;

internal sealed class CmykColorParser : IColorParser
{
    public RgbaColor? Parse(IReadOnlyList<string> components)
    {
        var hasAlpha = components.Count == 5;
        if (components.Count != 4 && !hasAlpha)
            return null;

        var cmyk = components
            .Select(x =>ParseFloatColorComponent(x, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint))
            .ToArray();
        var c = cmyk[0];
        var m = cmyk[1];
        var y = cmyk[2];
        var k = cmyk[3];
        var aFraction = hasAlpha ? cmyk[4] : 1.0;
        if (c == null || m == null || y == null || k == null || aFraction == null)
            return null;

        var r = ConvertToByteRgbComponent((1.0 - c.Value) * (1.0 - k.Value));
        var g = ConvertToByteRgbComponent((1.0 - m.Value) * (1.0 - k.Value));
        var b = ConvertToByteRgbComponent((1.0 - y.Value) * (1.0 - k.Value));
        var a = ConvertToByteRgbComponent(aFraction.Value);
        return RgbaColor.FromArgb(a, r, g, b);
    }
}
