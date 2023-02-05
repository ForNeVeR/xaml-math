using System.Linq;
using System.Windows;
using System.Windows.Media;
using XamlMath;
using XamlMath.Exceptions;
#if !NET452
using System.ComponentModel;
#endif

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
#if NET452
        var glyphRun = new GlyphRun(typeface, 0, false, info.Size * scale,
            new ushort[] { glyphIndex }, new Point(x * scale, y * scale),
            new double[] { typeface.AdvanceWidths[glyphIndex] }, null, null, null, null, null, null);
#else
        var glyphRun = new GlyphRun((float)scale);
        ((ISupportInitialize)glyphRun).BeginInit();
        glyphRun.GlyphTypeface = typeface;
        glyphRun.FontRenderingEmSize = info.Size * scale;
        glyphRun.GlyphIndices = new[] { glyphIndex };
        glyphRun.BaselineOrigin = new Point(x * scale, y * scale);
        glyphRun.AdvanceWidths = new[] { typeface.AdvanceWidths[glyphIndex] };
        ((ISupportInitialize)glyphRun).EndInit();
#endif
        return glyphRun;
    }
}
