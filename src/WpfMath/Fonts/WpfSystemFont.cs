using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using XamlMath;
using XamlMath.Exceptions;
using XamlMath.Fonts;
using XamlMath.Utils;

namespace WpfMath.Fonts;

internal sealed class WpfSystemFont : ITeXFont
{
    private readonly FontFamily fontFamily;
    private readonly Lazy<Typeface> _typeface;

    public WpfSystemFont(double size, FontFamily fontFamily)
    {
        this.fontFamily = fontFamily;
        Size = size;
        _typeface = new Lazy<Typeface>(InitializeTypeface);
    }

    private Typeface InitializeTypeface() => new(this.fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

    public bool SupportsMetrics => false;

    public double Size { get; }

    public ExtensionChar GetExtension(CharInfo charInfo, TexStyle style) =>
        throw MethodNotSupported(nameof(GetExtension));

    public CharFont? GetLigature(CharFont leftChar, CharFont rightChar) => null;

    public CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style) =>
        throw MethodNotSupported(nameof(GetNextLargerCharInfo));

    public Result<CharInfo> GetDefaultCharInfo(char character, TexStyle style) =>
        Result.Error<CharInfo>(MethodNotSupported(nameof(this.GetDefaultCharInfo)));

    public Result<CharInfo> GetCharInfo(char character, string textStyle, TexStyle style)
    {
        var typeface = _typeface.Value;
        if (!typeface.TryGetGlyphTypeface(out var glyphTypeface))
        {
            return Result.Error<CharInfo>(new TypeFaceNotFoundException(
                $"Glyph typeface for font {this.fontFamily.BaseUri} was not found"));
        }

        var metrics = GetFontMetrics(character, typeface);
        return Result.Ok(
            new CharInfo(character, new WpfGlyphTypeface(glyphTypeface), 1.0, TexFontUtilities.NoFontId, metrics));
    }

    public Result<CharInfo> GetCharInfo(CharFont charFont, TexStyle style) =>
        Result.Error<CharInfo>(MethodNotSupported(nameof(this.GetCharInfo)));

    public Result<CharInfo> GetCharInfo(string name, TexStyle style) =>
        Result.Error<CharInfo>(MethodNotSupported(nameof(GetCharInfo)));

    public double GetKern(CharFont leftChar, CharFont rightChar, TexStyle style) => 0.0;

    public double GetQuad(int fontId, TexStyle style) => throw MethodNotSupported(nameof(GetQuad));

    public double GetSkew(CharFont charFont, TexStyle style) => throw MethodNotSupported(nameof(GetSkew));

    public bool HasSpace(int fontId) => throw MethodNotSupported(nameof(HasSpace));

    public bool HasNextLarger(CharInfo charInfo) => throw MethodNotSupported(nameof(HasNextLarger));

    public bool IsExtensionChar(CharInfo charInfo) => throw MethodNotSupported(nameof(IsExtensionChar));

    public int GetMuFontId() => throw MethodNotSupported(nameof(GetMuFontId));

    public double GetXHeight(TexStyle style, int fontId) => throw MethodNotSupported(nameof(GetXHeight));

    public double GetSpace(TexStyle style) => throw MethodNotSupported(nameof(GetSpace));

    public double GetAxisHeight(TexStyle style) => throw MethodNotSupported(nameof(GetAxisHeight));

    public double GetBigOpSpacing1(TexStyle style) => throw MethodNotSupported(nameof(GetBigOpSpacing1));

    public double GetBigOpSpacing2(TexStyle style) => throw MethodNotSupported(nameof(GetBigOpSpacing2));

    public double GetBigOpSpacing3(TexStyle style) => throw MethodNotSupported(nameof(GetBigOpSpacing3));

    public double GetBigOpSpacing4(TexStyle style) => throw MethodNotSupported(nameof(GetBigOpSpacing4));

    public double GetBigOpSpacing5(TexStyle style) => throw MethodNotSupported(nameof(GetBigOpSpacing5));

    public double GetSub1(TexStyle style) => throw MethodNotSupported(nameof(GetSub1));

    public double GetSub2(TexStyle style) => throw MethodNotSupported(nameof(GetSub2));

    public double GetSubDrop(TexStyle style) => throw MethodNotSupported(nameof(GetSubDrop));

    public double GetSup1(TexStyle style) => throw MethodNotSupported(nameof(GetSup1));

    public double GetSup2(TexStyle style) => throw MethodNotSupported(nameof(GetSup2));

    public double GetSup3(TexStyle style) => throw MethodNotSupported(nameof(GetSup3));

    public double GetSupDrop(TexStyle style) => throw MethodNotSupported(nameof(GetSupDrop));

    public double GetNum1(TexStyle style) => throw MethodNotSupported(nameof(GetNum1));

    public double GetNum2(TexStyle style) => throw MethodNotSupported(nameof(GetNum2));

    public double GetNum3(TexStyle style) => throw MethodNotSupported(nameof(GetNum3));

    public double GetDenom1(TexStyle style) => throw MethodNotSupported(nameof(GetDenom1));

    public double GetDenom2(TexStyle style) => throw MethodNotSupported(nameof(GetDenom2));

    public double GetDefaultLineThickness(TexStyle style) => throw MethodNotSupported(nameof(GetDefaultLineThickness));

    private static TexNotSupportedException MethodNotSupported(string callerMethod)
    {
        return new TexNotSupportedException(
            $"Call of method {callerMethod} on {nameof(WpfSystemFont)} is not supported");
    }

    private static TeXFontMetrics GetFontMetrics(char c, Typeface typeface)
    {
        var formattedText = new FormattedText(c.ToString(),
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            typeface,
            1.0,
            Brushes.Black
#if !NET452
            , 1
#endif
        );

        // The formattedText object now describes a box that contains a drawn character. To extract the actual character
        // metrics, some calculations are required. First of all, let's calculate the "depth" (see Box::Depth for the
        // details on the box model we use here).
        var depth =
            formattedText.Height - formattedText.Baseline // this gets us the part under the baseline
            + formattedText.OverhangAfter; // this is the part under the baseline plus the overhanging elements, if any

        // Now, to get the character's height above the baseline, we also need to calculate the distance from the top of
        // the box to the first character's pixel. This is a bit complex because the FormattedText only tells us the
        // whole character's Extent (the whole span of pixels), and not its position inside the box — not directly, at
        // least. But we can derive that from other properties.
        var topFreeSpace = formattedText.Height + formattedText.OverhangAfter - formattedText.Extent;

        // Now, our box model requires us to get the character's height above the baseline, plus the depth below the
        // baseline. Its total drawn height, from the box model's point of view at least, is BaseHeight + Depth.
        var height = formattedText.Baseline - topFreeSpace;

        // NOTE: A scaling factor should be taken into account for whatever reason, see
        // https://stackoverflow.com/a/45958639/2684760
        var scalingFactor = typeface.FontFamily.LineSpacing / typeface.FontFamily.Baseline;
        height /= scalingFactor;

        return new TeXFontMetrics(formattedText.Width, height, depth, 0, 1.0);
    }
}
