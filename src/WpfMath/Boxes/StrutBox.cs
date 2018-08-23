using WpfMath.Rendering;
using WpfMath.Utils;

namespace WpfMath.Boxes
{
    /// <summary>
    /// Box representing whitespace.
    /// </summary>
    internal class StrutBox : Box
    {
        private static readonly StrutBox emptyStrutBox = new StrutBox(0, 0, 0, 0);

        /// <summary>
        /// Gets a box that has no content.
        /// </summary>
        public static StrutBox Empty=> new StrutBox(0.5,0.5,0,0){ShowBounds=true;};

        public StrutBox(double width, double height, double depth, double shift)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
            this.Shift = shift;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
