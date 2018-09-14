using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using WpfMath.Atoms;
using WpfMath.Utils;

namespace WpfMath.Parsers
{
    internal class InternalMathSymbolParser
    {
        //private static string resourceName = TexUtilities.ResourcesDataDirectory;
        private static IDictionary<string, TexAtomType> typeMappings;

        static InternalMathSymbolParser()
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
            typeMappings.Add("over", TexAtomType.Over);
            typeMappings.Add("under", TexAtomType.Under);
        }

        private XmlElement rootElement;

        public string SymbolsFilePath { get; private set; }
        public bool AreInternal { get; private set; }

        /// <summary>
        /// Initializes the <see cref="InternalMathSymbolParser"/> with the specified symbols resource directory.
        /// </summary>
        /// <param name="symbolsfilepath">The directory containing the symbols file.
        /// <para/>Use a period ('.') to separate directories and not slashes('/' or '\').
        /// </param>
        public InternalMathSymbolParser(string symbolsfilepath= "WpfMath.Data.TexSymbols.xml",bool isInternal=true)
        {
            SymbolsFilePath = symbolsfilepath;
            AreInternal = isInternal;
            //resourceName = symbolsfilepath;
            if (isInternal==true)
            {
                // for 3.5
                XmlDocument symbolsDocument = new XmlDocument();
                symbolsDocument.Load(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(symbolsfilepath)));
                if (symbolsDocument.DocumentElement.Name=="TeXSymbols"&& symbolsDocument.DocumentElement.HasChildNodes==true)
                {
                    this.rootElement = symbolsDocument.DocumentElement;
                }
            }
            else
            {
                XmlDocument symbolsDocument = new XmlDocument();
                
                symbolsDocument.Load(symbolsfilepath);
                if (symbolsDocument.DocumentElement.Name == "TeXSymbols" && symbolsDocument.DocumentElement.HasChildNodes == true)
                {
                    this.rootElement = symbolsDocument.DocumentElement;
                }
            }
        }

        public IDictionary<string, Func<SourceSpan, SymbolAtom>> GetSymbols()
        {
            var result = new Dictionary<string, Func<SourceSpan, SymbolAtom>>();

            foreach (var symbolElement in rootElement.GetXmlNodes("Symbol"))
            {
                string symbolName = symbolElement.Attribute_Value("name");
                string symbolType = symbolElement.Attribute_Value("type");
                bool symbolIsDelimeter = symbolElement.Attribute_BoolValue("del");
                if (symbolName != null && symbolType != null&&result.ContainsKey(symbolName))
                {
                    Debug.WriteLine("duplicate symbol name: " + symbolName);
                }
                if (symbolName!=null && symbolType != null&&result.ContainsKey(symbolName)==false)
                {

                    result.Add(
                    symbolName,
                    source => new SymbolAtom(source, symbolName, typeMappings[symbolType], symbolIsDelimeter,SymbolsFilePath,AreInternal));

                }
                
            }

            return result;
        }

        /// <summary>
        /// Returns a list of declared delimiter symbols in the current symbols data file.
        /// </summary>
        /// <returns></returns>
        public List<string> GetDelimiters()
        {
            var result = new List<string>();
            foreach (var symbolElement in rootElement.GetXmlNodes("Symbol"))
            {
                string symbolName = symbolElement.Attribute_Value("name");
                string symbolType = symbolElement.Attribute_Value("type");
                bool symbolIsDelimeter = symbolElement.Attribute_BoolValue("del");

                if (symbolName != null && symbolType != null&&symbolIsDelimeter)
                {
                    result.Add(symbolName);
                }
            }
            return result;
        }

    }
}
