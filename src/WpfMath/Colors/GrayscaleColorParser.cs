using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace WpfMath.Colors
{
    internal class GrayscaleColorParser : IColorParser
    {
        public Color? Parse(IEnumerable<string> components)
        {
            var componentList = components.ToList();
            var hasAlpha = componentList.Count == 2;
            if (componentList.Count != 1 && !hasAlpha)
                return null;

            var success = double.TryParse(
                componentList[0],
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var gradation);
            if (!success || gradation < 0.0 || gradation > 1.0)
                return null;

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
                return null;

            var colorValue = (byte) Math.Round(gradation * 255.0, MidpointRounding.AwayFromZero);
            var a = (byte) Math.Round(alpha.Value * 255.0, MidpointRounding.AwayFromZero);
            return Color.FromArgb(a, colorValue, colorValue, colorValue);
        }
    }
}
