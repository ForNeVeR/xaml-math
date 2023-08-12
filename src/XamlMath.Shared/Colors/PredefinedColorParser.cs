using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using static XamlMath.Utils.ColorHelpers;

namespace XamlMath.Colors;

public sealed class PredefinedColorParser : IColorParser
{
    private const string ResourceName = TexUtilities.ResourcesDataDirectory + "PredefinedColors.xml";

    private readonly IReadOnlyDictionary<string, RgbaColor> _colors;

    public static readonly PredefinedColorParser Instance = new(ResourceName);

    private PredefinedColorParser(string resourceName)
    {
        using var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        var doc = XDocument.Load(resource);
        _colors = Parse(doc!.Root!);
    }

    public RgbaColor? Parse(IReadOnlyList<string> components)
    {
        var hasAlphaComponent = components.Count == 2;
        if (components.Count != 1 && !hasAlphaComponent)
            return null;

        var colorName = components[0];
        if (!_colors.TryGetValue(colorName, out var color))
            return null;

        byte alpha = 255;
        if (hasAlphaComponent)
        {
            var alphaFraction = ParseFloatColorComponent(components[1], NumberStyles.AllowDecimalPoint);
            if (!alphaFraction.HasValue)
                return null;

            alpha = ConvertToByteRgbComponent(alphaFraction.Value);
        }

        color.A = alpha;
        return color;
    }

    private static Dictionary<string, RgbaColor> Parse(XElement rootElement)
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
