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
    /// <summary>
    /// Box representing a circle.
    /// </summary>
    internal class OvalRule:Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HorizontalRule"/>.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="shift"></param>
        public OvalRule(TexEnvironment environment, double height, double width, double shift)
        {
            this.Width = width;
            this.Height = height;
            this.Shift = shift;
            this.Foreground = environment.Foreground;
            this.Background = environment.Background;	//Not strictly necessary
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = Foreground ?? Brushes.Black;
            var rectangle = new Rect(x, y - Height, Width, Height);
            renderer.RenderEllipse(rectangle, color,Brushes.Transparent);

        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
