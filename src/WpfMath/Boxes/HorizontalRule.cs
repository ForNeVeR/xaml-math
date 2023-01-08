using System.Windows;
using System.Windows.Media;
using WpfMath.Rendering;

namespace WpfMath.Boxes
{
    /// <summary>Box representing horizontal line.</summary>
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
            var color = this.Foreground.ToWpf() ?? Brushes.Black;
            var rectangle = new Rectangle(x, y - this.Height, this.Width, this.Height);
            renderer.RenderRectangle(rectangle, color.ToPlatform());
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
