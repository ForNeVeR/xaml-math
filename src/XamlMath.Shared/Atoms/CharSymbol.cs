using XamlMath.Boxes;
using XamlMath.Fonts;
using XamlMath.Utils;

namespace XamlMath.Atoms;

// Atom representing single character that can be marked as text symbol.
internal abstract record CharSymbol : Atom
{
    protected CharSymbol(SourceSpan? source, TexAtomType type = TexAtomType.Ordinary)
        : base(source, type)
    {
        this.IsTextSymbol = false;
    }

    public bool IsTextSymbol { get; }

    /// <summary>Returns the preferred font to render this character.</summary>
    public virtual ITeXFont GetStyledFont(TexEnvironment environment) => environment.MathFont;

    /// <summary>Returns a <see cref="CharInfo"/> for this character.</summary>
    protected abstract Result<CharInfo> GetCharInfo(ITeXFont font, TexStyle style);

    protected sealed override Box CreateBoxCore(TexEnvironment environment)
    {
        var font = this.GetStyledFont(environment);
        var charInfo = this.GetCharInfo(font, environment.Style);
        return new CharBox(environment, charInfo.Value);
    }

    /// <summary>Checks if the symbol can be rendered by font.</summary>
    public bool IsSupportedByFont(ITeXFont font, TexStyle style) =>
        this.GetCharInfo(font, style).IsSuccess;

    /// <summary>Returns the symbol rendered by font.</summary>
    public abstract Result<CharFont> GetCharFont(ITeXFont texFont);
}
