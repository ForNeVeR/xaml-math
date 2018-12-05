using WpfMath.Rendering;

namespace WpfMath.Boxes
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

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
