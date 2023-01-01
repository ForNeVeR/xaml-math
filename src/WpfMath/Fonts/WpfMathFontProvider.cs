using System;
using System.Windows.Media;

namespace WpfMath.Fonts;

/// <summary>A font provider implementation specifically for the WpfMath assembly.</summary>
public class WpfMathFontProvider : IFontProvider
{
    private const string FontsDirectory = "Fonts/";

    public IFontTypeface ReadFontFile(string fontFileName)
    {
        var fontUri = new Uri($"pack://application:,,,/WpfMath;component/{FontsDirectory}{fontFileName}");
        return new WpfGlyphTypeface(new GlyphTypeface(fontUri));
    }
}
