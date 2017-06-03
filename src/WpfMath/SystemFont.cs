using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
{
    internal class SystemFont : ITeXFont
    {
        private readonly FontFamily _fontFamily;

        public SystemFont(double size, FontFamily fontFamily)
        {
            _fontFamily = fontFamily;
            Size = size;
        }

        public double Size { get; }

        public ITeXFont DeriveFont(double newSize)
        {
            throw new NotImplementedException();
        }

        public ExtensionChar GetExtension(CharInfo charInfo, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public CharFont GetLigature(CharFont leftChar, CharFont rightChar) => null;

        public CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public CharInfo GetDefaultCharInfo(char character, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public CharInfo GetCharInfo(char character, string textStyle, TexStyle style)
        {
            var typeface = GetTypeface();
            if (!typeface.TryGetGlyphTypeface(out var glyphTypeface))
            {
                throw new Exception("Cannot get typeface"); // TODO[F]: Base class for exception; explain the error more
            }

            var metrics = GetFontMetrics(character, typeface);
            return new CharInfo(character, glyphTypeface, 1.0, null, metrics);
        }

        public CharInfo GetCharInfo(CharFont charFont, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public CharInfo GetCharInfo(string name, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetKern(CharFont leftChar, CharFont rightChar, TexStyle style) => 0.0;

        public double GetQuad(int fontId, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSkew(CharFont charFont, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public bool HasSpace(int fontId)
        {
            throw new NotImplementedException();
        }

        public bool HasNextLarger(CharInfo charInfo)
        {
            throw new NotImplementedException();
        }

        public bool IsExtensionChar(CharInfo charInfo)
        {
            throw new NotImplementedException();
        }

        public int GetMuFontId()
        {
            throw new NotImplementedException();
        }

        public double GetXHeight(TexStyle style, int fontId)
        {
            throw new NotImplementedException();
        }

        public double GetSpace(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetAxisHeight(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetBigOpSpacing1(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetBigOpSpacing2(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetBigOpSpacing3(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetBigOpSpacing4(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetBigOpSpacing5(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSub1(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSub2(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSubDrop(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSup1(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSup2(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSup3(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetSupDrop(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetNum1(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetNum2(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetNum3(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetDenom1(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetDenom2(TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetDefaultLineThickness(TexStyle style)
        {
            throw new NotImplementedException();
        }

        private TexFontMetrics GetFontMetrics(char c, Typeface typeface)
        {
            var formattedText = new FormattedText(c.ToString(),
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                1.0,
                Brushes.Black);
            return new TexFontMetrics(formattedText.Width, formattedText.Height, 0.0, formattedText.Width, 1.0);
        }

        private Typeface GetTypeface() => new Typeface(_fontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal); // TODO[F]: Put into lazy field
    }
}
