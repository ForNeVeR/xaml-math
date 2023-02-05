using System.Linq;
using System.Windows.Media;
using WpfMath.Fonts;
using XamlMath;

namespace WpfMath.Rendering;

public static class WpfTeXEnvironment
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
        string systemTextFontName = "Arial",
        Brush? foreground = null,
        Brush? background = null)
    {
        var mathFont = new DefaultTexFont(WpfMathFontProvider.Instance, scale);
        var textFont = GetSystemFont(systemTextFontName, scale);

        return new TexEnvironment(
            style,
            mathFont,
            textFont,
            background.ToPlatform(),
            foreground.ToPlatform());
    }

    private static WpfSystemFont GetSystemFont(string fontName, double size)
    {
        var fontFamily = System.Windows.Media.Fonts.SystemFontFamilies.First(
            ff => ff.ToString() == fontName || ff.FamilyNames.Values?.Contains(fontName) == true);
        return new WpfSystemFont(size, fontFamily);
    }
}
