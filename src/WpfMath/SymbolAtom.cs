using System;
using System.Collections;
using System.Collections.Generic;

namespace WpfMath
{
    // Atom representing symbol (non-alphanumeric character).
    internal class SymbolAtom : CharSymbol
    {
        /// <summary>
        /// Special name of empty delimiter symbol that shouldn't be rendered.
        /// </summary>
        internal const string EmptyDelimiterName = "_emptyDelimiter";

        // Dictionary of definitions of all symbols, keyed by name.
        private static IDictionary<string, SymbolAtom> symbols;

        // Set of all valid symbol types.
        private static BitArray validSymbolTypes;

        static SymbolAtom()
        {
            var symbolParser = new TexSymbolParser();
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
        }

        public static SymbolAtom GetAtom(string name, SourceSpan source)
        {
            try
            {
                var symbol = symbols[name];
                return new SymbolAtom(symbol, symbol.Type) { Source = source };
            }
            catch (KeyNotFoundException)
            {
                throw new SymbolNotFoundException(name);
            }
        }

        public static bool TryGetAtom(SourceSpan name, out SymbolAtom atom)
        {
            SymbolAtom temp;
            var nameString = name.ToString();
            if (symbols.TryGetValue(name.ToString(), out temp))
            {
                atom = new SymbolAtom(temp, temp.Type) { Source = name };
                return true;
            }
            atom = null;
            return false;
        }

        public SymbolAtom(SymbolAtom symbolAtom, TexAtomType type) : base(type)
        {
            if (!validSymbolTypes[(int)type])
                throw new ArgumentException("The specified type is not a valid symbol type.", nameof(type));
            this.Name = symbolAtom.Name;
            this.IsDelimeter = symbolAtom.IsDelimeter;
        }

        public SymbolAtom(string name, TexAtomType type, bool isDelimeter) : base(type)
        {
            this.Name = name;
            this.IsDelimeter = isDelimeter;
        }

        public bool IsDelimeter { get; }

        public string Name { get; }

        public override Atom Copy()
        {
            return CopyTo(new SymbolAtom(Name, Type, IsDelimeter));
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            return new CharBox(environment, environment.MathFont.GetCharInfo(this.Name, environment.Style));
        }

        public override CharFont GetCharFont(ITeXFont texFont)
        {
            // Style is irrelevant here.
            return texFont.GetCharInfo(Name, TexStyle.Display).GetCharacterFont();
        }
    }
}
