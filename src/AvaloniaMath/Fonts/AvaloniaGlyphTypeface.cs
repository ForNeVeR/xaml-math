using Avalonia.Media;
using XamlMath.Fonts;

namespace AvaloniaMath.Fonts;

internal record AvaloniaGlyphTypeface(Typeface Value) : IFontTypeface;

internal static class AvaloniaGlyphExtensions
{
    public static IFontTypeface ToPlatform(this Typeface typeface) => new AvaloniaGlyphTypeface(typeface);

    public static Typeface ToAvalonia(this IFontTypeface typeface) =>
        ((AvaloniaGlyphTypeface)typeface).Value;
}
