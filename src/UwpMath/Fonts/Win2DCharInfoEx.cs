using Microsoft.Graphics.Canvas.Text;

using System.Linq;
using System.Numerics;

using XamlMath;
using XamlMath.Exceptions;

namespace UwpMath.Fonts;

public static class Win2DCharInfoEx
{
    public static Win2DGlyphRun GetGlyphRun(this CharInfo info, double x, double y, double scale)
    {
        CanvasFontSet fontSet = ((Win2DGlyphTypeface)info.Font).FontSet;
        CanvasFontFace fontFace = fontSet.Fonts[0];
        if (!fontFace.HasCharacter(info.Character))
        {
            string fontName = fontFace.FamilyNames.First().Value;
            string characterHex = ((uint)info.Character).ToString("X4");
            throw new TexCharacterMappingNotFoundException($"The {fontName} font does not support '{info.Character}' (U+{characterHex}) character.");
        }
        return new Win2DGlyphRun
        {
            FontFace = fontFace,
            FontSize = (float)(info.Size * scale),
            GlyphIndices = fontFace.GetGlyphIndices([info.Character]),
            AdvanceWidths = [fontFace.GetGlyphMetrics([info.Character], isSideways: false)[0].AdvanceWidth],
            BaselineOrigin = new Vector2((float)(x * scale), (float)(y * scale))
        };
    }
}
