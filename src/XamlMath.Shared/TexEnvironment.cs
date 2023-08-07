using XamlMath.Fonts;
using XamlMath.Rendering;

namespace XamlMath;

/// <summary>Specifies current graphical parameters used to create boxes.</summary>
public sealed record TexEnvironment(
    TexStyle Style,
    ITeXFont MathFont,
    ITeXFont TextFont,
    IBrush? Background = null,
    IBrush? Foreground = null)
{
    // ID of font that was last used.
    private int lastFontId = TexFontUtilities.NoFontId;

    internal int LastFontId
    {
        get => this.lastFontId == TexFontUtilities.NoFontId ? this.MathFont.GetMuFontId() : this.lastFontId;
        set => this.lastFontId = value;
    }

    internal TexEnvironment GetCrampedStyle() =>
        this with { Style = (int)this.Style % 2 == 1 ? this.Style : this.Style + 1 };

    internal TexEnvironment GetNumeratorStyle() =>
        this with { Style = this.Style + 2 - 2 * ((int)this.Style / 6) };

    internal TexEnvironment GetDenominatorStyle() =>
        this with { Style = (TexStyle)(2 * ((int)this.Style / 2) + 1 + 2 - 2 * ((int)this.Style / 6)) };

    internal TexEnvironment GetRootStyle() =>
        this with { Style = TexStyle.ScriptScript };

    internal TexEnvironment GetSubscriptStyle() =>
        this with { Style = (TexStyle)(2 * ((int)this.Style / 4) + 4 + 1) };

    internal TexEnvironment GetSuperscriptStyle() =>
        this with { Style = (TexStyle)(2 * ((int)this.Style / 4) + 4 + (int)this.Style % 2) };
}
