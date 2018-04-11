namespace WpfMath
{
    // Atom representing single character that can be marked as text symbol.
    internal abstract class CharSymbol : Atom
    {
        protected CharSymbol(TexAtomType type) : base(type)
        {
            this.IsTextSymbol = false;
        }

        protected CharSymbol() : this(TexAtomType.Ordinary)
        {
        }

        protected override Atom CopyTo(Atom atom)
        {
            ((CharSymbol)atom).IsTextSymbol = IsTextSymbol;
            return base.CopyTo(atom);
        }
        public bool IsTextSymbol { get; }

        /// <summary>Returns the preferred font to render this character.</summary>
        public virtual ITeXFont GetStyledFont(TexEnvironment environment) => environment.MathFont;

        public abstract CharFont GetCharFont(ITeXFont texFont);
    }
}
