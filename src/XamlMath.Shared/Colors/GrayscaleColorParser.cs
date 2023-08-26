using System.Collections.Generic;
using System.Globalization;
using static XamlMath.Utils.ColorHelpers;

namespace XamlMath.Colors;

internal sealed class GrayscaleColorParser : IColorParser
{
    public RgbaColor? Parse(IReadOnlyList<string> components)
    {
        var hasAlpha = components.Count == 2;
        if (components.Count != 1 && !hasAlpha)
            return null;

        var gradation = ParseFloatColorComponent(components[0], NumberStyles.AllowDecimalPoint);
        if (!gradation.HasValue)
            return null;

        var alpha = hasAlpha
            ? ParseFloatColorComponent(components[1], NumberStyles.AllowDecimalPoint)
            : 1.0;
        if (!alpha.HasValue)
            return null;

        var colorValue = ConvertToByteRgbComponent(gradation.Value);
        return RgbaColor.FromArgb(ConvertToByteRgbComponent(alpha.Value), colorValue, colorValue, colorValue);
    }
}
