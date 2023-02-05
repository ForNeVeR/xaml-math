using System;
using System.Globalization;

namespace XamlMath.Utils;

internal static class ColorHelpers
{
    public static double? ParseFloatColorComponent(string component, NumberStyles numberStyles)
    {
        var success = double.TryParse(component, numberStyles, CultureInfo.InvariantCulture, out var val);
        return success && val >= 0.0 && val <= 1.0 ? (double?)val : null;
    }

    public static byte? ParseByteColorComponent(string component, NumberStyles numberStyles)
    {
        var success = byte.TryParse(component, numberStyles, CultureInfo.InvariantCulture, out var val);
        return success ? (byte?)val : null;
    }

    public static byte ConvertToByteRgbComponent(double val) =>
        (byte) Math.Round(255.0 * val, MidpointRounding.AwayFromZero);
}
