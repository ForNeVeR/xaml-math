using XamlMath.Boxes;
using XamlMath.Fonts;
using XamlMath.Utils;

namespace XamlMath.Atoms;

// Dummy atom representing atom whose type can change or which can be replaced by a ligature.
internal sealed record DummyAtom : Atom
{
    public DummyAtom(TexAtomType type, Atom atom, bool isTextSymbol)
        : base(atom.Source, type)
    {
        this.Atom = atom;
        this.IsTextSymbol = isTextSymbol;
    }

    public DummyAtom(Atom atom)
        : this(TexAtomType.None, atom, false)
    {
    }

    public Atom WithPreviousAtom(DummyAtom? previousAtom)
    {
        if (this.Atom is IRow row)
        {
            return this with { Atom = row.WithPreviousAtom(previousAtom) };
        }

        return this;
    }

    public static DummyAtom CreateLigature(FixedCharAtom ligatureAtom) =>
        new(TexAtomType.None, ligatureAtom, false);

    public Atom Atom { get; init; }

    public bool IsTextSymbol { get; init; }

    public bool IsKern => this.Atom is SpaceAtom;

    public Result<CharFont> GetCharFont(ITeXFont texFont) =>
        ((CharSymbol)this.Atom).GetCharFont(texFont);

    protected override Box CreateBoxCore(TexEnvironment environment) =>
        this.Atom.CreateBox(environment);

    public override TexAtomType GetLeftType()
    {
        return this.Type == TexAtomType.None ? this.Atom.GetLeftType() : this.Type;
    }

    public override TexAtomType GetRightType()
    {
        return this.Type == TexAtomType.None ? this.Atom.GetRightType() : this.Type;
    }
}
