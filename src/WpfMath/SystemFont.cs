using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WpfMath.Exceptions;
using WpfMath.Utils;

namespace WpfMath
{
    internal class SystemFont : ITeXFont
    {
        private readonly FontFamily fontFamily;

        public SystemFont(double size, FontFamily fontFamily)
        {
            this.fontFamily = fontFamily;
            Size = size;
        }

        public bool SupportsMetrics => false;

        public double Size { get; }

        public ITeXFont DeriveFont(double newSize) => throw MethodNotSupported(nameof(DeriveFont));
        public ExtensionChar GetExtension(CharInfo charInfo, TexStyle style) =>
            throw MethodNotSupported(nameof(GetExtension));

        public CharFont? GetLigature(CharFont leftChar, CharFont rightChar) => null;

        public CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style) =>
            throw MethodNotSupported(nameof(GetNextLargerCharInfo));

        public Result<CharInfo> GetDefaultCharInfo(char character, TexStyle style) =>
            Result.Error<CharInfo>(MethodNotSupported(nameof(this.GetDefaultCharInfo)));

        public Result<CharInfo> GetCharInfo(char character, string textStyle, TexStyle style)
        {
            var typeface = this.GetTypeface();
            if (!typeface.TryGetGlyphTypeface(out var glyphTypeface))
            {
                return Result.Error<CharInfo>(new TypeFaceNotFoundException(
                    $"Glyph typeface for font {this.fontFamily.BaseUri} was not found"));
            }

            var metrics = GetFontMetrics(character, typeface);
            return Result.Ok(new CharInfo(character, glyphTypeface, 1.0, TexFontUtilities.NoFontId, metrics));
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
                $"Call of method {callerMethod} on {nameof(SystemFont)} is not supported");
        }

        private TexFontMetrics GetFontMetrics(char c, Typeface typeface)
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
            return new TexFontMetrics(formattedText.Width, formattedText.Height, 0.0, formattedText.Width, 1.0);
        }

        private Typeface GetTypeface() => new Typeface(this.fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal); // TODO[F]: Put into lazy field
    }
}
