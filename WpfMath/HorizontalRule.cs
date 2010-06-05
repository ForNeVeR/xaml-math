using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
{
    // Box representing horizontal line.
    internal class HorizontalRule : Box
    {
        public HorizontalRule(double thickness, double width, double shift)
        {
            this.Width = width;
            this.Height = thickness;
            this.Shift = shift;
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(
                x * scale, (y - this.Height) * scale, this.Width * scale, this.Height * scale));
        }

        public override int GetLastFontId()
        {
            return WpfMath.TexFontUtilities.NoFontId;
        }
    }
}
