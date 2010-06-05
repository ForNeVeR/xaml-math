using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Box representing other box with horizontal rule above it.
internal class OverBar : VerticalBox
{
    public OverBar(Box box, double kern, double thickness)
        : base()
    {
        Add(new StrutBox(0, thickness, 0, 0));
        Add(new HorizontalRule(thickness, box.Width, 0));
        Add(new StrutBox(0, kern, 0, 0));
        Add(box);
    }
}
