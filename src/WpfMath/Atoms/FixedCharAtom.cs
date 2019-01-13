using WpfMath.Utils;

namespace WpfMath.Atoms
{
    // Atom representing character that does not depend on text style.
    internal class FixedCharAtom : CharSymbol
    {
        public FixedCharAtom(SourceSpan source, CharFont charFont)
            : base(source)
        {
            this.CharFont = charFont;
        }

        public CharFont CharFont { get; }

        protected override Result<CharInfo> GetCharInfo(ITeXFont font, TexStyle style) =>
            font.GetCharInfo(this.CharFont, style);

        public override Result<CharFont> GetCharFont(ITeXFont texFont) =>
            Result.Ok(this.CharFont);
    }
}
