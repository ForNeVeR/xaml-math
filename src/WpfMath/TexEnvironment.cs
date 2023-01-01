using System.Windows.Media;

namespace WpfMath
{
    /// <summary>Specifies current graphical parameters used to create boxes.</summary>
    public sealed class TexEnvironment
    {
        // ID of font that was last used.
        private int lastFontId = TexFontUtilities.NoFontId;

        internal TexEnvironment(
            TexStyle style,
            ITeXFont mathFont,
            ITeXFont textFont,
            Brush? background = null,
            Brush? foreground = null)
        {
            if (style == TexStyle.Display || style == TexStyle.Text ||
                style == TexStyle.Script || style == TexStyle.ScriptScript)
                this.Style = style;
            else
                this.Style = TexStyle.Display;

            this.MathFont = mathFont;
            TextFont = textFont;
            this.Background = background;
            this.Foreground = foreground;
        }

        internal TexStyle Style
        {
            get;
            private set;
        }

        internal ITeXFont MathFont
        {
            get;
            private set;
        }

        internal ITeXFont TextFont { get; }

        internal Brush? Background
        {
            get;
            set;
        }

        internal Brush? Foreground
        {
            get;
            set;
        }

        internal int LastFontId
        {
            get { return this.lastFontId == TexFontUtilities.NoFontId ? this.MathFont.GetMuFontId() : this.lastFontId; }
            set { this.lastFontId = value; }
        }

        internal TexEnvironment GetCrampedStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (int)this.Style % 2 == 1 ? this.Style : this.Style + 1;
            return newEnvironment;
        }

        internal TexEnvironment GetNumeratorStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = this.Style + 2 - 2 * ((int)this.Style / 6);
            return newEnvironment;
        }

        internal TexEnvironment GetDenominatorStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)this.Style / 2) + 1 + 2 - 2 * ((int)this.Style / 6));
            return newEnvironment;
        }

        internal TexEnvironment GetRootStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = TexStyle.ScriptScript;
            return newEnvironment;
        }

        internal TexEnvironment GetSubscriptStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)this.Style / 4) + 4 + 1);
            return newEnvironment;
        }

        internal TexEnvironment GetSuperscriptStyle()
        {
            var newEnvironment = Clone();
            newEnvironment.Style = (TexStyle)(2 * ((int)this.Style / 4) + 4 + ((int)this.Style % 2));
            return newEnvironment;
        }

        internal TexEnvironment Clone()
        {
            return new TexEnvironment(Style, MathFont, TextFont, Background, Foreground);
        }

        internal void Reset()
        {
            this.Background = null;
            this.Foreground = null;
        }
    }
}
