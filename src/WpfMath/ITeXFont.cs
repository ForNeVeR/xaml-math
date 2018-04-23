namespace WpfMath
{
    // Font that specifies how TexFormula objects are rendered.
    internal interface ITeXFont
    {
        /// <summary>Whether the font supports <see cref="CharInfo"/>.</summary>
        bool SupportsMetrics { get; }

        /// <summary>Whether the font supports the named symbol.</summary>
        bool SupportsSymbol(string name);

        /// <summary>Whether the font supports the character with default size.</summary>
        bool SupportsDefaultCharacter(char character, TexStyle style);

        /// <summary>Whether the font supports the character with the passed style.</summary>
        bool SupportsCharacter(char character, string textStyle, TexStyle style);

        double Size { get; }

        ITeXFont DeriveFont(double newSize);

        ExtensionChar GetExtension(CharInfo charInfo, TexStyle style);

        CharFont GetLigature(CharFont leftChar, CharFont rightChar);

        CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style);

        CharInfo GetDefaultCharInfo(char character, TexStyle style, bool assert = true);

        CharInfo GetCharInfo(char character, string textStyle, TexStyle style, bool assert = true);

        CharInfo GetCharInfo(CharFont charFont, TexStyle style, bool assert = true);

        CharInfo GetCharInfo(string name, TexStyle style);

        double GetKern(CharFont leftChar, CharFont rightChar, TexStyle style);

        double GetQuad(int fontId, TexStyle style);

        double GetSkew(CharFont charFont, TexStyle style);

        bool HasSpace(int fontId);

        bool HasNextLarger(CharInfo charInfo);

        bool IsExtensionChar(CharInfo charInfo);

        int GetMuFontId();

        double GetXHeight(TexStyle style, int fontId);

        double GetSpace(TexStyle style);

        double GetAxisHeight(TexStyle style);

        double GetBigOpSpacing1(TexStyle style);

        double GetBigOpSpacing2(TexStyle style);

        double GetBigOpSpacing3(TexStyle style);

        double GetBigOpSpacing4(TexStyle style);

        double GetBigOpSpacing5(TexStyle style);

        double GetSub1(TexStyle style);

        double GetSub2(TexStyle style);

        double GetSubDrop(TexStyle style);

        double GetSup1(TexStyle style);

        double GetSup2(TexStyle style);

        double GetSup3(TexStyle style);

        double GetSupDrop(TexStyle style);

        double GetNum1(TexStyle style);

        double GetNum2(TexStyle style);

        double GetNum3(TexStyle style);

        double GetDenom1(TexStyle style);

        double GetDenom2(TexStyle style);

        double GetDefaultLineThickness(TexStyle style);
    }
}
