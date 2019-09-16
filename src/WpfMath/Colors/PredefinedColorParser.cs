using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;

namespace WpfMath.Colors
{
    internal class PredefinedColorParser : IColorParser
    {
        private const string ResourceName = TexUtilities.ResourcesDataDirectory + "PredefinedColors.xml";
        public static readonly PredefinedColorParser Instance = new PredefinedColorParser(ResourceName);

        private readonly IReadOnlyDictionary<string, Color> _colors;

        private PredefinedColorParser(string resourceName)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                var doc = XDocument.Load(resource);
                _colors = Parse(doc.Root);
            }
        }

        public Color? Parse(IEnumerable<string> components)
        {
            // Return a color iff components contain only one element, and it is contained in the _colors dictionary.
            var firstItem = true;
            Color? color = null;
            foreach (var component in components)
            {
                if (!firstItem)
                    return null;

                var colorName = component;
                if (_colors.TryGetValue(colorName, out var parsedColor))
                    color = parsedColor;

                firstItem = false;
            }

            return color;
        }

        private Dictionary<string, Color> Parse(XElement rootElement)
        {
            var colors = new Dictionary<string, Color>();
            foreach (var colorElement in rootElement.Elements("color"))
            {
                var name = colorElement.AttributeValue("name");
                var r = colorElement.AttributeValue("r");
                var g = colorElement.AttributeValue("g");
                var b = colorElement.AttributeValue("b");
                colors.Add(name, Color.FromRgb(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b)));
            }

            return colors;
        }
    }
}
