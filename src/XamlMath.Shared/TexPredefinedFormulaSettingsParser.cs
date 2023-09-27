using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using XamlMath.Data;
using XamlMath.Utils;

namespace XamlMath;

// Parses settings for predefined formulas from XML file.
internal sealed class TexPredefinedFormulaSettingsParser
{
    private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "TexFormulaSettings.xml";

    private readonly XElement rootElement;

    public TexPredefinedFormulaSettingsParser()
    {
        using var resource = typeof(XamlMathResourceMarker).Assembly.ReadResource(resourceName);
        var doc = XDocument.Load(resource);
        this.rootElement = doc.Root;
    }

    private readonly record struct CharMappingPair(char Key, string Value);

    private static CharMappingPair ExtractCharMappingPair(XElement mapTag)
    {
        var character = mapTag.AttributeValue("char");
        var symbol = mapTag.AttributeValue("symbol");
        Debug.Assert(character != null);
        Debug.Assert(symbol != null);
        Debug.Assert(character.Length == 1);
        return new(character[0], symbol);
    }

    private IReadOnlyList<string> GetMappingsFromElement(XName elementName)
    {
        var mappings = new string[TexFontInfo.charCodesCount];
        var charToSymbol = rootElement.Element(elementName);
        if (charToSymbol != null)
        {
            var additionsToMap = charToSymbol.Elements("Map").Select(ExtractCharMappingPair);
            foreach (var addition in additionsToMap)
            {
                mappings[addition.Key] = addition.Value;
            }
        }
        return mappings;
    }

    public IReadOnlyList<string> GetSymbolMappings() => GetMappingsFromElement("CharacterToSymbolMappings");

    public IReadOnlyList<string> GetDelimiterMappings() => GetMappingsFromElement("CharacterToDelimiterMappings");

    public IEnumerable<string> GetTextStyles()
    {
        var textStyles = rootElement.Element("TextStyles");
        if (textStyles == null) yield break;
        foreach (var textStyleElement in textStyles.Elements("TextStyle"))
        {
            var name = textStyleElement.AttributeValue("name");
            Debug.Assert(name != null);
            yield return name;
        }
    }
}
