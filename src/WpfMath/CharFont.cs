using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    /// <summary>
    /// Single character together with specific font.
    /// </summary>
    public class CharFont
    {
        public CharFont(char character, int fontId)
        {
            this.Character = character;
            this.FontId = fontId;
        }

        public char Character
        {
            get;
            private set;
        }

        public int FontId
        {
            get;
            private set;
        }
    }
}
