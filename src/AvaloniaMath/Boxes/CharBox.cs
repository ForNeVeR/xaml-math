using System.Linq;
using Avalonia;
using Avalonia.Media;
using AvaloniaMath.Rendering;
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
            var characterUInt = (uint)this.Character.Character;
            var glyphTypeface = typeface.GlyphTypeface;

            if (!glyphTypeface.TryGetGlyph(characterUInt, out var glyphIndex))
            {
                var fontName = typeface.FontFamily.FamilyNames.First();
                var characterHex = characterUInt.ToString("X4");
                throw new TexCharacterMappingNotFoundException(
                    $"The {fontName} font does not support '{this.Character.Character}' (U+{characterHex}) character.");
            }

            var glyphRun = new GlyphRun(glyphTypeface, this.Character.Size * scale, new[] {glyphIndex})
            {
                BaselineOrigin = new Point(x * scale, y * scale)
            };

            return glyphRun;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = ((AvaloniaBrush?) Foreground)?.Get() ?? Brushes.Black;
            renderer.RenderGlyphRun(scale => this.GetGlyphRun(scale, x, y), x, y, color);
        }

        public override int GetLastFontId()
        {
            return this.Character.FontId;
        }
    }
}
