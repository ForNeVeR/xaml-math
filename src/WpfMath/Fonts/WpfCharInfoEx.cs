using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using XamlMath;
using XamlMath.Exceptions;

namespace WpfMath.Fonts;

public static class WpfCharInfoEx
{
    public static GlyphRun GetGlyphRun(this CharInfo info, double x, double y, double scale)
    {
        var typeface = ((WpfGlyphTypeface)info.Font).Typeface;
        var characterInt = (int)info.Character;
        if (!typeface.CharacterToGlyphMap.TryGetValue(characterInt, out var glyphIndex))
        {
            var fontName = typeface.FamilyNames.Values.First();
            var characterHex = characterInt.ToString("X4");
            throw new TexCharacterMappingNotFoundException(
                $"The {fontName} font does not support '{info.Character}' (U+{characterHex}) character.");
        }

        var glyphRun = new GlyphRun((float)scale);
        ((ISupportInitialize)glyphRun).BeginInit();
        glyphRun.GlyphTypeface = typeface;
        glyphRun.FontRenderingEmSize = info.Size * scale;
        glyphRun.GlyphIndices = new[] { glyphIndex };
        glyphRun.BaselineOrigin = new Point(x * scale, y * scale);
        glyphRun.AdvanceWidths = new[] { typeface.AdvanceWidths[glyphIndex] };
        ((ISupportInitialize)glyphRun).EndInit();

        return glyphRun;
    }
}
