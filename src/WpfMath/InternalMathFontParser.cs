using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using WpfMath.Utils;

namespace WpfMath.Parsers
{
    /// <summary>
    /// Represents a class that parses information for DefaultTeXFont settings from an internal (located in this library) XML file.
    /// </summary>
    internal class InternalMathFontParser
    {
        private static string resourceName = "" ;
        private static readonly IDictionary<string, int> rangeTypeMappings;
        private static readonly IDictionary<string, ICharChildParser> charChildParsers;

        static InternalMathFontParser()
        {
            rangeTypeMappings = new Dictionary<string, int>();
            charChildParsers = new Dictionary<string, ICharChildParser>();

            SetRangeTypeMappings();
            SetCharChildParsers();
        }

        private static void SetRangeTypeMappings()
        {
            rangeTypeMappings.Add("Digit", (int)TexCharKind.Digit);
            rangeTypeMappings.Add("EnglishCapital", (int)TexCharKind.EnglishCapital);
            rangeTypeMappings.Add("EnglishSmall", (int)TexCharKind.EnglishSmall);
        }

        private static void SetCharChildParsers()
        {
            charChildParsers.Add("Kern", new KernParser());
            charChildParsers.Add("Lig", new LigParser());
            charChildParsers.Add("NextLarger", new NextLargerParser());
            charChildParsers.Add("Extension", new ExtensionParser());
        }

        private IDictionary<string, CharFont[]> parsedTextStyles;

        private XmlElement rootElement;

        /// <summary>
        /// Gets or sets the number of fonts described in the current math font.
        /// </summary>
        public int FontIdCount { get; private set; }

        /// <summary>
        /// Gets or sets the directory of the current math font's font file(s).
        /// </summary>
        public string FontsDirectory { get; private set; } 

        public bool IsFontInternal { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="InternalMathFontParser"/>.
        /// </summary>
        /// <param name="MathFontInfoPath">The path containing the math font settings information.
        /// <para/>Use a period ('.') to separate directories and not slashes('/' or '\').
        /// </param>
        /// <param name="MathFontsDirectory">The directory to the fonts described in the <paramref name="MathFontInfoPath"/>.</param>
        /// <param name="FontsCount">The number of fonts described in the <paramref name="MathFontInfoPath"/> XML file.</param>
        public InternalMathFontParser(string MathFontInfoPath= "WpfMath.Data.DefaultTexFont.xml", string MathFontsDirectory= "Fonts/Default/",int FontsCount=4,bool isInternal=true)
        {
            resourceName = MathFontInfoPath;
            FontsDirectory = MathFontsDirectory;
            FontIdCount = FontsCount;
            IsFontInternal = isInternal;
            if (isInternal)
            {
                XmlDocument fontInfoDocument = new XmlDocument();
                fontInfoDocument.Load(new System.IO.StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(MathFontInfoPath)));
                if (fontInfoDocument.DocumentElement.Name=="TeXFont"&&fontInfoDocument.HasChildNodes==true)
                {
                    this.rootElement = fontInfoDocument.DocumentElement;
                }
            }
            else
            {
                XmlDocument fontInfoDocument = new XmlDocument();
                fontInfoDocument.Load(MathFontInfoPath);
                if (fontInfoDocument.DocumentElement.Name == "TeXFont" && fontInfoDocument.HasChildNodes == true)
                {
                    this.rootElement = fontInfoDocument.DocumentElement;
                }
            }
            ParseTextStyleMappings();
        }

        public TexFontInfo[] GetFontDescriptions()
        {
            var result = new TexFontInfo[FontIdCount];

            var fontDescriptions = rootElement.GetXmlNode("FontDescriptions"); 
            if (fontDescriptions != null)
            {
                foreach (var fontNode in fontDescriptions.GetXmlNodes("Font"))
                {
                    string fontName = fontNode.Attribute_Value("name");
                    int fontId = fontNode.Attribute_IntValue("id");
                    double space = fontNode.Attribute_DoubleValue("space");
                    double xHeight = fontNode.Attribute_DoubleValue("xHeight");
                    double quad = fontNode.Attribute_DoubleValue("quad");
                    int skewChar = fontNode.Attribute_IntValue("skewChar", -1);

                    var font = CreateFont(fontName);
                    var fontInfo = new TexFontInfo(fontId, font, xHeight, space, quad);
                    if (skewChar != -1)
                        fontInfo.SkewCharacter = (char)skewChar;

                    foreach (var charElement in fontNode.GetXmlNodes("Char"))
                        ProcessCharElement(charElement, fontInfo);

                    if (result[fontId] != null)
                        throw new InvalidOperationException(string.Format("Multiple entries for font with ID {0}.", fontId));
                    result[fontId] = fontInfo;
                }
            }

            return result;
        }

        private static void ProcessCharElement(XmlNode charElement, TexFontInfo fontInfo)
        {
            var character = (char)charElement.Attribute_IntValue("code");

            var metrics = new double[4];
            metrics[TexFontUtilities.MetricsWidth] = charElement.Attribute_DoubleValue("width", 0d);
            metrics[TexFontUtilities.MetricsHeight] = charElement.Attribute_DoubleValue("height", 0d);
            metrics[TexFontUtilities.MetricsDepth] = charElement.Attribute_DoubleValue("depth", 0d);
            metrics[TexFontUtilities.MetricsItalic] = charElement.Attribute_DoubleValue("italic", 0d);
            fontInfo.SetMetrics(character, metrics);

            if (charElement.GetXmlNodes().Count>0)
            {
                foreach (var childElement in charElement.GetXmlNodes())
                {
                    if (charChildParsers.ContainsKey(childElement.Name))
                    {
                        var parser = charChildParsers[childElement.Name];
                        if (parser == null)
                            throw new InvalidOperationException("Unknown element type.");
                        parser.Parse(childElement, character, fontInfo);
                    }
                    
                }
            }
            
        }

        public IDictionary<string, CharFont> GetSymbolMappings()
        {
            var result = new Dictionary<string, CharFont>();

            var symbolMappingsElement = rootElement.GetXmlNode("SymbolMappings");
            if (symbolMappingsElement == null)
                throw new InvalidOperationException("Cannot find SymbolMappings element.");

            foreach (var mappingElement in symbolMappingsElement.GetXmlNodes("SymbolMapping"))
            {
                string symbolName = mappingElement.Attribute_Value("name");
                int character = mappingElement.Attribute_IntValue("ch");
                int fontId = mappingElement.Attribute_IntValue("fontId");
                if (symbolName!=null&&!result.ContainsKey(symbolName))
                {
                    result.Add(symbolName, new CharFont((char)character, fontId));
                }
            }

            if (!result.ContainsKey("sqrt"))
                throw new InvalidOperationException("Cannot find SymbolMap element for 'sqrt'.");

            return result;
        }

        public List<string> GetSymbolNames()
        {
            var result = new List<string>();

            var symbolMappingsElement = rootElement.GetXmlNode("SymbolMappings");
            if (symbolMappingsElement == null)
                throw new InvalidOperationException("Cannot find SymbolMappings element.");

            foreach (var mappingElement in symbolMappingsElement.GetXmlNodes("SymbolMapping"))
            {
                string symbolName = mappingElement.Attribute_Value("name");
                
                if (symbolName != null && result.Contains(symbolName)==false)
                {
                    result.Add(symbolName);
                }
            }
            return result;
        }

        public IList<string> GetDefaultTextStyleMappings()
        {
            var result = new string[5];

            var defaultTextStyleMappings = rootElement.GetXmlNode("DefaultTextStyleMapping");
            if (defaultTextStyleMappings == null)
                throw new InvalidOperationException("Cannot find DefaultTextStyleMapping element.");

            foreach (var mappingElement in defaultTextStyleMappings.GetXmlNodes("MapStyle"))
            {
                string code = mappingElement.Attribute_Value("code");
                var codeMapping = rangeTypeMappings[code];

                var textStyleName = mappingElement.Attribute_Value("textStyle");
                var textStyleMapping = parsedTextStyles[textStyleName];

                var charFonts = parsedTextStyles[textStyleName];
                Debug.Assert(charFonts[codeMapping] != null);

                result[codeMapping] = textStyleName;
            }
            
            return result;
        }

        public IDictionary<string, double> GetParameters()
        {
            var result = new Dictionary<string, double>();

            var parameters = rootElement.GetXmlNode("Parameters");
            if (parameters == null)
                throw new InvalidOperationException("Cannot find Parameters element.");
            
            foreach (XmlAttribute attribute in parameters.Attributes)
            {
                result.Add(attribute.Name.ToString(), parameters.Attribute_DoubleValue(attribute.Name.ToString()));
            }

            return result;
        }

        public IDictionary<string, object> GetGeneralSettings()
        {
            var result = new Dictionary<string, object>();

            var generalSettings = rootElement.GetXmlNode("GeneralSettings");
            if (generalSettings == null)
                throw new InvalidOperationException("Cannot find GeneralSettings element.");

            result.Add("mufontid", generalSettings.Attribute_IntValue("mufontid"));
            result.Add("spacefontid", generalSettings.Attribute_IntValue("spacefontid"));
            result.Add("scriptfactor", generalSettings.Attribute_DoubleValue("scriptfactor"));
            result.Add("scriptscriptfactor", generalSettings.Attribute_DoubleValue("scriptscriptfactor"));

            return result;
        }

        public IDictionary<string, CharFont[]> GetTextStyleMappings()
        {
            return parsedTextStyles;
        }

        private void ParseTextStyleMappings()
        {
            this.parsedTextStyles = new Dictionary<string, CharFont[]>();

            var textStyleMappings = rootElement.GetXmlNode("TextStyleMappings");
            if (textStyleMappings == null)
                throw new InvalidOperationException("Cannot find TextStyleMappings element.");

            foreach (var mappingElement in textStyleMappings.GetXmlNodes("TextStyleMapping"))
            {
                var textStyleName = mappingElement.Attribute_Value("name");
                var charFonts = new CharFont[FontIdCount];
                foreach (var mapRangeElement in mappingElement.GetXmlNodes("MapRange"))
                {
                    var fontId = mapRangeElement.Attribute_IntValue("fontId");
                    Debug.WriteLine("Text Style Mapping Font ID: "+fontId);
                    var character = mapRangeElement.Attribute_IntValue("start");
                    Debug.WriteLine("Text Style Mapping Character: " + character);
                    var code = mapRangeElement.Attribute_Value("code");
                    Debug.WriteLine("Text Style Mapping Code: " + code);
                    int codeMapping = rangeTypeMappings.ContainsKey(code)? rangeTypeMappings[code]:40;

                    charFonts[(int)codeMapping] = new CharFont((char)character, fontId);
                }
                this.parsedTextStyles.Add(textStyleName, charFonts);
            }
        }

        private GlyphTypeface CreateFont(string name)
        {
            if (IsFontInternal)
            {
                // Load font from embedded resource.
                var fontUri = new Uri(string.Format("pack://application:,,,/WpfMath;component/{0}{1}", FontsDirectory, name));
                
                return new GlyphTypeface(fontUri);
            }
            else
            {
                // Load font from embedded resource.
                var fontUri = new Uri( FontsDirectory+ name);
                return new GlyphTypeface(fontUri);
            }
        }

        public class ExtensionParser : ICharChildParser
        {
            public ExtensionParser()
            {
            }

            public void Parse(XmlNode element, char character, TexFontInfo fontInfo)
            {
                var extensionChars = new int[6];
                extensionChars[TexFontUtilities.ExtensionRepeat] = element.Attribute_IntValue("rep");
                extensionChars[TexFontUtilities.ExtensionTop] = element.Attribute_IntValue("top",
                    (int)TexCharKind.None);
                extensionChars[TexFontUtilities.ExtensionMiddle] = element.Attribute_IntValue("mid",
                    (int)TexCharKind.None);
                extensionChars[TexFontUtilities.ExtensionBottom] = element.Attribute_IntValue("bot",
                    (int)TexCharKind.None);
                extensionChars[TexFontUtilities.ExtensionLeft] = element.Attribute_IntValue("left",
                    (int)TexCharKind.None);
                extensionChars[TexFontUtilities.ExtensionRight] = element.Attribute_IntValue("right",
                    (int)TexCharKind.None);

                fontInfo.SetExtensions(character, extensionChars);
            }
        }

        public class KernParser : ICharChildParser
        {
            public KernParser()
            {
            }

            public void Parse(XmlNode element, char character, TexFontInfo fontInfo)
            {
                fontInfo.AddKern(character, (char)element.Attribute_IntValue("code"),
                    element.Attribute_DoubleValue("val"));
            }
        }

        public class LigParser : ICharChildParser
        {
            public LigParser()
            {
            }

            public void Parse(XmlNode element, char character, TexFontInfo fontInfo)
            {
                fontInfo.AddLigature(character, (char)element.Attribute_IntValue("code"),
                    (char)element.Attribute_IntValue("ligCode"));
            }
        }

        public class NextLargerParser : ICharChildParser
        {
            public NextLargerParser()
            {
            }

            public void Parse(XmlNode element, char character, TexFontInfo fontInfo)
            {
                fontInfo.SetNextLarger(character, (char)element.Attribute_IntValue("code"),
                    element.Attribute_IntValue("fontId"));
            }
        }

        public interface ICharChildParser
        {
            void Parse(XmlNode element, char character, TexFontInfo fontInfo);
        }

    }
}
