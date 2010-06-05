using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Extension character that contains character information for each of its parts.
    internal class ExtensionChar
    {
        public ExtensionChar(WpfMath.CharInfo top, WpfMath.CharInfo middle, WpfMath.CharInfo bottom, WpfMath.CharInfo repeat)
        {
            this.Top = top;
            this.Middle = middle;
            this.Repeat = repeat;
            this.Bottom = bottom;
        }

        public WpfMath.CharInfo Top
        {
            get;
            private set;
        }

        public WpfMath.CharInfo Middle
        {
            get;
            private set;
        }

        public WpfMath.CharInfo Bottom
        {
            get;
            private set;
        }

        public WpfMath.CharInfo Repeat
        {
            get;
            private set;
        }
    }
}
