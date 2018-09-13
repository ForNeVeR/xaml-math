using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMath.Boxes;
using WpfMath.Exceptions;
using WpfMath.Parsers;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents an atom that contains an alphanumeric item.
    /// </summary>
    internal class AlphaNumericAtom : Atom
    {
        public List<string> FontSymbolMappings { get; private set; }

        public AlphaNumericAtom(SourceSpan source,
            string alphanumericsymbol, string defaultalphanumericsymbol=null,
            TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
            AlphanumericSymbol = alphanumericsymbol;
            DefaultAlphanumericSymbol = defaultalphanumericsymbol??alphanumericsymbol;
        }

        private void Initialize(string fontDescrFilePath,string fontsDir,int fontsCount,bool areInternal)
        {
            //var symbolMappingParser = new InternalMathFontParser(fontDescrFilePath,fontsDir,fontsCount,areInternal);
           // FontSymbolMappings = symbolMappingParser.GetSymbolNames();
        }

        /// <summary>
        /// Gets the name of the alphanumeric character in this atom.
        /// </summary>
        public string AlphanumericSymbol { get; }

        /// <summary>
        /// Gets the name of the default alphanumeric character in this atom.
        /// </summary>
        public string DefaultAlphanumericSymbol { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            try
            {
                var charInfo = texFont.GetCharInfo(AlphanumericSymbol, style).Value;
                var resultBox = new CharBox(environment, charInfo);
                return resultBox;
            }
            catch (Exception)
            {
                try
                {
                    var charInfo = texFont.GetCharInfo(DefaultAlphanumericSymbol, style).Value;
                    var resultBox = new CharBox(environment, charInfo);
                    return resultBox;
                }
                catch (Exception)
                {
                    throw new TexParseException($"The symbol \" {AlphanumericSymbol}\" or \" {DefaultAlphanumericSymbol}\" cannot be found in the current math font package.");
                }
                
            }
            
        }

    }
}
