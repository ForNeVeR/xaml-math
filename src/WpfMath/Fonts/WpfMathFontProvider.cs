using System;
using System.Windows.Media;

namespace WpfMath.Fonts;

public class WpfMathFontProvider : IFontProvider
{
    private const string FontsDirectory = "Fonts/";

    public IFontTypeface ReadFontFile(string fontFileName)
    {
        var fontUri = new Uri($"pack://application:,,,/WpfMath;component/{FontsDirectory}{fontFileName}");
        return new WpfGlyphTypeface(new GlyphTypeface(fontUri));
    }
}
