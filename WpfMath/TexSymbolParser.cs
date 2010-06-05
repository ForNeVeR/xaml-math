using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

// Parse definitions of symbols from XML files.
internal class TexSymbolParser
{
    private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "TexSymbols.xml";

    private static IDictionary<string, TexAtomType> typeMappings;

    static TexSymbolParser()
    {
        typeMappings = new Dictionary<string, TexAtomType>();

        SetTypeMappings();
    }

    private static void SetTypeMappings()
    {
        typeMappings.Add("ord", TexAtomType.Ordinary);
        typeMappings.Add("op", TexAtomType.BigOperator);
        typeMappings.Add("bin", TexAtomType.BinaryOperator);
        typeMappings.Add("rel", TexAtomType.Relation);
        typeMappings.Add("open", TexAtomType.Opening);
        typeMappings.Add("close", TexAtomType.Closing);
        typeMappings.Add("punct", TexAtomType.Punctuation);
        typeMappings.Add("acc", TexAtomType.Accent);
    }

    private XElement rootElement;

    public TexSymbolParser()
    {
        var doc = XDocument.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
        this.rootElement = doc.Root;
    }

    public IDictionary<string, SymbolAtom> GetSymbols()
    {
        var result = new Dictionary<string, SymbolAtom>();

        foreach (var symbolElement in rootElement.Elements("Symbol"))
        {
            var symbolName = symbolElement.AttributeValue("name");
            var symbolType = symbolElement.AttributeValue("type");
            var symbolIsDelimeter = symbolElement.AttributeBooleanValue("del", false);

            result.Add(symbolName, new SymbolAtom(symbolName, (TexAtomType)typeMappings[symbolType],
                symbolIsDelimeter));
        }

        return result;
    }
}
