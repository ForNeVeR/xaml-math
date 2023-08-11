using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using XamlMath.Data;
using XamlMath.Utils;

namespace XamlMath
{
    // Parses settings for predefined formulas from XML file.
    internal sealed class TexPredefinedFormulaSettingsParser
    {
        private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "TexFormulaSettings.xml";

        static TexPredefinedFormulaSettingsParser()
        {
        }

        private static void AddToMap(IEnumerable<XElement> mapList, string[] table)
        {
            foreach (var map in mapList)
            {
                var character = map.AttributeValue("char");
                var symbol = map.AttributeValue("symbol");
                Debug.Assert(character != null);
                Debug.Assert(symbol != null);
                Debug.Assert(character.Length == 1);
                table[character[0]] = symbol;
            }
        }

        private readonly XElement rootElement;

        public TexPredefinedFormulaSettingsParser()
        {
            using var resource = typeof(XamlMathResourceMarker).Assembly.ReadResource(resourceName);
            var doc = XDocument.Load(resource);
            this.rootElement = doc.Root;
        }

        public IReadOnlyList<string> GetSymbolMappings()
        {
            var mappings = new string[TexFontInfo.charCodesCount];
            var charToSymbol = rootElement.Element("CharacterToSymbolMappings");
            if (charToSymbol != null)
                AddToMap(charToSymbol.Elements("Map"), mappings);
            return mappings;
        }

        public IReadOnlyList<string> GetDelimiterMappings()
        {
            var mappings = new string[TexFontInfo.charCodesCount];
            var charToDelimiter = rootElement.Element("CharacterToDelimiterMappings");
            if (charToDelimiter != null)
                AddToMap(charToDelimiter.Elements("Map"), mappings);
            return mappings;
        }

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
}
