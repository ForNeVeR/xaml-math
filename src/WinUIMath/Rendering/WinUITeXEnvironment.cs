using Microsoft.Graphics.Canvas;
using Microsoft.UI.Xaml.Media;

using WinUIMath.Fonts;

using XamlMath;

namespace WinUIMath.Rendering;

internal class WinUITeXEnvironment
{
    /// <summary>Creates an instance of <see cref="TexEnvironment"/> for a WPF program.</summary>
    /// <param name="style">Initial style for the formula content.</param>
    /// <param name="scale">Formula font size.</param>
    /// <param name="systemTextFontName">Name of the system font to use for the <code>\text</code> blocks.</param>
    /// <param name="foreground">Foreground color. Black if not specified.</param>
    /// <param name="background">Background color.</param>
    public static TexEnvironment Create(
        TexStyle style = TexStyle.Display,
        double scale = 20.0,
        string? systemTextFontName = null,
        Brush? foreground = null,
        Brush? background = null)
    {
        var mathFont = new DefaultTexFont(new Win2DMathFontProvider(), scale);
        var textFont = GetSystemFont(scale);

        return new TexEnvironment(
            style,
            mathFont,
            textFont,
            background?.ToPlatform(),
            foreground?.ToPlatform());
    }

    //private static WinUISystemFont GetSystemFont(string fontName, double size)
    //{
    //    FontFamily fontFamily = string.IsNullOrEmpty(fontName) ? FontFamily.XamlAutoFontFamily : new FontFamily(fontName);
    //    return new WinUISystemFont(size, fontFamily);
    //}

    private static WinUISystemFont GetSystemFont(double size)
    {
        return new WinUISystemFont(size, FontFamily.XamlAutoFontFamily);
    }
}
