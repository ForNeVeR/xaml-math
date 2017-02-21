using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
{
    // Single character togeter with information about font and metrics.
    internal class CharInfo
    {
        public CharInfo(char character, GlyphTypeface font, double size, int fontId, TexFontMetrics metrics)
        {
            this.Character = character;
            this.Font = font;
            this.Size = size;
            this.FontId = fontId;
            this.Metrics = metrics;
        }

        public char Character
        {
            get;
            set;
        }

        public GlyphTypeface Font
        {
            get;
            set;
        }

        public double Size
        {
            get;
            set;
        }

        public TexFontMetrics Metrics
        {
            get;
            set;
        }

        public int FontId
        {
            get;
            set;
        }

        public CharFont GetCharacterFont()
        {
            return new CharFont(this.Character, this.FontId);
        }
    }
}
