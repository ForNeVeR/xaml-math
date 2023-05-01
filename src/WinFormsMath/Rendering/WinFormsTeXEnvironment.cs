using WinFormsMath.Fonts;
using XamlMath;

namespace WinFormsMath.Rendering;

public static class WinFormsTeXEnvironment
{
    /// <summary>Creates an instance of <see cref="TexEnvironment"/> for a Windows Forms program.</summary>
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
        var mathFont = new DefaultTexFont(WinFormsMathFontProvider.Instance, scale);
        var textFont = GetSystemFont(systemTextFontName, scale);

        return new TexEnvironment(
            style,
            mathFont,
            textFont,
            background.ToPlatform(),
            foreground.ToPlatform());
    }

    private static WinFormsSystemFont GetSystemFont(string fontName, double size)
    {
        var fontFamily = FontFamily.Families.First(ff => ff.Name == fontName);
        return new WinFormsSystemFont(size, fontFamily);
    }
}
