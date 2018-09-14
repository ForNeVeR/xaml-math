using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using WpfMath.Exceptions;
using WpfMath.Parsers;
using WpfMath.Utils;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Atom representing symbol (non-alphanumeric character).
    /// </summary>
    internal class SymbolAtom : CharSymbol
    {
        /// <summary>
        /// Special name of empty delimiter symbol that shouldn't be rendered.
        /// </summary>
        internal const string EmptyDelimiterName = "_emptyDelimiter";

        // Dictionary of definitions of all symbols, keyed by name.
        private static IDictionary<string, Func<SourceSpan, SymbolAtom>> symbols;

        // Set of all valid symbol types.
        private static BitArray validSymbolTypes;

        private static string _symbolsFilePath = "WpfMath.Data.AsanaMathData.AsanaMath_TexSymbols.xml";
        private static bool _isInternal = true;

        public static string SymbolsFilePath { get { return _symbolsFilePath; } set { _symbolsFilePath = value; } }
        public static bool IsInternal { get { return _isInternal; } set { _isInternal = value; } }

        private static void Initialize(string symbols_filepath,bool isinternal)
        {
            SymbolsFilePath = symbols_filepath;
            IsInternal = isinternal;
            
            //var symbolParser = new InternalMathSymbolParser();
            var symbolParser = new InternalMathSymbolParser(SymbolsFilePath, IsInternal);
            symbols = symbolParser.GetSymbols();

            validSymbolTypes = new BitArray(16);
            validSymbolTypes.Set((int)TexAtomType.Ordinary, true);
            validSymbolTypes.Set((int)TexAtomType.BigOperator, true);
            validSymbolTypes.Set((int)TexAtomType.BinaryOperator, true);
            validSymbolTypes.Set((int)TexAtomType.Relation, true);
            validSymbolTypes.Set((int)TexAtomType.Opening, true);
            validSymbolTypes.Set((int)TexAtomType.Closing, true);
            validSymbolTypes.Set((int)TexAtomType.Punctuation, true);
            validSymbolTypes.Set((int)TexAtomType.Accent, true);
            validSymbolTypes.Set((int)TexAtomType.Over, true);
            validSymbolTypes.Set((int)TexAtomType.Under, true);
        }

        static SymbolAtom()
        {
            Initialize("WpfMath.Data.TexSymbols.xml", true);
        }

        public static SymbolAtom GetAtom(string name, SourceSpan source, string symbolsfilepath = "WpfMath.Data.TexSymbols.xml", bool areInternal = true)
        {
            try
            {
                Initialize(symbolsfilepath, areInternal);
                if (symbols.ContainsKey(name))
                {
                    var symbol = symbols[name](source);

                    return new SymbolAtom(source, symbol, symbol.Type);
                }
                else
                {
                    throw new TexParseException("The symbol " +'"' +name + '"' + " does not exist in the current math font package.");
                }
                
            }
            catch (KeyNotFoundException)
            {
                throw new SymbolNotFoundException(name);
            }
        }

        public static bool TryGetAtom(SourceSpan name, out SymbolAtom atom,string symbolsfilepath= "WpfMath.Data.TexSymbols.xml",bool areInternal=true)
        {
            Initialize(symbolsfilepath, areInternal);
            if (symbols.TryGetValue(name.ToString(), out var temp))
            {
                var symbol = temp(name);
                atom = new SymbolAtom(name, symbol, symbol.Type);
                return true;
            }
            else
            {
                atom = null;
                return false;
            }
        }

        public SymbolAtom(SourceSpan source, SymbolAtom symbolAtom, TexAtomType type, string symbolsfilepath = "WpfMath.Data.TexSymbols.xml", bool areInternal = true) : base(source, type)
        {
            Initialize(symbolsfilepath, areInternal);
            if (!validSymbolTypes[(int)type])
                throw new ArgumentException("The specified type is not a valid symbol type.", nameof(type));
            this.Name = symbolAtom.Name;
            this.IsDelimeter = symbolAtom.IsDelimeter;
        }

        public SymbolAtom(SourceSpan source, string name, TexAtomType type, bool isDelimeter, string symbolsfilepath = "WpfMath.Data.TexSymbols.xml", bool areInternal = true) : base(source, type)
        {
            Initialize(symbolsfilepath, areInternal);
            this.Name = name;
            this.IsDelimeter = isDelimeter;
        }

        public bool IsDelimeter { get; }

        public string Name { get; }

        protected override Result<CharInfo> GetCharInfo(ITeXFont font, TexStyle style) =>
            font.GetCharInfo(this.Name, style);

        public override Result<CharFont> GetCharFont(ITeXFont texFont) =>
            // Style is irrelevant here.
            texFont.GetCharInfo(this.Name, TexStyle.Display).Map(ci => ci.GetCharacterFont());
    }
}
            
