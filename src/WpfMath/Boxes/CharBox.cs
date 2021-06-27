using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfMath.Exceptions;
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
            var typeface = this.Character.Font;
            var characterInt = (int)this.Character.Character;
            if (!typeface.CharacterToGlyphMap.TryGetValue(characterInt, out var glyphIndex))
            {
                var fontName = typeface.FamilyNames.Values.First();
                var characterHex = characterInt.ToString("X4");
                throw new TexCharacterMappingNotFoundException(
                    $"The {fontName} font does not support '{this.Character.Character}' (U+{characterHex}) character.");
            }
#if NET452
            var glyphRun = new GlyphRun(typeface, 0, false, this.Character.Size * scale,
                new ushort[] { glyphIndex }, new Point(x * scale, y*scale),
                new double[] { typeface.AdvanceWidths[glyphIndex] }, null, null, null, null, null, null);
#else
            var glyphRun = new GlyphRun((float)scale);
            ((ISupportInitialize)glyphRun).BeginInit();
            glyphRun.GlyphTypeface = typeface;
            glyphRun.FontRenderingEmSize = this.Character.Size * scale;
            glyphRun.GlyphIndices = new[] { glyphIndex };
            glyphRun.BaselineOrigin = new Point(x * scale, y * scale);
            glyphRun.AdvanceWidths = new[] { typeface.AdvanceWidths[glyphIndex] };
            ((ISupportInitialize)glyphRun).EndInit();
#endif
            return glyphRun;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = ((WpfBrush)Foreground)?.Get() ?? Brushes.Black;
            renderer.RenderGlyphRun(scale => this.GetGlyphRun(scale, x, y), x, y, color);
        }

        public override int GetLastFontId()
        {
            return this.Character.FontId;
        }
    }
}
