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

        public static SymbolAtom GetAtom(string name)
        {
            try
            {
                return symbols[name];
            }
            catch (KeyNotFoundException)
            {
                throw new SymbolNotFoundException(name);
            }
        }

        public static bool TryGetAtom(string name, out SymbolAtom atom)
        {
            return symbols.TryGetValue(name, out atom);
        }

        public SymbolAtom(SymbolAtom symbolAtom, TexAtomType type)
            : base()
        {
            if (!validSymbolTypes[(int)type])
                throw new ArgumentException("The specified type is not a valid symbol type.", "type");
            this.Type = type;
            this.Name = symbolAtom.Name;
            this.IsDelimeter = symbolAtom.IsDelimeter;
        }

        public SymbolAtom(string name, TexAtomType type, bool isDelimeter)
            : base()
        {
            this.Type = type;
            this.Name = name;
            this.IsDelimeter = isDelimeter;
        }

        public bool IsDelimeter
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
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
