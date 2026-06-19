using Microsoft.Graphics.Canvas.Text;

using XamlMath.Fonts;

namespace WinUIMath.Fonts;

internal record Win2DGlyphTypeface(CanvasFontSet FontSet) : IFontTypeface;
