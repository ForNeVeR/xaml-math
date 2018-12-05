using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Extension character that contains character information for each of its parts.
    internal class ExtensionChar
    {
        public ExtensionChar(CharInfo top, CharInfo middle, CharInfo bottom, CharInfo repeat)
        {
            this.Top = top;
            this.Middle = middle;
            this.Repeat = repeat;
            this.Bottom = bottom;
        }

        public CharInfo Top
        {
            get;
            private set;
        }

        public CharInfo Middle
        {
            get;
            private set;
        }

        public CharInfo Bottom
        {
            get;
            private set;
        }

        public CharInfo Repeat
        {
            get;
            private set;
        }
    }
}
