using System;

using Windows.UI.Xaml.Media;

using XamlMath;
using XamlMath.Fonts;
using XamlMath.Utils;

namespace UwpMath.Fonts;

internal class UwpSystemFont : ITeXFont
{
    public UwpSystemFont(double size, FontFamily fontFamily)
    {
        Size = size;
        FontFamily = fontFamily;
    }

    public double Size { get; }

    public FontFamily FontFamily { get; }

    public bool SupportsMetrics => false;

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

    public Result<CharInfo> GetCharInfo(char character, string textStyle, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public Result<CharInfo> GetCharInfo(CharFont charFont, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public Result<CharInfo> GetCharInfo(string name, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public Result<CharInfo> GetDefaultCharInfo(char character, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public double GetDefaultLineThickness(TexStyle style)
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

    public ExtensionChar GetExtension(CharInfo charInfo, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public double GetKern(CharFont leftChar, CharFont rightChar, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public CharFont? GetLigature(CharFont leftChar, CharFont rightChar)
    {
        throw new NotImplementedException();
    }

    public int GetMuFontId()
    {
        throw new NotImplementedException();
    }

    public CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style)
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

    public double GetQuad(int fontId, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public double GetSkew(CharFont charFont, TexStyle style)
    {
        throw new NotImplementedException();
    }

    public double GetSpace(TexStyle style)
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

    public double GetXHeight(TexStyle style, int fontId)
    {
        throw new NotImplementedException();
    }

    public bool HasNextLarger(CharInfo charInfo)
    {
        throw new NotImplementedException();
    }

    public bool HasSpace(int fontId)
    {
        throw new NotImplementedException();
    }

    public bool IsExtensionChar(CharInfo charInfo)
    {
        throw new NotImplementedException();
    }
}
