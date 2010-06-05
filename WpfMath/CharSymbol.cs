using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public abstract WpfMath.CharFont GetCharFont(WpfMath.ITeXFont texFont);
    }
}
