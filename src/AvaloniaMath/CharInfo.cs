using Avalonia.Media;

namespace WpfMath
{
    // Single character together with information about font and metrics.
    internal class CharInfo
    {
        public CharInfo(char character, Typeface font, double size, int fontId, TexFontMetrics metrics)
        {
            this.Character = character;
            this.Font = font;
            this.Size = size;
            FontId = fontId;
            this.Metrics = metrics;
        }

        public char Character
        {
            get;
            set;
        }

        public Typeface Font
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
        }

        public CharFont GetCharacterFont()
        {
            return new CharFont(Character, FontId);
        }
    }
}
