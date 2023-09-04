using XamlMath.Boxes;

namespace XamlMath.Atoms;

internal sealed record NullAtom : Atom
{
    public NullAtom(SourceSpan? source = null, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
    {
    }

    protected override Box CreateBoxCore(TexEnvironment environment) => new StrutBox(0, 0, 0, 0);
}
