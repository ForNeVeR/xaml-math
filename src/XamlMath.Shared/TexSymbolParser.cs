using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WpfMath.Atoms;
using WpfMath.Data;
using WpfMath.Utils;

namespace WpfMath
{
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
            using var resource = typeof(XamlMathResourceMarker).Assembly.ReadResource(resourceName);
            var doc =  XDocument.Load(resource);
            this.rootElement = doc.Root;
        }

        public IDictionary<string, Func<SourceSpan?, SymbolAtom>> GetSymbols()
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
}
