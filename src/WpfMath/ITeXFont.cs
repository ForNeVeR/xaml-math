using WpfMath.Utils;

namespace WpfMath
{
    ///<summary>Font that specifies how TexFormula objects are rendered.</summary>
    /// <remarks>
    /// For the details on values marked as "font metrics parameters", see Knuth, Donald Ervin - The TeXbook, Appendix
    /// G: Generating Boxes from Formulas.
    /// </remarks>
    internal interface ITeXFont
    {
        /// <summary>Whether the font supports <see cref="CharInfo"/>.</summary>
        bool SupportsMetrics { get; }

        double Size { get; }

        ITeXFont DeriveFont(double newSize);

        ExtensionChar GetExtension(CharInfo charInfo, TexStyle style);

        CharFont GetLigature(CharFont leftChar, CharFont rightChar);

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

        /// <summary>The x_height parameter of the font metrics.</summary>
        double GetXHeight(TexStyle style, int fontId);

        double GetSpace(TexStyle style);

        /// <summary>The axis_height parameter of the font metrics.</summary>
        double GetAxisHeight(TexStyle style);

        /// <summary>The big_op_spacing1 parameter of the font metrics.</summary>
        double GetBigOpSpacing1(TexStyle style);

        /// <summary>The big_op_spacing2 parameter of the font metrics.</summary>
        double GetBigOpSpacing2(TexStyle style);

        /// <summary>The big_op_spacing3 parameter of the font metrics.</summary>
        double GetBigOpSpacing3(TexStyle style);

        /// <summary>The big_op_spacing4 parameter of the font metrics.</summary>
        double GetBigOpSpacing4(TexStyle style);

        /// <summary>The big_op_spacing5 parameter of the font metrics.</summary>
        double GetBigOpSpacing5(TexStyle style);

        /// <summary>The sub1 parameter of the font metrics.</summary>
        double GetSub1(TexStyle style);

        /// <summary>The sub2 parameter of the font metrics.</summary>
        double GetSub2(TexStyle style);

        /// <summary>The sub_drop parameter of the font metrics.</summary>
        double GetSubDrop(TexStyle style);

        /// <summary>The sup1 parameter of the font metrics.</summary>
        double GetSup1(TexStyle style);

        /// <summary>The sup2 parameter of the font metrics.</summary>
        double GetSup2(TexStyle style);

        /// <summary>The sup3 parameter of the font metrics.</summary>
        double GetSup3(TexStyle style);

        /// <summary>The sup3 parameter of the font metrics.</summary>
        double GetSupDrop(TexStyle style);

        /// <summary>The num1 parameter of the font metrics.</summary>
        double GetNum1(TexStyle style);

        /// <summary>The num2 parameter of the font metrics.</summary>
        double GetNum2(TexStyle style);

        /// <summary>The num3 parameter of the font metrics.</summary>
        double GetNum3(TexStyle style);

        /// <summary>The denom1 parameter of the font metrics.</summary>
        double GetDenom1(TexStyle style);

        /// <summary>The denom2 parameter of the font metrics.</summary>
        double GetDenom2(TexStyle style);

        /// <summary>The default_rule_thickness parameter of the font metrics.</summary>
        double GetDefaultLineThickness(TexStyle style);
    }
}
