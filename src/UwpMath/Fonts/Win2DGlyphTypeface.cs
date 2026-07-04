using Microsoft.Graphics.Canvas.Text;

using XamlMath.Fonts;

namespace UwpMath.Fonts;

internal record Win2DGlyphTypeface(CanvasFontSet FontSet) : IFontTypeface;
