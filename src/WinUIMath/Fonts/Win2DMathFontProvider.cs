using Microsoft.Graphics.Canvas.Text;

using System;
using System.IO;

using XamlMath.Fonts;

namespace WinUIMath.Fonts;

internal class Win2DMathFontProvider : IFontProvider
{
    public IFontTypeface ReadFontFile(string fontFileName)
    {
        //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", fontFileName);
        string path = Path.Combine(@"D:\_temp\XAML-Math-Fonts\", fontFileName);
        CanvasFontSet fontSet = new(new Uri(path));  // UNABLE_TO_MASK_PATH
        return new Win2DGlyphTypeface(fontSet);
    }
}
