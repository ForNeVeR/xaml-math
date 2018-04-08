namespace WpfMath
{
    // Atom representing single character in specific text style.
    internal class CharAtom : CharSymbol
    {
        public CharAtom(char character, string textStyle = null)
        {
            this.Character = character;
            this.TextStyle = textStyle;
        }

        public char Character { get; }

        // Null means default text style.
        public string TextStyle { get; }

        public override Box CreateBox(TexEnvironment environment)
        {
            var font = GetStyledFont(environment);
            var charInfo = GetCharInfo(font, environment.Style);
            return new CharBox(environment, charInfo);
        }

        public override ITeXFont GetStyledFont(TexEnvironment environment) =>
            TextStyle == TexUtilities.TextStyleName ? environment.TextFont : base.GetStyledFont(environment);

        private CharInfo GetCharInfo(ITeXFont texFont, TexStyle style)
        {
            if (this.TextStyle == null)
                return texFont.GetDefaultCharInfo(this.Character, style);
            else
                return texFont.GetCharInfo(this.Character, this.TextStyle, style);
        }

        public override CharFont GetCharFont(ITeXFont texFont)
        {
            // Style is irrelevant here.
            return GetCharInfo(texFont, TexStyle.Display).GetCharacterFont();
        }
    }
}
