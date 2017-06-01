using System;
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

        public CharFont GetLigature(CharFont leftChar, CharFont rightChar)
        {
            throw new NotImplementedException();
        }

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

            var metrics = GetFontMetrics();
            return new CharInfo(character, glyphTypeface, Size, null, metrics);
        }

        public CharInfo GetCharInfo(CharFont charFont, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public CharInfo GetCharInfo(string name, TexStyle style)
        {
            throw new NotImplementedException();
        }

        public double GetKern(CharFont leftChar, CharFont rightChar, TexStyle style)
        {
            throw new NotImplementedException();
        }

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

        private TexFontMetrics GetFontMetrics() => throw new NotImplementedException();
        private Typeface GetTypeface() => throw new NotImplementedException();
    }
}
