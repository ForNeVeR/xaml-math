using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
{
    // Box representing whitespace.
    internal class StrutBox : Box
    {
        private static readonly StrutBox emptyStrutBox = new StrutBox(0, 0, 0, 0);

        public static StrutBox Empty
        {
            get { return emptyStrutBox; }
        }

        public StrutBox(double width, double height, double depth, double shift)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.Shift = shift;
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
            x = TexRenderer.RoundToWholePixels(x, scale);
            y = TexRenderer.RoundToWholePixels(y, scale);
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
