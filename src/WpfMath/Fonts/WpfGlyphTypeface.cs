using System.Windows.Media;
using XamlMath.Fonts;

namespace WpfMath.Fonts;

internal record WpfGlyphTypeface(GlyphTypeface Typeface) : IFontTypeface;
