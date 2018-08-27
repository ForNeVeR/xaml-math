using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    internal class CasesAtom : Atom
    {
        public CasesAtom(SourceSpan source,Atom atom, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
            this.BaseAtom = atom;
        }

        public Atom BaseAtom { get; private set; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var resultBox = new HorizontalBox();
            var innermatrix = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);
            var heightdiff = innermatrix.Depth-innermatrix.Height;
           
            var leftparen = DelimiterFactory.CreateBox("lbrace", innermatrix.TotalHeight+heightdiff, environment);

            leftparen.Shift = -innermatrix.Height;
            innermatrix.Shift = heightdiff/2;
            resultBox.Add(leftparen);
            resultBox.Add(innermatrix);

            return resultBox;
        }
    }
}
