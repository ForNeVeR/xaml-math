using XamlMath.Boxes;
using XamlMath.Rendering;

namespace XamlMath.Atoms;

/// <summary>Atom specifying graphical style.</summary>
internal sealed record StyledAtom : Atom, IRow
{
    public StyledAtom(
        SourceSpan? source,
        Atom? atom,
        IBrush? backgroundColor,
        IBrush? foregroundColor)
        : base(source)
    {
        this.RowAtom = new RowAtom(source, atom);
        this.Background = backgroundColor;
        this.Foreground = foregroundColor;
    }

    // RowAtom to which colors are applied.
    public RowAtom RowAtom { get; init; }

    public IBrush? Background { get; init; }

    public IBrush? Foreground { get; init; }

    public Atom WithPreviousAtom(DummyAtom? previousAtom) =>
        this with { RowAtom = (RowAtom) RowAtom.WithPreviousAtom(previousAtom) };

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        var newEnvironment = environment with
        {
            Foreground = this.Foreground ?? environment.Foreground
        };
        var childBox = this.RowAtom.CreateBox(newEnvironment);
        if (Background != null)
            childBox.Background = Background;
        return childBox;
    }

    public override TexAtomType GetLeftType()
    {
        return this.RowAtom.GetLeftType();
    }

    public override TexAtomType GetRightType()
    {
        return this.RowAtom.GetRightType();
    }
}
