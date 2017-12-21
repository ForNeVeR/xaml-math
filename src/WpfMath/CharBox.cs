using System.Windows;
using Avalonia;
using Avalonia.Media;
using WpfMath.Avalonia;
using AM = Avalonia.Media; 

namespace WpfMath
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

        private void GetGlyphRun(DrawingContext gfx, IBrush brush, double scale, double x, double y)
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

            if (Character.Font.FontSize >= 0.0)
            {
                var tf = new Typeface(
                    Character.Font.FontFamilyName,
                    Character.Font.FontSize * scale,
                    fontStyle,
                    fontWeight);

                var ft = new FormattedText
                {
                    Typeface = tf,
                    Text = Character.Character.ToString(),
                    TextAlignment = TextAlignment.Left,
                    Wrapping = TextWrapping.NoWrap
                };

                var origin = new Point(x, y);

                gfx.DrawText(brush, origin, ft);
            }
        }

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
            var brush = (IBrush) this.Foreground ?? Brushes.Black;
            //GlyphRun glyphRun = 
                GetGlyphRun(drawingContext, brush, scale, x, y);

            // Draw character at given position.
            //drawingContext.DrawGlyphRun(, glyphRun);
        }

        public override void RenderGeometry(GeometryGroup geometry, double scale, double x, double y)
        {
            /* movi - restore this later
            GlyphRun glyphRun = GetGlyphRun(scale, x, y);

            GeometryGroup geoGroup = glyphRun.BuildGeometry() as GeometryGroup;
            geometry.Children.Add(geoGroup);
            */
        }

        public override int GetLastFontId()
        {
            return this.Character.FontId;
        }
    }
}
