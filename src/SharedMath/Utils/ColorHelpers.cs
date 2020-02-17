using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using WpfMath.Colors;

namespace WpfMath.Utils
{
    internal static class ColorHelpers
    {
        private static readonly IReadOnlyDictionary<string, RgbaColor> predefinedRgbColors;

        static ColorHelpers()
        {
            const string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedColors.xml";
            using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var doc = XDocument.Load(resource);
            predefinedRgbColors = GetPredefinedColors(doc.Root);
        }

        public static bool TryParseCmykColor(IReadOnlyList<string> components, out RgbaColor color)
        {
            color = default;
            var hasAlpha = components.Count == 5;
            if (components.Count != 4 && !hasAlpha)
                return false;

            var cmyk = components
                .Select(x=> ParseFloatColorComponent(x, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint))
                .ToArray();
            var c = cmyk[0];
            var m = cmyk[1];
            var y = cmyk[2];
            var k = cmyk[3];
            var aFraction = hasAlpha ? cmyk[4] : 1.0;
            if (!(c.HasValue && m.HasValue && y.HasValue && k.HasValue && aFraction.HasValue))
                return false;

            color.R = ConvertToByteRgbComponent((1.0 - c.Value) * (1.0 - k.Value));
            color.G = ConvertToByteRgbComponent((1.0 - m.Value) * (1.0 - k.Value));
            color.B = ConvertToByteRgbComponent((1.0 - y.Value) * (1.0 - k.Value));
            color.A = ConvertToByteRgbComponent(aFraction.Value);
            return true;
        }

        public static bool TryParseGrayscaleColor(IReadOnlyList<string> components, out RgbaColor color)
        {
            color = default;
            var hasAlpha = components.Count == 2;
            if (components.Count != 1 && !hasAlpha)
                return false;

            var gradation = ParseFloatColorComponent(components[0], NumberStyles.AllowDecimalPoint);
            if (!gradation.HasValue)
                return false;

            var alpha = hasAlpha
                ? ParseFloatColorComponent(components[1], NumberStyles.AllowDecimalPoint)
                : 1.0;
            if (!alpha.HasValue)
                return false;

            var colorValue = ConvertToByteRgbComponent(gradation.Value);
            color.R = colorValue;
            color.G = colorValue;
            color.B = colorValue;
            color.A = ConvertToByteRgbComponent(alpha.Value);
            return true;
        }

        public static bool TryParsePredefinedColor(IReadOnlyList<string> components, out RgbaColor color)
        {
            color = default;
            var hasAlphaComponent = components.Count == 2;
            if (components.Count != 1 && !hasAlphaComponent)
                return false;

            var colorName = components[0];
            if (!predefinedRgbColors.TryGetValue(colorName, out var predefinedRgbColor))
                return false;

            byte? alpha = 255;
            if (hasAlphaComponent)
            {
                var alphaFraction = ParseFloatColorComponent(components[1], NumberStyles.AllowDecimalPoint);
                if (!alphaFraction.HasValue)
                    return false;

                alpha = ConvertToByteRgbComponent(alphaFraction.Value);
            }

            if (!alpha.HasValue)
                return false;

            color = predefinedRgbColor;
            color.A = alpha.Value;
            return true;
        }

        public static bool TryParseHtmlColor(string component, out RgbaColor color)
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
                color.R = (byte) ((colorCode & 0xFF0000) >> 16);
                color.G = (byte) ((colorCode & 0xFF00) >> 8);
                color.B = (byte) (colorCode & 0xFF);
                color.A = 0xFF;
            }
            else
            {
                color.R = (byte) ((colorCode & 0xFF0000) >> 24);
                color.G = (byte) ((colorCode & 0xFF00) >> 16);
                color.B = (byte) ((colorCode & 0xFF) >> 8);
                color.A = (byte) (colorCode & 0xFF);
            }
            return true;
        }

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

        internal static bool TryParseRgbColor<T>(
            IReadOnlyList<string> components,
            AlphaChannelMode alphaChannelMode,
            T defaultAlpha,
            Func<string, T?> tryParseComponent,
            Func<T, byte> getByteValue,
            out RgbaColor color)
            where T : struct
        {
            color = default;
            var values = components
                .Select(tryParseComponent)
                .ToArray();

            var index = 0;
            var alpha = alphaChannelMode == AlphaChannelMode.AlphaFirst
                ? values[index++]
                : defaultAlpha;

            var r = values[index++];
            var g = values[index++];
            var b = values[index++];

            if (alphaChannelMode == AlphaChannelMode.AlphaLast)
                alpha = values[index];

            if (!(alpha.HasValue && r.HasValue && g.HasValue && b.HasValue))
                return false;
            color = new RgbaColor(getByteValue(alpha.Value), getByteValue(r.Value), getByteValue(g.Value), getByteValue(b.Value));
            return true;
        }

        private static Dictionary<string, RgbaColor> GetPredefinedColors(XElement rootElement)
        {
            var colors = new Dictionary<string, RgbaColor>();
            foreach (var colorElement in rootElement.Elements("color"))
            {
                var name = colorElement.AttributeValue("name");
                var r = colorElement.AttributeValue("r");
                var g = colorElement.AttributeValue("g");
                var b = colorElement.AttributeValue("b");
                colors.Add(name, new RgbaColor(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b)));
            }

            return colors;
        }
    }
}
