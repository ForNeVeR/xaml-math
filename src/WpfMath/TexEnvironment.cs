using System.Windows.Media;

namespace WpfMath
{
    // Specifies current graphical parameters used to create boxes.
    internal class TexEnvironment
    {
        // ID of font that was last used.
        private int lastFontId = TexFontUtilities.NoFontId;

        public TexEnvironment(TexStyle style, ITeXFont texFont)
            : this(style, texFont, null, null)
        {
        }

        private TexEnvironment(TexStyle style, ITeXFont texFont, Brush background, Brush foreground)
        {
            if (style == TexStyle.Display || style == TexStyle.Text ||
                style == TexStyle.Script || style == TexStyle.ScriptScript)
                this.Style = style;
            else
                this.Style = TexStyle.Display;

            this.TexFont = texFont;
            this.Background = background;
            this.Foreground = foreground;
        }

        public TexStyle Style
        {
            get;
            private set;
        }

        public ITeXFont TexFont
        {
            get;
            private set;
        }

        public Brush Background
        {
            get;
            set;
        }

        public Brush Foreground
        {
            get;
            set;
        }

        public int LastFontId
        {
            get { return this.lastFontId == TexFontUtilities.NoFontId ? this.TexFont.GetMuFontId() : this.lastFontId; }
            set { this.lastFontId = value; }
        }

        public TexEnvironment GetCrampedStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (int)this.Style % 2 == 1 ? this.Style : this.Style + 1;
            return newEnvironment;
        }

        public TexEnvironment GetNumeratorStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = this.Style + 2 - 2 * ((int)this.Style / 6);
            return newEnvironment;
        }

        public TexEnvironment GetDenominatorStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)this.Style / 2) + 1 + 2 - 2 * ((int)this.Style / 6));
            return newEnvironment;
        }

        public TexEnvironment GetRootStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = TexStyle.ScriptScript;
            return newEnvironment;
        }

        public TexEnvironment GetSubscriptStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)this.Style / 4) + 4 + 1);
            return newEnvironment;
        }

        public TexEnvironment GetSuperscriptStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)this.Style / 4) + 4 + ((int)this.Style % 2));
            return newEnvironment;
        }

        public TexEnvironment Clone()
        {
            return new TexEnvironment(this.Style, this.TexFont, this.Background, this.Foreground);
        }

        public TexEnvironment WithFont(ITeXFont font)
        {
            var env = Clone();
            env.TexFont = font;
            return env;
        }

        public void Reset()
        {
            this.Background = null;
            this.Foreground = null;
        }
    }
}
