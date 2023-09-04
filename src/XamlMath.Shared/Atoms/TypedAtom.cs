using XamlMath.Boxes;

namespace XamlMath.Atoms;

// Atom representing other atom with custom left and right types.
internal sealed record TypedAtom : Atom
{
    public TypedAtom(SourceSpan? source, Atom? atom, TexAtomType leftType, TexAtomType rightType)
        : base(source)
    {
        this.Atom = atom;
        this.LeftType = leftType;
        this.RightType = rightType;
    }

    public Atom? Atom { get; }

    public TexAtomType LeftType { get; }

    public TexAtomType RightType { get; }

    protected override Box CreateBoxCore(TexEnvironment environment) =>
        this.Atom!.CreateBox(environment); // Nullable TODO: This probably needs null checking

    public override TexAtomType GetLeftType()
    {
        return this.LeftType;
    }

    public override TexAtomType GetRightType()
    {
        return this.RightType;
    }
}
