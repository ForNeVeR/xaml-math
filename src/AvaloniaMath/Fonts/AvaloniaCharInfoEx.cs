using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using XamlMath;
using XamlMath.Exceptions;

namespace AvaloniaMath.Fonts;

internal static class AvaloniaCharInfoEx
{
    public static GlyphRun GetGlyphRun(this CharInfo info, double x, double y, double scale)
    {
        var typeface = info.Font.ToAvalonia();
        var characterUInt = (uint)info.Character;
        var glyphTypeface = typeface.GlyphTypeface;

        if (!glyphTypeface.TryGetGlyph(characterUInt, out var glyphIndex))
        {
            var fontName = typeface.FontFamily.FamilyNames.First();
            var characterHex = characterUInt.ToString("X4");
            throw new TexCharacterMappingNotFoundException(
                $"The {fontName} font does not support '{info.Character}' (U+{characterHex}) character.");
        }

        var glyphRun = new GlyphRun(glyphTypeface, info.Size * scale, new ReadOnlyMemory<char>(new[] {info.Character}), new[] {glyphIndex})
        {
            BaselineOrigin = new Point(x * scale, y * scale)
        };

        return glyphRun;
    }
}
