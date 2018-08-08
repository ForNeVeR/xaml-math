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
    /// Box representing vertical line.
    /// </summary>
    internal class VerticalRule:Box
    {
        /// <summary>
        /// Creates a new instance of a <see cref="Box"/> representing a vertical line.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="thickness"></param>
        /// <param name="height"></param>
        /// <param name="shift"></param>
        public VerticalRule(TexEnvironment environment, double thickness, double height, double shift)
        {
            this.Height = height;
            this.Width = thickness;
            this.Shift = shift;
            this.Foreground = environment.Foreground;
            this.Background = environment.Background;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = Foreground ?? Brushes.Red;
            var rectangle = new Rect(x-Width , y, Width, Height);
            renderer.RenderRectangle(rectangle, color,color);
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
