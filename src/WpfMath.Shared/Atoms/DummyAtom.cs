using WpfMath.Boxes;
using WpfMath.Fonts;
using WpfMath.Utils;

namespace WpfMath.Atoms
{
    // Dummy atom representing atom whose type can change or which can be replaced by a ligature.
    internal record DummyAtom : Atom
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
            new DummyAtom(TexAtomType.None, ligatureAtom, false);

        public Atom Atom { get; init; }

        public bool IsTextSymbol { get; init; }

        public bool IsKern
        {
            get { return this.Atom is SpaceAtom; }
        }

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
}
