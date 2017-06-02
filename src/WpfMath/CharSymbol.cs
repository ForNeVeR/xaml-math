namespace WpfMath
{
    // Atom representing single character that can be marked as text symbol.
    internal abstract class CharSymbol : Atom
    {
        public CharSymbol()
        {
            this.IsTextSymbol = false;
        }

        public bool IsTextSymbol
        {
            get;
            set;
        }

        /// <summary>Returns the preferred font to render this character.</summary>
        public virtual ITeXFont OverrideFont(ITeXFont defaultFont) => defaultFont;
        public abstract CharFont GetCharFont(ITeXFont texFont);
    }
}
