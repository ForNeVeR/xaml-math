using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using XamlMath.Fonts;

namespace WpfMath.Fonts;

/// <summary>A font provider implementation specifically for the WpfMath assembly.</summary>
internal sealed class WpfMathFontProvider : IFontProvider
{
    private WpfMathFontProvider() {}

    public static readonly WpfMathFontProvider Instance = new();

    static WpfMathFontProvider()
    {
        // If the application isn't WPF, pack scheme doesn't get registered.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Application.ResourceAssembly == null)
        {
            Application.ResourceAssembly = Assembly.GetExecutingAssembly();
            if (!UriParser.IsKnownScheme("pack"))
                UriParser.Register(new GenericUriParser(GenericUriParserOptions.GenericAuthority), "pack", -1);
        }
    }

    private const string FontsDirectory = "Fonts/";

    public IFontTypeface ReadFontFile(string fontFileName)
    {
        var fontUri = new Uri($"pack://application:,,,/WpfMath;component/{FontsDirectory}{fontFileName}");
        return new WpfGlyphTypeface(new GlyphTypeface(fontUri));
    }
}
