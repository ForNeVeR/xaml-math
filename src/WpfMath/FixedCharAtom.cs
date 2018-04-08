using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing character that does not depend on text style.
    internal class FixedCharAtom : CharSymbol
    {
        public FixedCharAtom(CharFont charFont)
        {
            this.CharFont = charFont;
        }

        public CharFont CharFont { get; }

        public override CharFont GetCharFont(ITeXFont texFont)
        {
            return this.CharFont;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            var charInfo = environment.MathFont.GetCharInfo(this.CharFont, environment.Style);
            return new CharBox(environment, charInfo);
        }
    }
}
