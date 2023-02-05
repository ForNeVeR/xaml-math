using Avalonia.Media;
using XamlMath.Fonts;

namespace AvaloniaMath.Fonts;

public class AvaloniaMathFontProvider : IFontProvider
{
    private AvaloniaMathFontProvider() {}

    public static AvaloniaMathFontProvider Instance = new();

    public IFontTypeface ReadFontFile(string fontFileName)
    {
        var fontName = fontFileName.Substring(0, fontFileName.LastIndexOf('.'));
        var ff = FontFamily.Parse($"avares://AvaloniaMath/Fonts#{fontName}");
        var typeface = new Typeface(ff);
        return new AvaloniaGlyphTypeface(typeface);
    }
}
