using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using XamlMath.Data;
using XamlMath.Fonts;
using XamlMath.Utils;

namespace XamlMath;

// Parses information for DefaultTeXFont settings from XML file.
internal sealed class DefaultTexFontParser
{
    private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "DefaultTexFont.xml";

    private const int fontIdCount = 5;

    private static readonly IReadOnlyDictionary<string, int> rangeTypeMappings;
    private static readonly IReadOnlyDictionary<string, ICharChildParser> charChildParsers;

    private readonly IFontProvider _fontProvider;

    static DefaultTexFontParser()
    {
        rangeTypeMappings = new Dictionary<string, int>
        {
            ["numbers"] = (int)TexCharKind.Numbers,
            ["capitals"] = (int)TexCharKind.Capitals,
            ["small"] = (int)TexCharKind.Small,
        };

        charChildParsers = new Dictionary<string, ICharChildParser>
        {
            ["Kern"] = new KernParser(),
            ["Lig"] = new LigParser(),
            ["NextLarger"] = new NextLargerParser(),
            ["Extension"] = new ExtensionParser(),
        };
    }

    private readonly IReadOnlyDictionary<string, IReadOnlyList<CharFont>> parsedTextStyles;

    private readonly XElement rootElement;

    public DefaultTexFontParser(IFontProvider fontProvider)
    {
        _fontProvider = fontProvider;

        using var resource = typeof(XamlMathResourceMarker).Assembly.ReadResource(resourceName);
        var doc = XDocument.Load(resource);
        this.rootElement = doc.Root;

        this.parsedTextStyles = CreateParseTextStyleMappings(doc.Root);
    }

    public IReadOnlyList<TexFontInfo> GetFontDescriptions()
    {
        var result = new TexFontInfo[fontIdCount];

        var fontDescriptions = rootElement.Element("FontDescriptions");
        if (fontDescriptions != null)
        {
            foreach (var fontElement in fontDescriptions.Elements("Font"))
            {
                var fontName = fontElement.AttributeValue("name");
                var fontId = fontElement.AttributeInt32Value("id");
                var space = fontElement.AttributeDoubleValue("space");
                var xHeight = fontElement.AttributeDoubleValue("xHeight");
                var quad = fontElement.AttributeDoubleValue("quad");
                var skewChar = fontElement.AttributeInt32Value("skewChar", -1);

                var font = _fontProvider.ReadFontFile(fontName);
                var fontInfo = new TexFontInfo(fontId, font, xHeight, space, quad);
                if (skewChar != -1)
                    fontInfo.SkewCharacter = (char)skewChar;

                foreach (var charElement in fontElement.Elements("Char"))
                    ProcessCharElement(charElement, fontInfo);

                if (result[fontId] != null)
                    throw new InvalidOperationException($"Multiple entries for font with ID {fontId}.");
                result[fontId] = fontInfo;
            }
        }

        return result;
    }

    private static void ProcessCharElement(XElement charElement, TexFontInfo fontInfo)
    {
        var character = (char)charElement.AttributeInt32Value("code");

        var metrics = new double[4];
        metrics[TexFontUtilities.MetricsWidth] = charElement.AttributeDoubleValue("width", 0d);
        metrics[TexFontUtilities.MetricsHeight] = charElement.AttributeDoubleValue("height", 0d);
        metrics[TexFontUtilities.MetricsDepth] = charElement.AttributeDoubleValue("depth", 0d);
        metrics[TexFontUtilities.MetricsItalic] = charElement.AttributeDoubleValue("italic", 0d);
        fontInfo.SetMetrics(character, metrics);

        foreach (var childElement in charElement.Elements())
        {
            var parser = charChildParsers[childElement.Name.ToString()];
            if (parser == null)
                throw new InvalidOperationException("Unknown element type.");
            parser.Parse(childElement, character, fontInfo);
        }
    }

    public IReadOnlyDictionary<string, CharFont> GetSymbolMappings()
    {
        var result = new Dictionary<string, CharFont>();

        var symbolMappingsElement = rootElement.Element("SymbolMappings");
        if (symbolMappingsElement == null)
            throw new InvalidOperationException("Cannot find SymbolMappings element.");

        foreach (var mappingElement in symbolMappingsElement.Elements("SymbolMapping"))
        {
            var symbolName = mappingElement.AttributeValue("name");
            var character = mappingElement.AttributeInt32Value("ch");
            var fontId = mappingElement.AttributeInt32Value("fontId");

            result.Add(symbolName, new CharFont((char)character, fontId));
        }

        if (!result.ContainsKey("sqrt"))
            throw new InvalidOperationException("Cannot find SymbolMap element for 'sqrt'.");

        return result;
    }

    public IReadOnlyList<string> GetDefaultTextStyleMappings()
    {
        var result = new string[3];

        var defaultTextStyleMappings = rootElement.Element("DefaultTextStyleMapping");
        if (defaultTextStyleMappings == null)
            throw new InvalidOperationException("Cannot find DefaultTextStyleMapping element.");

        foreach (var mappingElement in defaultTextStyleMappings.Elements("MapStyle"))
        {
            var code = mappingElement.AttributeValue("code");
            var codeMapping = rangeTypeMappings[code];

            var textStyleName = mappingElement.AttributeValue("textStyle");

            var charFonts = parsedTextStyles[textStyleName];
            Debug.Assert(charFonts[codeMapping] != null);

            result[codeMapping] = textStyleName;
        }

        return result;
    }

    public IReadOnlyDictionary<string, double> GetParameters()
    {
        var result = new Dictionary<string, double>();

        var parameters = rootElement.Element("Parameters");
        if (parameters == null)
            throw new InvalidOperationException("Cannot find Parameters element.");

        foreach (var attribute in parameters.Attributes())
        {
            result.Add(attribute.Name.ToString(), parameters.AttributeDoubleValue(attribute.Name.ToString()));
        }

        return result;
    }

    public IReadOnlyDictionary<string, object> GetGeneralSettings()
    {
        var generalSettings = rootElement.Element("GeneralSettings");

        if (generalSettings == null)
            throw new InvalidOperationException("Cannot find GeneralSettings element.");

        return new Dictionary<string, object>
        {
            ["mufontid"] = generalSettings.AttributeInt32Value("mufontid"),
            ["spacefontid"] = generalSettings.AttributeInt32Value("spacefontid"),
            ["scriptfactor"] = generalSettings.AttributeDoubleValue("scriptfactor"),
            ["scriptscriptfactor"] = generalSettings.AttributeDoubleValue("scriptscriptfactor"),
        };
    }

    public IReadOnlyDictionary<string, IReadOnlyList<CharFont>> GetTextStyleMappings()
    {
        return parsedTextStyles;
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<CharFont>> CreateParseTextStyleMappings(XElement root)
    {
        var result = new Dictionary<string, IReadOnlyList<CharFont>>();
        var textStyleMappings = root.Element("TextStyleMappings");
        if (textStyleMappings == null) throw new InvalidOperationException("Cannot find TextStyleMappings element.");
        foreach (var mappingElement in textStyleMappings.Elements("TextStyleMapping"))
        {
            var textStyleName = mappingElement.AttributeValue("name");
            var charFonts = new CharFont[3];
            foreach (var mapRangeElement in mappingElement.Elements("MapRange"))
            {
                var fontId = mapRangeElement.AttributeInt32Value("fontId");
                var character = mapRangeElement.AttributeInt32Value("start");
                var code = mapRangeElement.AttributeValue("code");
                var codeMapping = rangeTypeMappings[code];

                charFonts[(int)codeMapping] = new CharFont((char)character, fontId);
            }
            result.Add(textStyleName, charFonts);
        }
        return result;
    }

    public sealed class ExtensionParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo fontInfo)
        {
            var extensionChars = new int[4];
            extensionChars[TexFontUtilities.ExtensionRepeat] = element.AttributeInt32Value("rep");
            extensionChars[TexFontUtilities.ExtensionTop] = element.AttributeInt32Value("top",
                (int)TexCharKind.None);
            extensionChars[TexFontUtilities.ExtensionMiddle] = element.AttributeInt32Value("mid",
                (int)TexCharKind.None);
            extensionChars[TexFontUtilities.ExtensionBottom] = element.AttributeInt32Value("bot",
                (int)TexCharKind.None);

            fontInfo.SetExtensions(character, extensionChars);
        }
    }

    public sealed class KernParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo fontInfo)
        {
            fontInfo.AddKern(character, (char)element.AttributeInt32Value("code"),
                element.AttributeDoubleValue("val"));
        }
    }

    public sealed class LigParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo fontInfo)
        {
            fontInfo.AddLigature(character, (char)element.AttributeInt32Value("code"),
                (char)element.AttributeInt32Value("ligCode"));
        }
    }

    public sealed class NextLargerParser : ICharChildParser
    {
        public void Parse(XElement element, char character, TexFontInfo fontInfo)
        {
            fontInfo.SetNextLarger(character, (char)element.AttributeInt32Value("code"),
                element.AttributeInt32Value("fontId"));
        }
    }

    public interface ICharChildParser
    {
        void Parse(XElement element, char character, TexFontInfo fontInfo);
    }
}
