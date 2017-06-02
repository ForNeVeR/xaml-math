using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

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

        public char Character
        {
            get;
            private set;
        }

        // Null means default text style.
        public string TextStyle
        {
            get;
            private set;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            environment = environment.WithFont(OverrideFont(environment.TexFont));
            var charInfo = GetCharInfo(environment.TexFont, environment.Style);
            return new CharBox(environment, charInfo);
        }

        public override ITeXFont OverrideFont(ITeXFont defaultFont)
        {
            if (TextStyle != "text") // TODO[F]: Extract to constant
            {
                return defaultFont;
            }

            var fontFamily = Fonts.SystemFontFamilies.First(ff => ff.ToString() == "Arial"); // TODO[F]: Should be configurable
            var font = new SystemFont(defaultFont.Size, fontFamily);

            return font;
        }

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
