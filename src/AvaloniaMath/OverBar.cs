using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Box representing other box with horizontal rule above it.
    internal class OverBar : VerticalBox
    {
        public OverBar(TexEnvironment environment, Box box, double kern, double thickness)
            : base()
        {
            Add(new StrutBox(0, thickness, 0, 0));
            Add(new HorizontalRule(environment, thickness, box.Width, 0));
            Add(new StrutBox(0, kern, 0, 0));
            Add(box);
        }
    }
}
