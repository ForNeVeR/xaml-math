using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using WpfMath.Utils;

namespace WpfMath.Parsers
{
    /// <summary>
    /// Represents a class that parses settings for predefined formulas from an XML file.
    /// </summary>
    internal class InternalTexFormulaSettingsParser
    {

        static InternalTexFormulaSettingsParser()
        {
        }

        private static void AddToMap(IEnumerable<XmlNode> mapList, string[] table)
        {
            foreach (var map in mapList)
            {
                var character = map.Attribute_Value("char");
                var symbol = map.Attribute_Value("symbol");
                Debug.Assert(character != null);
                Debug.Assert(symbol != null);
                Debug.Assert(character.Length == 1);
                table[character[0]] = symbol;
            }
        }

        private XmlElement rootElement;

        public InternalTexFormulaSettingsParser(string settingsPath= "WpfMath.Data.TexFormulaSettings.xml",bool isInternal=true)
        {
            if (isInternal)
            {
                XmlDocument formulaSettingsDocument = new XmlDocument();
                formulaSettingsDocument.Load(new System.IO.StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(settingsPath)));
                if (formulaSettingsDocument.DocumentElement.Name == "FormulaSettings" && formulaSettingsDocument.HasChildNodes == true)
                {
                    this.rootElement = formulaSettingsDocument.DocumentElement;
                }
            }
            else
            {
                XmlDocument formulaSettingsDocument = new XmlDocument();
                formulaSettingsDocument.Load(settingsPath);
                if (formulaSettingsDocument.DocumentElement.Name == "FormulaSettings" && formulaSettingsDocument.HasChildNodes == true)
                {
                    this.rootElement = formulaSettingsDocument.DocumentElement;
                }
            }
        }

        public IList<string> GetSymbolMappings()
        {
            var mappings = new string[TexFontInfo.charCodesCount];
            var charToSymbol = rootElement.GetXmlNode("CharacterToSymbolMappings");
            if (charToSymbol != null)
                AddToMap(charToSymbol.GetXmlNodes("Map"), mappings);
            return mappings;
        }

        public IList<string> GetDelimiterMappings()
        {
            var mappings = new string[TexFontInfo.charCodesCount];
            var charToDelimiter = rootElement.GetXmlNode("CharacterToDelimiterMappings");
            if (charToDelimiter != null)
                AddToMap(charToDelimiter.GetXmlNodes("Map"), mappings);
            return mappings;
        }

        public HashSet<string> GetTextStyles()
        {
            var result = new HashSet<string>();

            var textStyles = rootElement.GetXmlNode("TextStyles");
            if (textStyles != null)
            {
                foreach (var textStyleElement in textStyles.GetXmlNodes("TextStyle"))
                {
                    var name = textStyleElement.Attribute_Value("name");
                    Debug.Assert(name != null);
                    result.Add(name);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the default text style mapping for.
        /// </summary>
        /// <remarks>
        /// Item1->Digit
        /// Item2->EnglishCapital
        /// Item3->EnglishSmall
        /// Item4->GreekCapital
        /// Item5->GreekSmall
        /// </remarks>
        public Tuple<string, string, string, string, string> GetDefaultTextStyleMappings()
        {
            Tuple<string, string, string, string, string> result =new Tuple<string, string, string, string, string>("mathrm","mathrm","mathrm", "mathrm", "mathrm");

            var textStyles = rootElement.GetXmlNode("TextStyles");
            if (textStyles != null&&textStyles.HasChildNodes)
            {
                string DigitDefaultStyle = "mathrm";
                string EnglishCapitalDefaultStyle = "mathrm";
                string EnglishSmallDefaultStyle = "mathrm";
                string GreekCapitalDefaultStyle = "mathrm";
                string GreekSmallDefaultStyle = "mathrm";
                
                foreach (var textStyleElement in textStyles.GetXmlNodes("TextStyle"))
                {
                    var name = textStyleElement.Attribute_Value("name");
                    Debug.Assert(name != null);
                    if (textStyleElement.Attribute_BoolValue("DigitDefault"))
                    {
                        DigitDefaultStyle = name;
                    }
                    if (textStyleElement.Attribute_BoolValue("EnglishCapitalDefault"))
                    {
                        EnglishCapitalDefaultStyle = name;
                    }
                    if (textStyleElement.Attribute_BoolValue("EnglishSmallDefault"))
                    {
                        EnglishSmallDefaultStyle = name;
                    }
                    if (textStyleElement.Attribute_BoolValue("GreekCapitalDefault"))
                    {
                        GreekCapitalDefaultStyle = name;
                    }
                    if (textStyleElement.Attribute_BoolValue("GreekSmallDefault"))
                    {
                        GreekSmallDefaultStyle = name;
                    }
                }

                result = new Tuple<string, string, string, string, string>(DigitDefaultStyle, EnglishCapitalDefaultStyle, EnglishSmallDefaultStyle, GreekCapitalDefaultStyle, GreekSmallDefaultStyle);
                return result;
            }

            return result;
        }

    }
}
