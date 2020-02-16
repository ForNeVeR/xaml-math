using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WpfMath.Utils
{
    internal static class ColorHelpers
    {
        public static bool TryCmykColorParse(
            IEnumerable<string> components,
            out (byte r, byte g, byte b, byte a) color)
        {
            color = default;
            var componentList = components.ToList();
            var hasAlpha = componentList.Count == 5;
            if (componentList.Count != 4 && !hasAlpha)
                return false;

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
                return false;

            color.r = (byte) Math.Round(255.0 * (1.0 - c.Value) * (1.0 - k.Value), MidpointRounding.AwayFromZero);
            color.g = (byte) Math.Round(255.0 * (1.0 - m.Value) * (1.0 - k.Value), MidpointRounding.AwayFromZero);
            color.b = (byte) Math.Round(255.0 * (1.0 - y.Value) * (1.0 - k.Value), MidpointRounding.AwayFromZero);
            color.a = (byte) Math.Round(255.0 * aFraction.Value, MidpointRounding.AwayFromZero);
            return true;
        }

        public static bool TryGrayscaleColorParse(
            IEnumerable<string> components,
            out (byte r, byte g, byte b, byte a) color)
        {
            color = default;
            var componentList = components.ToList();
            var hasAlpha = componentList.Count == 2;
            if (componentList.Count != 1 && !hasAlpha)
                return false;

            var success = double.TryParse(
                componentList[0],
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var gradation);
            if (!success || gradation < 0.0 || gradation > 1.0)
                return false;

            double? alpha = 1.0;
            if (hasAlpha)
                alpha = double.TryParse(
                    componentList[1],
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out var value) && value >= 0.0 && value <= 1.0
                    ? (double?) value
                    : null;

            if (alpha == null)
                return false;

            var colorValue = (byte) Math.Round(gradation * 255.0, MidpointRounding.AwayFromZero);
            color.r = colorValue;
            color.g = colorValue;
            color.b = colorValue;
            color.a = (byte) Math.Round(alpha.Value * 255.0, MidpointRounding.AwayFromZero);
            return true;
        }

        public static bool TryHtmlColorParse(
            string component,
            out (byte r, byte g, byte b, byte a) color)
        {
            color = default;
            var isRgb = component.Length == 6;
            var isRgba = component.Length == 8;
            if (!isRgb && !isRgba)
                return false;

            if (!int.TryParse(component, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var colorCode))
                return false;

            if (isRgb)
            {
                color.r = (byte) ((colorCode & 0xFF0000) >> 16);
                color.g = (byte) ((colorCode & 0xFF00) >> 8);
                color.b = (byte) (colorCode & 0xFF);
                color.a = 0xFF;
            }
            else
            {
                color.r = (byte) ((colorCode & 0xFF0000) >> 24);
                color.g = (byte) ((colorCode & 0xFF00) >> 16);
                color.b = (byte) ((colorCode & 0xFF) >> 8);
                color.a = (byte) (colorCode & 0xFF);
            }
            return true;
        }
    }
}
