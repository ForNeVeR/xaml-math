using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    // Atom representing other atom that is not rendered.
    internal class PhantomAtom : Atom, IRow
    {
        private readonly bool useWidth;
        private readonly bool useHeight;
        private readonly bool useDepth;

        public PhantomAtom(
            SourceSpan source,
            Atom baseAtom,
            bool useWidth = true,
            bool useHeight = true,
            bool useDepth = true)
            : base(source)
        {
            this.RowAtom = baseAtom == null ? new RowAtom(null) : new RowAtom(null, baseAtom);
            this.useWidth = useWidth;
            this.useHeight = useHeight;
            this.useDepth = useDepth;
        }

        public Atom WithPreviousAtom(DummyAtom previousAtom) =>
            new PhantomAtom(
                this.Source,
                this.RowAtom.WithPreviousAtom(previousAtom),
                this.useWidth,
                this.useHeight,
                this.useDepth);

        public RowAtom RowAtom { get; }

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
