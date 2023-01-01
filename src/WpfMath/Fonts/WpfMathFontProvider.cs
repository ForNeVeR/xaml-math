using System;
using System.Windows.Media;

namespace WpfMath.Fonts;

/// <summary>A font provider implementation specifically for the WpfMath assembly.</summary>
internal class WpfMathFontProvider : IFontProvider
{
    private WpfMathFontProvider() {}

    public static WpfMathFontProvider Instance = new();

    private const string FontsDirectory = "Fonts/";

    public IFontTypeface ReadFontFile(string fontFileName)
    {
        var fontUri = new Uri($"pack://application:,,,/WpfMath;component/{FontsDirectory}{fontFileName}");
        return new WpfGlyphTypeface(new GlyphTypeface(fontUri));
    }
}
