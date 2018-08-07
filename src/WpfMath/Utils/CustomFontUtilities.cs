using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WpfMath.Utils
{
    public static class CustomFontUtilities
    {
        private static string FONTNAME = null;
        private static string FONTPATH = null;

        /// <summary>
        /// Initializes the <see cref="CustomFontUtilities"/> with the specified <paramref name="fontName"/> and <paramref name="fontpath"/>.
        /// </summary>
        /// <param name="fontName">The name of the font.</param>
        /// <param name="fontpath">The path of the font.</param>
        public static void Init(string fontName, string fontpath)
        {
            FONTNAME=fontName;
            FONTPATH = fontpath;
        }

        /// <summary>
        /// Puts the fields in their original state.
        /// </summary>
        public static void CloseIt()
        {
            FONTNAME = null;
            FONTPATH = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fontName"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static ObservableCollection<UnicodeListItem> GetFontChars(string fontName, int start, int end)
        {
            ObservableCollection<UnicodeListItem> grouplist = new ObservableCollection<UnicodeListItem>();
            FontFamily family = GetFontFamily();
            for (Int32 i = start; i <= end; i++)
            {
                try
                {
                    if (TypefaceContainsCharacter(GetTypefacefromFile( FontStyles.Normal, FontWeights.Normal), Convert.ToChar(i)))
                    {
                        UnicodeListItem listItem = new UnicodeListItem {
                            Point = i,
                            UnicodeText = $"{Convert.ToChar(i)}",
                            UnicodeListItemFontFamily= GetFontFamily()
                        };
                        grouplist.Add(listItem);
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
                
            }
            return grouplist;
        }

        /// <summary>
        /// Returns a <see cref="FontFamily"/> based on the specified <paramref name="fontname"/> and/or <paramref name="fontsrc"/>.
        /// </summary>
        /// <param name="fontname">The name of the font.</param>
        /// <param name="fontsrc">The fonts' Path.</param>
        /// <returns></returns>
        public static FontFamily GetFontFamily()
        {
            //FontDescrInfoDict[fontname]
            var item = FONTPATH == null ? new FontFamily(ValidateFontName(FONTNAME)) : new FontFamily(new Uri(FONTPATH), ValidateFontName(FONTNAME));
            
            if (FONTPATH != null&&FONTPATH.Length>3)
            {
                var res = Fonts.GetFontFamilies(FONTPATH);
                return res.First();
            }
            else if (FONTNAME!=null&&FONTPATH==null)
            {
                var res =new FontFamily(FONTNAME);
                return res;
            }
            else
            {
                return new FontFamily(new Uri("pack://application:,,,/Fonts/"), "./#Asana Math");
            }

        }

        public static Typeface GetTypefacefromFile(FontStyle fontStyle, FontWeight fontWeight)
        {
            return new Typeface(GetFontFamily(), fontStyle, fontWeight, FontStretches.Normal, GetFontFamily());
        }

        /// <summary>
        /// Gets the proper format of  the specified <paramref name="inputFontname"/>.
        /// </summary>
        /// <param name="inputFontname"></param>
        /// <returns></returns>
        public static string ValidateFontName(string inputFontname)
        {
            string input = inputFontname.Split('.')[0];
            string result = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetterOrDigit(input[i]))
                {
                    result += input[i];
                }
                else
                {
                    result += " ";
                }
            }
            return result;
        }

        /// <summary>
        /// Represents a Symbol and its Codepoint.
        /// </summary>
        public class UnicodeListItem
        {
            public string CodeName { get; set; }
            public int CodeId { get; set; }
            /// <summary>
            /// Gets or sets the codepoint of the unicode <see cref="char"/>.
            /// </summary>
            public int Point { get; set; }
            /// <summary>
            /// Gets or sets the symbol of the unicode <see cref="char"/>.
            /// </summary>
            public string UnicodeText { get; set; }
            public FontFamily UnicodeListItemFontFamily { get; set; }
            /// <summary>
            /// Gets or sets a value specifying whether the current <see cref="UnicodeListItem"/> has been added.
            /// </summary>
            public bool IsAdded { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public double? Depth { get; set; }
            public double? Italic { get; set; }
            public bool HasDepth { get { return Depth != null; } }
            public bool HasItalic { get { return Italic != null; } }
            public Brush Background { get; set; }
            public Brush Foreground { get; set; }
        }

        /// <summary>
        /// Specifies whether the typeface contains the given char.
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="chartocheck"></param>
        /// <returns></returns>
        public static bool TypefaceContainsCharacter(Typeface typeface, char chartocheck)
        {
            int unicodeval = Convert.ToUInt16(chartocheck);
            typeface.TryGetGlyphTypeface(out GlyphTypeface glyph);
            if (glyph != null && glyph.CharacterToGlyphMap.TryGetValue(unicodeval, out ushort glyphIndex))
            {
                return true;
            }
            return false;
        }

        private static Dictionary<string, double> GetFontProperties(Typeface typeface)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            typeface.TryGetGlyphTypeface(out GlyphTypeface glyph);
            result.Add("xHeight", typeface.XHeight);
            result.Add("capsHeight", typeface.CapsHeight);
            
            return result;
        }


    }
}
