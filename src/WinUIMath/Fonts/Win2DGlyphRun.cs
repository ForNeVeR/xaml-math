using Microsoft.Graphics.Canvas.Text;

using System.Numerics;

namespace WinUIMath.Fonts;

public class Win2DGlyphRun
{
    public required CanvasFontFace FontFace { get; set; }
    public required int[] GlyphIndices { get; set; }
    public required float FontSize { get; set; }
    public Vector2 BaselineOrigin { get; set; }
    public float[] AdvanceWidths { get; set; } = [];
}
