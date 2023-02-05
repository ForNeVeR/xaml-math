using XamlMath.Utils;

namespace XamlMath.Fonts;

/// <summary>Font that specifies how TexFormula objects are rendered.</summary>
public interface ITeXFont
{
    /// <summary>Whether the font supports <see cref="CharInfo"/>.</summary>
    bool SupportsMetrics { get; }

    double Size { get; }

    ExtensionChar GetExtension(CharInfo charInfo, TexStyle style);

    CharFont? GetLigature(CharFont leftChar, CharFont rightChar);

    CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style);

    Result<CharInfo> GetDefaultCharInfo(char character, TexStyle style);

    Result<CharInfo> GetCharInfo(char character, string textStyle, TexStyle style);

    Result<CharInfo> GetCharInfo(CharFont charFont, TexStyle style);

    Result<CharInfo> GetCharInfo(string name, TexStyle style);

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
