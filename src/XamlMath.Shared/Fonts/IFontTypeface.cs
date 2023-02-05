namespace XamlMath.Fonts;

/// <summary>
/// A marker interface for font typeface. Used to extract font glyphs. Should be casted back in the
/// implementation-specific code (e.g. WPF implementation may cast this to a WPF-specific type).
/// </summary>
public interface IFontTypeface
{
}
