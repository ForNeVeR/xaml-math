using System.Windows;
using System.Windows.Media;
using WpfMath.Rendering;

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

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = Foreground ?? Brushes.Black;
            var rectangle = new Rect(x, y - Height, Width, Height);
            renderer.RenderRectangle(rectangle, color);
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
