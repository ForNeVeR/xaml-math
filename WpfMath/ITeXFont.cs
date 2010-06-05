using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Font that specifies how TexFormula objects are rendered.
    internal interface ITeXFont
    {
        double Size { get; }

        ITeXFont DeriveFont(double newSize);

        ExtensionChar GetExtension(WpfMath.CharInfo charInfo, TexStyle style);

        WpfMath.CharFont GetLigature(WpfMath.CharFont leftChar, WpfMath.CharFont rightChar);

        WpfMath.CharInfo GetNextLargerCharInfo(WpfMath.CharInfo charInfo, TexStyle style);

        WpfMath.CharInfo GetDefaultCharInfo(char character, TexStyle style);

        WpfMath.CharInfo GetCharInfo(char character, string textStyle, TexStyle style);

        WpfMath.CharInfo GetCharInfo(WpfMath.CharFont charFont, TexStyle style);

        WpfMath.CharInfo GetCharInfo(string name, TexStyle style);

        double GetKern(WpfMath.CharFont leftChar, WpfMath.CharFont rightChar, TexStyle style);

        double GetQuad(int fontId, TexStyle style);

        double GetSkew(WpfMath.CharFont charFont, TexStyle style);

        bool HasSpace(int fontId);

        bool HasNextLarger(WpfMath.CharInfo charInfo);

        bool IsExtensionChar(WpfMath.CharInfo charInfo);

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
