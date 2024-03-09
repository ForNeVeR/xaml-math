using System;
using System.Collections.Generic;
using XamlMath.Exceptions;
using XamlMath.Fonts;
using XamlMath.Utils;

namespace XamlMath;

/// <summary>Default implementation of ITeXFont that reads all font information from XML file.</summary>
internal sealed class DefaultTexFont : ITeXFont
{
    private readonly IReadOnlyDictionary<string, double> parameters;
    private readonly IReadOnlyDictionary<string, object> generalSettings;
    private readonly IReadOnlyDictionary<string, IReadOnlyList<CharFont>> textStyleMappings;
    private readonly IReadOnlyDictionary<string, CharFont> symbolMappings;
    internal readonly IReadOnlyList<string> defaultTextStyleMappings;
    private readonly IReadOnlyList<TexFontInfo> fontInfoList;
   
    private double GetParameter(string name)
    {
        return parameters[name];
    }

    private double GetSizeFactor(TexStyle style)
    {
        if (style < TexStyle.Script)
            return 1d;
        else if (style < TexStyle.ScriptScript)
            return (double)generalSettings["scriptfactor"];
        else
            return (double)generalSettings["scriptscriptfactor"];
    }

    public DefaultTexFont(IFontProvider fontProvider, double size)
    {
        var parser = new DefaultTexFontParser(fontProvider);
        parameters = parser.GetParameters();
        generalSettings = parser.GetGeneralSettings();
        textStyleMappings = parser.GetTextStyleMappings();
        defaultTextStyleMappings = parser.GetDefaultTextStyleMappings();
        symbolMappings = parser.GetSymbolMappings();
        fontInfoList = parser.GetFontDescriptions();
        // Check that Mu font exists.
        var muFontId = (int)generalSettings["mufontid"];
        if (muFontId < 0 || muFontId >= fontInfoList.Count || fontInfoList[muFontId] == null)
            throw new InvalidOperationException("ID of Mu font is invalid.");

        this.Size = size;
    }

    public bool SupportsMetrics => true;

    public double Size { get; }

    public ExtensionChar GetExtension(CharInfo charInfo, TexStyle style)
    {
        var sizeFactor = GetSizeFactor(style);

        // Create character for each part of extension.
        var fontInfo = fontInfoList[charInfo.FontId];
        var extension = fontInfo.GetExtension(charInfo.Character);
        var parts = new CharInfo?[extension.Length];
        for (int i = 0; i < extension.Length; i++)
        {
            if (extension[i] == (int)TexCharKind.None)
                parts[i] = null;
            else
            {
                var metrics = GetMetrics(new CharFont((char)extension[i], charInfo.FontId), sizeFactor).Value;
                parts[i] = new CharInfo(
                    (char)extension[i],
                    charInfo.Font,
                    sizeFactor,
                    charInfo.FontId,
                    metrics);
            }
        }

        return new ExtensionChar(parts[TexFontUtilities.ExtensionTop], parts[TexFontUtilities.ExtensionMiddle],
            parts[TexFontUtilities.ExtensionBottom], parts[TexFontUtilities.ExtensionRepeat]);
    }

    public CharFont? GetLigature(CharFont leftCharFont, CharFont rightCharFont)
    {
        if (leftCharFont.FontId != rightCharFont.FontId)
            return null;

        var fontInfo = fontInfoList[leftCharFont.FontId];
        return fontInfo.GetLigature(leftCharFont.Character, rightCharFont.Character);
    }

    public CharInfo GetNextLargerCharInfo(CharInfo charInfo, TexStyle style)
    {
        var fontInfo = fontInfoList[charInfo.FontId];
        var charFont = fontInfo.GetNextLarger(charInfo.Character);
        var newFontInfo = fontInfoList[charFont.FontId];
        return new CharInfo(
            charFont.Character,
            newFontInfo.Font,
            GetSizeFactor(style),
            charFont.FontId,
            GetMetrics(charFont, GetSizeFactor(style)).Value);
    }

    private string GetDefaultTextStyleMapping(char character)
    {
        TexCharKind GetCharKind()
        {
            if (character >= '0' && character <= '9')
                return TexCharKind.Numbers;
            else if (character >= 'a' && character <= 'z')
                return TexCharKind.Small;
            else
                return TexCharKind.Capitals;
        }

        return defaultTextStyleMappings[(int)GetCharKind()];
    }

    public Result<CharInfo> GetDefaultCharInfo(char character, TexStyle style) =>
        this.GetCharInfo(character, GetDefaultTextStyleMapping(character), style);

    private Result<CharInfo> GetCharInfo(char character, IReadOnlyList<CharFont> charFont, TexStyle style)
    {
        TexCharKind charKind;
        int charIndexOffset;
        if (char.IsDigit(character))
        {
            charKind = TexCharKind.Numbers;
            charIndexOffset = character - '0';
        }
        else if (char.IsLetter(character) && char.IsLower(character))
        {
            charKind = TexCharKind.Small;
            charIndexOffset = character - 'a';
        }
        else
        {
            charKind = TexCharKind.Capitals;
            charIndexOffset = character - 'A';
        }

        return charFont[(int)charKind] == null
            ? this.GetDefaultCharInfo(character, style)
            : this.GetCharInfo(
                new CharFont(
                    (char)(charFont[(int)charKind].Character + charIndexOffset),
                    charFont[(int)charKind].FontId),
                style);
    }

    public Result<CharInfo> GetCharInfo(char character, string textStyle, TexStyle style) =>
        textStyleMappings.TryGetValue(textStyle, out var mapping)
            ? this.GetCharInfo(character, mapping, style)
            : Result.Error<CharInfo>(new TextStyleMappingNotFoundException(textStyle));

    public Result<CharInfo> GetCharInfo(string symbolName, TexStyle style) =>
        symbolMappings.TryGetValue(symbolName, out var mapping)
            ? this.GetCharInfo(mapping, style)
            : Result.Error<CharInfo>(new SymbolMappingNotFoundException(symbolName));

    public Result<CharInfo> GetCharInfo(CharFont charFont, TexStyle style)
    {
        var size = GetSizeFactor(style);
        var fontInfo = fontInfoList[charFont.FontId];
        var metrics = GetMetrics(charFont, size);
        return metrics.Map(m => new CharInfo(charFont.Character, fontInfo.Font, size, charFont.FontId, m));
    }

    public double GetKern(CharFont leftCharFont, CharFont rightCharFont, TexStyle style)
    {
        if (leftCharFont.FontId != rightCharFont.FontId)
            return 0;

        var fontInfo = fontInfoList[leftCharFont.FontId];
        return fontInfo.GetKern(leftCharFont.Character, rightCharFont.Character,
            GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);
    }

    public double GetQuad(int fontId, TexStyle style)
    {
        var fontInfo = fontInfoList[fontId];
        return fontInfo.GetQuad(GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);
    }

    public double GetSkew(CharFont charFont, TexStyle style)
    {
        var fontInfo = fontInfoList[charFont.FontId];
        char skewChar = fontInfo.SkewCharacter;
        if (skewChar == 1)
            return 0;
        return GetKern(charFont, new CharFont(skewChar, charFont.FontId), style);
    }

    public bool HasSpace(int fontId)
    {
        var fontInfo = fontInfoList[fontId];
        return fontInfo.HasSpace();
    }

    public bool HasNextLarger(CharInfo charInfo)
    {
        var fontInfo = fontInfoList[charInfo.FontId];
        return (fontInfo.GetNextLarger(charInfo.Character) != null);
    }

    public bool IsExtensionChar(CharInfo charInfo)
    {
        var fontInfo = fontInfoList[charInfo.FontId];
        return fontInfo.GetExtension(charInfo.Character) != null;
    }

    public int GetMuFontId()
    {
        return (int)generalSettings["mufontid"];
    }

    public double GetXHeight(TexStyle style, int fontCode)
    {
        var fontInfo = fontInfoList[fontCode];
        return fontInfo.GetXHeight(GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);
    }

    public double GetSpace(TexStyle style)
    {
        var spaceFontId = (int)generalSettings["spacefontid"];
        var fontInfo = fontInfoList[spaceFontId];
        return fontInfo.GetSpace(GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint);
    }

    public double GetAxisHeight(TexStyle style)
    {
        return GetParameter("axisheight") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetBigOpSpacing1(TexStyle style)
    {
        return GetParameter("bigopspacing1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetBigOpSpacing2(TexStyle style)
    {
        return GetParameter("bigopspacing2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetBigOpSpacing3(TexStyle style)
    {
        return GetParameter("bigopspacing3") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetBigOpSpacing4(TexStyle style)
    {
        return GetParameter("bigopspacing4") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetBigOpSpacing5(TexStyle style)
    {
        return GetParameter("bigopspacing5") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetSub1(TexStyle style)
    {
        return GetParameter("sub1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetSub2(TexStyle style)
    {
        return GetParameter("sub2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetSubDrop(TexStyle style)
    {
        return GetParameter("subdrop") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetSup1(TexStyle style)
    {
        return GetParameter("sup1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetSup2(TexStyle style)
    {
        return GetParameter("sup2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetSup3(TexStyle style)
    {
        return GetParameter("sup3") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetSupDrop(TexStyle style)
    {
        return GetParameter("supdrop") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetNum1(TexStyle style)
    {
        return GetParameter("num1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetNum2(TexStyle style)
    {
        return GetParameter("num2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetNum3(TexStyle style)
    {
        return GetParameter("num3") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetDenom1(TexStyle style)
    {
        return GetParameter("denom1") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetDenom2(TexStyle style)
    {
        return GetParameter("denom2") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    public double GetDefaultLineThickness(TexStyle style)
    {
        return GetParameter("defaultrulethickness") * GetSizeFactor(style) * TexFontUtilities.PixelsPerPoint;
    }

    private Result<TeXFontMetrics> GetMetrics(CharFont charFont, double size)
    {
        var fontInfo = fontInfoList[charFont.FontId];
        var metrics = fontInfo.GetMetrics(charFont.Character);
        return metrics.Map(m => new TeXFontMetrics(
            m[TexFontUtilities.MetricsWidth],
            m[TexFontUtilities.MetricsHeight],
            m[TexFontUtilities.MetricsDepth],
            m[TexFontUtilities.MetricsItalic],
            size * TexFontUtilities.PixelsPerPoint));
    }
}
