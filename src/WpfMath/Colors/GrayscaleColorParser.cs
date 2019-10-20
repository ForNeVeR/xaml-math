using System;
using System.Globalization;
using System.Windows.Media;

namespace WpfMath.Colors
{
    internal class GrayscaleColorParser : SingleComponentColorParser
    {
        protected override Color? ParseSingleComponent(string component)
        {
            var gradation = double.Parse(component, CultureInfo.InvariantCulture);
            if (gradation < 0.0 || gradation > 1.0)
                return null;

            var colorValue = (byte) Math.Round(gradation * 255.0, MidpointRounding.AwayFromZero);
            return Color.FromRgb(colorValue, colorValue, colorValue);
        }
    }
}
