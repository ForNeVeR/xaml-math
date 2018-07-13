using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath.Boxes
{
    /// <summary>
    /// Box representing other box with horizontal rule below it.
    /// </summary>
    internal class UnderBar:VerticalBox
    {
        public UnderBar(TexEnvironment environment, Box box, double kern, double thickness) : base()
        {
            Add(box);
            Add(new StrutBox(0, thickness, 0, 0));
            Add(new HorizontalRule(environment, thickness, box.Width, 0));
            Add(new StrutBox(0, kern, 0, 0));
        }
    }
}
