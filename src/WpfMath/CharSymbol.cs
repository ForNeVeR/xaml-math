namespace WpfMath
{
    // Atom representing single character that can be marked as text symbol.
    internal abstract class CharSymbol : Atom
    {
        protected CharSymbol(SourceSpan source, TexAtomType type = TexAtomType.Ordinary)
            : base(source, type)
        {
            this.IsTextSymbol = false;
        }

        public bool IsTextSymbol { get; }

        /// <summary>Returns the preferred font to render this character.</summary>
        public virtual ITeXFont GetStyledFont(TexEnvironment environment) => environment.MathFont;

        /// <summary>Checks if the symbol can be rendered by font.</summary>
        public abstract bool IsSupportedByFont(ITeXFont font);

        /// <summary>
        /// Returns the symbol rendered by font. Throws an exception if the symbol is not supported by font. Always
        /// succeed if <see cref="IsSupportedByFont"/>.
        /// </summary>
        public abstract CharFont GetCharFont(ITeXFont texFont);
    }
}
