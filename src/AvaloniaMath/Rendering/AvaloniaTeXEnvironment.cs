using Avalonia.Media;
using AvaloniaMath.Fonts;
using XamlMath;

namespace AvaloniaMath.Rendering;

public static class AvaloniaTeXEnvironment
{
    /// <summary>Creates an instance of <see cref="TexEnvironment"/> for an Avalonia program.</summary>
    /// <param name="style">Initial style for the formula content.</param>
    /// <param name="scale">Formula font size.</param>
    /// <param name="systemTextFontName">Name of the system font to use for the <code>\text</code> blocks.</param>
    /// <param name="foreground">Foreground color. Black if not specified.</param>
    /// <param name="background">Background color.</param>
    public static TexEnvironment Create(
        TexStyle style = TexStyle.Display,
        double scale = 20.0,
        string systemTextFontName = "Arial",
        Brush? foreground = null,
        Brush? background = null)
    {
        var mathFont = new DefaultTexFont(AvaloniaMathFontProvider.Instance, scale);
        var textFont = GetSystemFont(scale, systemTextFontName);

        return new TexEnvironment(
            style,
            mathFont,
            textFont,
            background.ToPlatform(),
            foreground.ToPlatform());
    }

    private static AvaloniaSystemFont GetSystemFont(double size, string fontName) => new(size, fontName);
}
