namespace WpfMath
{
    // Atom representing single character in specific text style.
    internal class CharAtom : CharSymbol
    {
        public CharAtom(SourceSpan source, char character, string textStyle = null)
            : base(source)
        {
            this.TextStyle = textStyle;
            this.Character = character;
        }

        public char Character { get; }

        // Null means default text style.
        public string TextStyle { get; }

        private bool IsDefaultTextStyle => this.TextStyle == null;

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var font = GetStyledFont(environment);
            var charInfo = GetCharInfo(font, environment.Style);
            return new CharBox(environment, charInfo);
        }

        public override ITeXFont GetStyledFont(TexEnvironment environment) =>
            TextStyle == TexUtilities.TextStyleName ? environment.TextFont : base.GetStyledFont(environment);

        public override bool IsSupportedByFont(ITeXFont font) =>
            this.IsDefaultTextStyle
                ? font.SupportsDefaultCharacter(this.Character, TexStyle.Display)
                : font.SupportsCharacter(this.Character, this.TextStyle, TexStyle.Display);

        private CharInfo GetCharInfo(ITeXFont texFont, TexStyle style) =>
            this.IsDefaultTextStyle
                ? texFont.GetDefaultCharInfo(this.Character, style)
                : texFont.GetCharInfo(this.Character, this.TextStyle, style);

        public override CharFont GetCharFont(ITeXFont texFont)
        {
            // Style is irrelevant here.
            return GetCharInfo(texFont, TexStyle.Display).GetCharacterFont();
        }
    }
}
