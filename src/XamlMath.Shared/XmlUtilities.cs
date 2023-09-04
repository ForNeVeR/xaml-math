using System;
using System.Globalization;
using System.Xml.Linq;

namespace XamlMath;

internal static class XmlUtilities
{
    public static bool AttributeBooleanValue(this XElement element, string attributeName, bool? defaultValue = null)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute == null)
        {
            if (defaultValue != null)
                return defaultValue.Value;
            throw new InvalidOperationException();
        }
        return bool.Parse(attribute.Value);
    }

    public static int AttributeInt32Value(this XElement element, string attributeName, int? defaultValue = null)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute == null)
        {
            if (defaultValue != null)
                return defaultValue.Value;
            throw new InvalidOperationException();
        }
        return int.Parse(attribute.Value, CultureInfo.InvariantCulture);
    }

    public static double AttributeDoubleValue(this XElement element, string attributeName, double? defaultValue = null)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute == null)
        {
            if (defaultValue != null)
                return defaultValue.Value;
            throw new InvalidOperationException();
        }
        return double.Parse(attribute.Value, CultureInfo.InvariantCulture);
    }

    public static string AttributeValue(this XElement element, string attributeName, string? defaultValue = null)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute == null)
        {
            if (defaultValue != null)
                return defaultValue;
            throw new InvalidOperationException();
        }
        return attribute.Value;
    }
}
