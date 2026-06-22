using Microsoft.Graphics.Canvas.Text;

using System;

using XamlMath.Fonts;

namespace UwpMath.Fonts;

internal class Win2DMathFontProvider : IFontProvider
{
    public IFontTypeface ReadFontFile(string fontFileName)
    {
        string path = $"ms-appx:///{nameof(UwpMath)}/Fonts/{fontFileName}";
        CanvasFontSet fontSet = new(new Uri(path));
        return new Win2DGlyphTypeface(fontSet);
    }
}
