using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Atom representing character that does not depend on text style.
internal class FixedCharAtom : CharSymbol
{
    public FixedCharAtom(CharFont charFont)
    {
        this.CharFont = charFont;
    }

    public CharFont CharFont
    {
        get;
        private set;
    }

    public override CharFont GetCharFont(ITeXFont texFont)
    {
        return this.CharFont;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var charInfo = environment.TexFont.GetCharInfo(this.CharFont, environment.Style);
        return new CharBox(environment, charInfo);
    }
}
