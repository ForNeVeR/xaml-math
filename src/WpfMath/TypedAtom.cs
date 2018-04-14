namespace WpfMath
{
    // Atom representing other atom with custom left and right types.
    internal class TypedAtom : Atom
    {
        public TypedAtom(SourceSpan source, Atom atom, TexAtomType leftType, TexAtomType rightType)
            : base(source)
        {
            this.Atom = atom;
            this.LeftType = leftType;
            this.RightType = rightType;
        }

        public Atom Atom { get; }

        public TexAtomType LeftType { get; }

        public TexAtomType RightType { get; }

        public override Box CreateBox(TexEnvironment environment)
        {
            return this.Atom.CreateBox(environment);
        }

        public override TexAtomType GetLeftType()
        {
            return this.LeftType;
        }

        public override TexAtomType GetRightType()
        {
            return this.RightType;
        }
    }
}
