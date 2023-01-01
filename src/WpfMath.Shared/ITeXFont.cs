using WpfMath.Utils;

namespace WpfMath
{
    /// <summary>Font that specifies how TexFormula objects are rendered.</summary>
    public interface ITeXFont
    {
        /// <summary>Whether the font supports <see cref="CharInfo"/>.</summary>
        internal bool SupportsMetrics { get; }

        internal double Size { get; }

        internal ExtensionChar GetExtension(CharInfo charInfo, TexStyle style);

        internal CharFont? GetLigature(CharFont leftChar, CharFont rightChar);

        internal CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style);

        internal Result<CharInfo> GetDefaultCharInfo(char character, TexStyle style);

        internal Result<CharInfo> GetCharInfo(char character, string textStyle, TexStyle style);

        internal Result<CharInfo> GetCharInfo(CharFont charFont, TexStyle style);

        internal Result<CharInfo> GetCharInfo(string name, TexStyle style);

        internal double GetKern(CharFont leftChar, CharFont rightChar, TexStyle style);

        internal double GetQuad(int fontId, TexStyle style);

        internal double GetSkew(CharFont charFont, TexStyle style);

        internal bool HasSpace(int fontId);

        internal bool HasNextLarger(CharInfo charInfo);

        internal bool IsExtensionChar(CharInfo charInfo);

        internal int GetMuFontId();

        internal double GetXHeight(TexStyle style, int fontId);

        internal double GetSpace(TexStyle style);

        internal double GetAxisHeight(TexStyle style);

        internal double GetBigOpSpacing1(TexStyle style);

        internal double GetBigOpSpacing2(TexStyle style);

        internal double GetBigOpSpacing3(TexStyle style);

        internal double GetBigOpSpacing4(TexStyle style);

        internal double GetBigOpSpacing5(TexStyle style);

        internal double GetSub1(TexStyle style);

        internal double GetSub2(TexStyle style);

        internal double GetSubDrop(TexStyle style);

        internal double GetSup1(TexStyle style);

        internal double GetSup2(TexStyle style);

        internal double GetSup3(TexStyle style);

        internal double GetSupDrop(TexStyle style);

        internal double GetNum1(TexStyle style);

        internal double GetNum2(TexStyle style);

        internal double GetNum3(TexStyle style);

        internal double GetDenom1(TexStyle style);

        internal double GetDenom2(TexStyle style);

        internal double GetDefaultLineThickness(TexStyle style);
    }
}
