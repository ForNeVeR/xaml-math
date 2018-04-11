namespace WpfMath
{
    // Atom representing other atom that is not rendered.
    internal class PhantomAtom : Atom, IRow
    {
        private readonly bool useWidth;
        private readonly bool useHeight;
        private readonly bool useDepth;

        public PhantomAtom(Atom baseAtom)
            : this(baseAtom, true, true, true)
        {
        }

        public PhantomAtom(Atom baseAtom, bool useWidth, bool useHeight, bool useDepth)
        {
            this.RowAtom = baseAtom == null ? new RowAtom() : new RowAtom(baseAtom);
            this.useWidth = useWidth;
            this.useHeight = useHeight;
            this.useDepth = useDepth;
        }

        public Atom WithPreviousAtom(DummyAtom previousAtom) =>
            new PhantomAtom(this.RowAtom.WithPreviousAtom(previousAtom), this.useWidth, this.useHeight, this.useDepth);

        public RowAtom RowAtom { get; }

        public override Atom Copy()
        {
            return CopyTo(new PhantomAtom(RowAtom?.Copy(), useWidth, useHeight, useDepth) { PreviousAtom = (DummyAtom)PreviousAtom?.Copy() });
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var resultBox = this.RowAtom.CreateBox(environment);
            return new StrutBox((this.useWidth ? resultBox.Width : 0), (this.useHeight ? resultBox.Height : 0),
                (this.useDepth ? resultBox.Depth : 0), resultBox.Shift);
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
}
