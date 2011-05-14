using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
{
    // Default implementation of ITeXFont that reads all font information from XML file.
    internal class DefaultTexFont : ITeXFont
    {
        private static readonly IDictionary<string, double> parameters;
        private static readonly IDictionary<string, object> generalSettings;
        private static readonly IDictionary<string, CharFont[]> textStyleMappings;
        private static readonly IDictionary<string, CharFont> symbolMappings;
        internal static readonly IList<string> defaultTextStyleMappings;
        private static readonly IList<TexFontInfo> fontInfoList;

        static DefaultTexFont()
        {
            var parser = new DefaultTexFontParser();
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
        }

        private static double GetParameter(string name)
        {
            return parameters[name];
        }

        private static double GetSizeFactor(TexStyle style)
        {
            if (style < TexStyle.Script)
                return 1d;
            else if (style < TexStyle.ScriptScript)
                return (double)generalSettings["scriptfactor"];
            else
                return (double)generalSettings["scriptscriptfactor"];
        }

        public DefaultTexFont(double size)
        {
            this.Size = size;
        }

        public double Size
        {
            get;
            private set;
        }

        public ITeXFont DeriveFont(double newSize)
        {
            return new DefaultTexFont(newSize);
        }

        public ExtensionChar GetExtension(CharInfo charInfo, TexStyle style)
        {
            var sizeFactor = GetSizeFactor(style);

            // Create character for each part of extension.
            var fontInfo = fontInfoList[charInfo.FontId];
            var extension = fontInfo.GetExtension(charInfo.Character);
            var parts = new CharInfo[extension.Length];
            for (int i = 0; i < extension.Length; i++)
            {
                if (extension[i] == (int)TexCharKind.None)
                    parts[i] = null;
                else
                    parts[i] = new CharInfo((char)extension[i], charInfo.Font, sizeFactor, charInfo.FontId,
                        GetMetrics(new CharFont((char)extension[i], charInfo.FontId), sizeFactor));
            }

            return new ExtensionChar(parts[TexFontUtilities.ExtensionTop], parts[TexFontUtilities.ExtensionMiddle],
                parts[TexFontUtilities.ExtensionBottom], parts[TexFontUtilities.ExtensionRepeat]);
        }

        public CharFont GetLigature(CharFont leftCharFont, CharFont rightCharFont)
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
            return new CharInfo(charFont.Character, newFontInfo.Font, GetSizeFactor(style), charFont.FontId,
                GetMetrics(charFont, GetSizeFactor(style)));
        }

        public CharInfo GetDefaultCharInfo(char character, TexStyle style)
        {
            if (character >= '0' && character <= '9')
                return GetCharInfo(character, defaultTextStyleMappings[(int)TexCharKind.Numbers], style);
            else if (character >= 'a' && character <= 'z')
                return GetCharInfo(character, defaultTextStyleMappings[(int)TexCharKind.Small], style);
            else
                return GetCharInfo(character, defaultTextStyleMappings[(int)TexCharKind.Capitals], style);
        }

        private CharInfo GetCharInfo(char character, CharFont[] charFont, TexStyle style)
        {
            TexCharKind charKind;
            int charIndexOffset;
            if (character >= '0' && character <= '9')
            {
                charKind = TexCharKind.Numbers;
                charIndexOffset = character - '0';
            }
            else if (character >= 'a' && character <= 'z')
            {
                charKind = TexCharKind.Small;
                charIndexOffset = character - 'a';
            }
            else
            {
                charKind = TexCharKind.Capitals;
                charIndexOffset = character - 'A';
            }

            if (charFont[(int)charKind] == null)
                return GetDefaultCharInfo(character, style);
            else
                return GetCharInfo(new CharFont((char)(charFont[(int)charKind].Character + charIndexOffset),
                    charFont[(int)charKind].FontId), style);
        }

        public CharInfo GetCharInfo(char character, string textStyle, TexStyle style)
        {
            var mapping = textStyleMappings[textStyle];
            if (mapping == null)
                throw new TextStyleMappingNotFoundException(textStyle);
            return GetCharInfo(character, (CharFont[])mapping, style);
        }

        public CharInfo GetCharInfo(string symbolName, TexStyle style)
        {
            var mapping = symbolMappings[symbolName];
            if (mapping == null)
                throw new SymbolMappingNotFoundException(symbolName);
            return GetCharInfo((CharFont)mapping, style);
        }

        public CharInfo GetCharInfo(CharFont charFont, TexStyle style)
        {
            var size = GetSizeFactor(style);
            if (charFont.Character == '(' || charFont.Character == ')' || charFont.Character == '|')
                size = 1.8;
            var fontInfo = fontInfoList[charFont.FontId];
            return new CharInfo(charFont.Character, fontInfo.Font, size, charFont.FontId, GetMetrics(charFont, size));
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

        private TexFontMetrics GetMetrics(CharFont charFont, double size)
        {
            var fontInfo = fontInfoList[charFont.FontId];
            var metrics = fontInfo.GetMetrics(charFont.Character);
            return new TexFontMetrics(metrics[TexFontUtilities.MetricsWidth], metrics[TexFontUtilities.MetricsHeight],
                metrics[TexFontUtilities.MetricsDepth], metrics[TexFontUtilities.MetricsItalic],
                size * TexFontUtilities.PixelsPerPoint);
        }
    }

    internal enum TexCharKind
    {
        None = -1,
        Numbers = 0,
        Capitals = 1,
        Small = 2
    }
}
