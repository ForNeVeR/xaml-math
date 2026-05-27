using Microsoft.Graphics.Canvas.Text;

using System.Numerics;

namespace WinUIMath.Fonts;

public class Win2DGlyphRun
{
    public CanvasFontFace FontFace { get; set; }
    public int[] GlyphIndices { get; set; }
    public float FontSize { get; set; }
    public Vector2 BaselineOrigin { get; set; }
    public float[] AdvanceWidths { get; set; }
}
