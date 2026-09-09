using XamlMath.Boxes;

namespace XamlMath.Atoms;

/// <summary>An atom that forces a specific <see cref="TexStyle"/> for its contents.</summary>
internal sealed record StyleAtom : Atom
{
    public StyleAtom(SourceSpan? source, Atom? baseAtom, TexStyle targetStyle)
        : base(source)
    {
        BaseAtom = baseAtom ?? new NullAtom();
        TargetStyle = targetStyle;
    }

    public Atom BaseAtom { get; }

    public TexStyle TargetStyle { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        var newEnvironment = environment with { Style = TargetStyle };
        return BaseAtom.CreateBox(newEnvironment);
    }

    public override TexAtomType GetLeftType() => BaseAtom.GetLeftType();

    public override TexAtomType GetRightType() => BaseAtom.GetRightType();
}
