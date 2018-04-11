namespace WpfMath
{
    // Atom representing single character in specific text style.
    internal class CharAtom : CharSymbol
    {
        public CharAtom(SourceSpan source, string textStyle = null)
        {
            this.Source = source;
            this.TextStyle = textStyle;
            this.Character = Source[0];
        }

        public char Character { get; }

        // Null means default text style.
        public string TextStyle { get; }

        public override Atom Copy()
        {
            return CopyTo(new CharAtom(Source, TextStyle));
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
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
