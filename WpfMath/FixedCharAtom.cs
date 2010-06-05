using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing character that does not depend on text style.
    internal class FixedCharAtom : CharSymbol
    {
        public FixedCharAtom(WpfMath.CharFont charFont)
        {
            this.CharFont = charFont;
        }

        public WpfMath.CharFont CharFont
        {
            get;
            private set;
        }

        public override WpfMath.CharFont GetCharFont(WpfMath.ITeXFont texFont)
        {
            return this.CharFont;
        }

        public override Box CreateBox(WpfMath.TexEnvironment environment)
        {
            var charInfo = environment.TexFont.GetCharInfo(this.CharFont, environment.Style);
            return new CharBox(environment, charInfo);
        }
    }
}
