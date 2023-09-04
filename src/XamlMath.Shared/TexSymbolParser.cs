using System;
using System.Collections.Generic;
using System.Xml.Linq;
using XamlMath.Atoms;
using XamlMath.Data;
using XamlMath.Utils;

namespace XamlMath;

// Parse definitions of symbols from XML files.
internal sealed class TexSymbolParser
{
    private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "TexSymbols.xml";

    private static readonly IReadOnlyDictionary<string, TexAtomType> typeMappings;

    static TexSymbolParser()
    {
        typeMappings = new Dictionary<string, TexAtomType>
        {
            ["ord"] = TexAtomType.Ordinary,
            ["op"] = TexAtomType.BigOperator,
            ["bin"] = TexAtomType.BinaryOperator,
            ["rel"] = TexAtomType.Relation,
            ["open"] = TexAtomType.Opening,
            ["close"] = TexAtomType.Closing,
            ["punct"] = TexAtomType.Punctuation,
            ["acc"] = TexAtomType.Accent,
        };
    }

    private readonly XElement rootElement;

    public TexSymbolParser()
    {
        using var resource = typeof(XamlMathResourceMarker).Assembly.ReadResource(resourceName);
        var doc =  XDocument.Load(resource);
        this.rootElement = doc.Root;
    }

    public IReadOnlyDictionary<string, Func<SourceSpan?, SymbolAtom>> GetSymbols()
    {
        var result = new Dictionary<string, Func<SourceSpan?, SymbolAtom>>();

        foreach (var symbolElement in rootElement.Elements("Symbol"))
        {
            var symbolName = symbolElement.AttributeValue("name");
            var symbolType = symbolElement.AttributeValue("type");
            var symbolIsDelimeter = symbolElement.AttributeBooleanValue("del", false);

            result.Add(
                symbolName,
                source => new SymbolAtom(source, symbolName, typeMappings[symbolType], symbolIsDelimeter));
        }

        return result;
    }
}
