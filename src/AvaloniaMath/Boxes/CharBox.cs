using Avalonia;
using Avalonia.Media;
using WpfMath.Colors;
using WpfMath.Rendering;

namespace WpfMath.Boxes
{
    // Box representing single character.
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

        internal GlyphRun GetGlyphRun(double scale, double x, double y)
        {

            var fontStyle = FontStyle.Normal;
            var fontWeight = FontWeight.Normal;

            if (Character.Font.Style.HasFlag(FontStyle.Italic))
            {
                fontStyle |= FontStyle.Italic;
            }

            if (Character.Font.Weight.HasFlag(FontWeight.Bold))
            {
                fontWeight |= FontWeight.Bold;
            }

            // TODO: Implement font decoration after Avalonia adds support.
            /*
            Underline;
            Strikethrough;
            */
            var typeface = new Typeface(
                    Character.Font.FontFamily,
                    scale * Character.Size,
                    fontStyle,
                    fontWeight);

            // var glyphIndex = typeface.CharacterToGlyphMap[this.Character.Character];
            //  var glyphRun = new GlyphRun(typeface, 0, false, this.Character.Size * scale,
            //      new ushort[] { glyphIndex }, new Point(x * scale, y * scale),
            //      new double[] { typeface.AdvanceWidths[glyphIndex] }, null, null, null, null, null, null);



            var glyphRun = new GlyphRun(typeface,this.Character.Size * scale,
           //      new Point(x * scale, 0),Character.Character);
            new Point(x * scale, y  * scale),Character.Character);


            return glyphRun;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = this.Foreground ?? ArgbColor.Black;
            renderer.RenderGlyphRun(scale => this.GetGlyphRun(scale, x, y), x, y, color);
        }

        public override int GetLastFontId()
        {
            return this.Character.FontId;
        }
    }
}
