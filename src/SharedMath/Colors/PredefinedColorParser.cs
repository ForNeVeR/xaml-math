using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    public class PredefinedColorParser : IColorParser
    {
        private const string ResourceName = TexUtilities.ResourcesDataDirectory + "PredefinedColors.xml";
        public static readonly PredefinedColorParser Instance = new PredefinedColorParser(ResourceName);

        private static IReadOnlyDictionary<string, ArgbColor> _colors;

        private PredefinedColorParser(string resourceName)
        {
            using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            var doc = XDocument.Load(resource);
            _colors = Parse(doc.Root);
        }

        public ArgbColor? Parse(IReadOnlyList<string> components)
        {
            var hasAlphaComponent = components.Count == 2;
            if (components.Count != 1 && !hasAlphaComponent)
                return null;

            var colorName = components[0];
            if (!_colors.TryGetValue(colorName, out var color))
                return null;

            byte? alpha = 255;
            if (hasAlphaComponent)
            {
                var alphaFraction = ColorHelpers.ParseFloatColorComponent(components[1], NumberStyles.AllowDecimalPoint);
                if (!alphaFraction.HasValue)
                    return null;

                alpha = ColorHelpers.ConvertToByteRgbComponent(alphaFraction.Value);
            }

            if (!alpha.HasValue)
                return null;

            color.A = alpha.Value;
            return color;
        }

        private static Dictionary<string, ArgbColor> Parse(XElement rootElement)
        {
            var colors = new Dictionary<string, ArgbColor>();
            foreach (var colorElement in rootElement.Elements("color"))
            {
                var name = colorElement.AttributeValue("name");
                var r = colorElement.AttributeValue("r");
                var g = colorElement.AttributeValue("g");
                var b = colorElement.AttributeValue("b");
                colors.Add(name, new ArgbColor(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b)));
            }

            return colors;
        }
    }
}
