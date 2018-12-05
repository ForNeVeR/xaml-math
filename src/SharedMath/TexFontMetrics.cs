using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Specifies font metrics for single character.
    internal class TexFontMetrics
    {
        public TexFontMetrics(double width, double height, double depth, double italicWidth, double scale)
        {
            this.Width = width * scale;
            this.Height = height * scale;
            this.Depth = depth * scale;
            this.Italic = italicWidth * scale;
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }

        public double Depth
        {
            get;
            set;
        }

        public double Italic
        {
            get;
            set;
        }
    }
}
