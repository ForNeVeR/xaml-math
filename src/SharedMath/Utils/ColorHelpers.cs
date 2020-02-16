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
        private static readonly IReadOnlyDictionary<string, (byte r, byte g, byte b)> predefinedRgbColors;

        static ColorHelpers()
        {
            const string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedColors.xml";
            using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var doc = XDocument.Load(resource);
            predefinedRgbColors = GetPredefinedColors(doc.Root);
        }

        public static bool TryCmykColorParse(
            IEnumerable<string> components,
            out (byte r, byte g, byte b, byte a) color)
        {
            color = default;
            var componentList = components.ToList();
            var hasAlpha = componentList.Count == 5;
            if (componentList.Count != 4 && !hasAlpha)
                return false;

            var cmyk = componentList
                .Select(x=> ParseFloatColorComponent(x, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint))
                .ToArray();
            var c = cmyk[0];
            var m = cmyk[1];
            var y = cmyk[2];
            var k = cmyk[3];
            var aFraction = hasAlpha ? cmyk[4] : 1.0;
            if (c == null || m == null || y == null || k == null || aFraction == null)
                return false;

            color.r = ConvertToByteRgbComponent((1.0 - c.Value) * (1.0 - k.Value));
            color.g = ConvertToByteRgbComponent((1.0 - m.Value) * (1.0 - k.Value));
            color.b = ConvertToByteRgbComponent((1.0 - y.Value) * (1.0 - k.Value));
            color.a = ConvertToByteRgbComponent(aFraction.Value);
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

            var gradation = ParseFloatColorComponent(componentList[0], NumberStyles.AllowDecimalPoint);
            if (!gradation.HasValue)
                return false;

            var alpha = hasAlpha
                ? ParseFloatColorComponent(componentList[1], NumberStyles.AllowDecimalPoint)
                : 1.0;
            if (!alpha.HasValue)
                return false;

            var colorValue = ConvertToByteRgbComponent(gradation.Value);
            color.r = colorValue;
            color.g = colorValue;
            color.b = colorValue;
            color.a = (byte) Math.Round(alpha.Value * 255.0, MidpointRounding.AwayFromZero);
            return true;
        }

        public static bool TryPredefinedColorParse(
            IEnumerable<string> components,
            out (byte r, byte g, byte b, byte a) color)
        {
            color = default;
            var componentList = components.ToList();
            var hasAlphaComponent = componentList.Count == 2;
            if (componentList.Count != 1 && !hasAlphaComponent)
                return false;

            var colorName = componentList[0];
            if (!predefinedRgbColors.TryGetValue(colorName, out var predefinedRgbColor))
                return false;

            byte? alpha = 255;
            if (hasAlphaComponent)
            {
                var alphaFraction = ParseFloatColorComponent(componentList[1], NumberStyles.AllowDecimalPoint);
                if (!alphaFraction.HasValue)
                    return false;

                alpha = ConvertToByteRgbComponent(alphaFraction.Value);
            }

            if (!alpha.HasValue)
                return false;

            color = (predefinedRgbColor.r, predefinedRgbColor.g, predefinedRgbColor.b, alpha.Value);
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
            out (byte r, byte g, byte b, byte a) color)
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
            color = (getByteValue(r.Value), getByteValue(g.Value), getByteValue(b.Value), getByteValue(alpha.Value));
            return true;
        }

        private static Dictionary<string, (byte r, byte g, byte b)> GetPredefinedColors(XElement rootElement)
        {
            var colors = new Dictionary<string, (byte r, byte g, byte b)>();
            foreach (var colorElement in rootElement.Elements("color"))
            {
                var name = colorElement.AttributeValue("name");
                var r = colorElement.AttributeValue("r");
                var g = colorElement.AttributeValue("g");
                var b = colorElement.AttributeValue("b");
                colors.Add(name, (Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b)));
            }

            return colors;
        }
    }
}
