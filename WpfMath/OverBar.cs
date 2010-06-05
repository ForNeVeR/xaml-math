using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Box representing other box with horizontal rule above it.
    internal class OverBar : WpfMath.VerticalBox
    {
        public OverBar(Box box, double kern, double thickness)
            : base()
        {
            Add(new WpfMath.StrutBox(0, thickness, 0, 0));
            Add(new WpfMath.HorizontalRule(thickness, box.Width, 0));
            Add(new WpfMath.StrutBox(0, kern, 0, 0));
            Add(box);
        }
    }
}
