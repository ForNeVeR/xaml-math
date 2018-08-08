using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfMath.Rendering;
using WpfMath.Utils;

namespace WpfMath.Boxes
{
    internal class RoundedRectangleBox:Box
    {
        private double x_radius = 0;
        private double y_radius = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedRectangleBox"/>.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="shift"></param>
        public RoundedRectangleBox(TexEnvironment environment, double height, double width, double shift,double xrad=3,double yrad=3)
        {
            this.Width = width;
            this.Height = height;
            this.Shift = shift;
            x_radius = xrad;
            y_radius = yrad;
            this.Foreground = environment.Foreground;
            this.Background = environment.Background;	//Not strictly necessary
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = Foreground ?? Brushes.Black;
            var rectangle = new Rect(x, y -Height, Width, Height);
            renderer.RenderRoundedRectangle(rectangle,color, Brushes.Transparent,x_radius,y_radius);

        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
