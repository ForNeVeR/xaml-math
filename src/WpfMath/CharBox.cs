using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

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

        private GlyphRun GetGlyphRun(double scale, double x, double y)
        {
            var typeface = this.Character.Font;
            var glyphIndex = typeface.CharacterToGlyphMap[this.Character.Character];
            var glyphRun = new GlyphRun(typeface, 0, false, this.Character.Size * scale,
                new ushort[] { glyphIndex }, new Point(x * scale, y * scale),
                new double[] { typeface.AdvanceWidths[glyphIndex] }, null, null, null, null, null, null);
            return glyphRun;

        }

        public override void RenderGeometry(GeometryGroup geometry, double scale, double x, double y)
        {
            base.RenderGeometry(geometry, scale, x, y);

            GlyphRun glyphRun = GetGlyphRun(scale, x, y);

            GeometryGroup geoGroup = glyphRun.BuildGeometry() as GeometryGroup;
            geometry.Children.Add(geoGroup);
        }

        public override int GetLastFontId()
        {
            return this.Character.FontId;
        }
    }
}
