using XamlMath.Fonts;
using XamlMath.Utils;

namespace XamlMath.Atoms;

// Atom representing single character in specific text style.
internal sealed record CharAtom : CharSymbol
{
    public CharAtom(SourceSpan? source, char character, string? textStyle = null)
        : base(source)
    {
        this.TextStyle = textStyle;
        this.Character = character;
    }

    public char Character { get; }

    // Null means default text style.
    public string? TextStyle { get; }

    public override ITeXFont GetStyledFont(TexEnvironment environment) =>
        this.TextStyle == TexUtilities.TextStyleName ? environment.TextFont : base.GetStyledFont(environment);

    protected override Result<CharInfo> GetCharInfo(ITeXFont texFont, TexStyle style) =>
        this.TextStyle == null
            ? texFont.GetDefaultCharInfo(this.Character, style)
            : texFont.GetCharInfo(this.Character, this.TextStyle, style);

    public override Result<CharFont> GetCharFont(ITeXFont texFont) =>
        // Style is irrelevant here.
        this.GetCharInfo(texFont, TexStyle.Display).Map(ci => ci.GetCharacterFont());
}
