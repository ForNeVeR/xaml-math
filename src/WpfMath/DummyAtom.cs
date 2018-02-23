namespace WpfMath
{
    // Dummy atom representing atom whose type can change or which can be replaced by a ligature.
    internal class DummyAtom : Atom
    {
        public DummyAtom(Atom atom)
        {
            this.Type = TexAtomType.None;
            this.Atom = atom;
            this.IsTextSymbol = false;
        }

        public DummyAtom PreviousAtom
        {
            set
            {
                if (this.Atom is IRow)
                    ((IRow)this.Atom).PreviousAtom = value;
            }
        }

        public Atom Atom
        {
            get;
            private set;
        }

        public bool IsTextSymbol
        {
            get;
            set;
        }

        public bool IsCharSymbol
        {
            get { return this.Atom is CharSymbol; }
        }

        public bool IsKern
        {
            get { return this.Atom is SpaceAtom; }
        }

        public void SetLigature(FixedCharAtom ligatureAtom)
        {
            this.Atom = ligatureAtom;
            this.Type = TexAtomType.None;
            this.IsTextSymbol = false;
        }

        public CharFont GetCharFont(ITeXFont texFont)
        {
            return ((CharSymbol)this.Atom).GetCharFont(texFont);
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            if (this.IsTextSymbol)
                ((CharSymbol)this.Atom).IsTextSymbol = true;
            var resultBox = this.Atom.CreateBox(environment);
            if (this.IsTextSymbol)
                ((CharSymbol)this.Atom).IsTextSymbol = false;
            return resultBox;
        }

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
