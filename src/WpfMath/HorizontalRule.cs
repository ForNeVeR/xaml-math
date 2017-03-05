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
        public HorizontalRule(TexEnvironment environment, double thickness, double width, double shift)
        {
            this.Width = width;
            this.Height = thickness;
            this.Shift = shift;
            this.Foreground = environment.Foreground;
            this.Background = environment.Background;	//Not strictly necessary
        }

        public override void RenderGeometry(GeometryGroup geometry, double scale, double x, double y)
        {
            base.RenderGeometry(geometry, scale, x, y);

            var rect = new Rect(x * scale, (y - this.Height) * scale, this.Width * scale, this.Height * scale);
            var rectangleGeometry = new RectangleGeometry(rect);
            geometry.Children.Add(rectangleGeometry);
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
