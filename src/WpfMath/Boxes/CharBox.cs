using System.Windows.Media;
using WpfMath.Rendering;

namespace WpfMath.Boxes
{
    /// <summary>Box representing single character.</summary>
    internal class CharBox : Box
    {
        public CharBox(TexEnvironment environment, CharInfo charInfo)
            : base(environment)
        {
            this.Character = charInfo;
            this.Width = charInfo.Metrics.Width;
            this.Height = charInfo.Metrics.Height;
            this.Depth = charInfo.Metrics.Depth;
            this.Italic = charInfo.Metrics.Italic;
        }

        public CharInfo Character
        {
            get;
            private set;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = this.Foreground.ToWpf() ?? Brushes.Black;
            renderer.RenderCharacter(Character, x, y, color.ToPlatform());
        }

        public override int GetLastFontId()
        {
            return this.Character.FontId;
        }
    }
}
