using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    // Atom representing big delimeter (e.g. brackets).
    internal class BigDelimeterAtom : Atom
    {
        public BigDelimeterAtom(SourceSpan source, Atom delimeterAtom, int size)
            : base(source)
        {
            this.DelimeterAtom = delimeterAtom;
            this.Size = size;
        }

        public Atom DelimeterAtom { get; }

        public int Size { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            // TODO
            var resultBox = (Box)null; // DelimiterFactory.CreateBox(this.DelimeterAtom, this.Size, environment);
            resultBox.Shift = -(resultBox.Height + resultBox.Depth) / 2 -
                environment.MathFont.GetAxisHeight(environment.Style);
            return resultBox;
        }
    }
}
