using WpfMath.Rendering;

namespace WpfMath.Boxes
{
    // Box representing glue.
    internal class GlueBox : Box
    {
        public GlueBox(double space, double stretch, double shrink)
        {
            this.Width = space;
            this.Stretch = stretch;
            this.Shrink = shrink;
        }

        public double Stretch
        {
            get;
            private set;
        }

        public double Shrink
        {
            get;
            private set;
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
