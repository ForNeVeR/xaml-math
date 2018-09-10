using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents two matrix atoms separated by a single vetical bar.
    /// </summary>
    internal class AugmentedMatrixAtom:Atom
    {
        public AugmentedMatrixAtom(SourceSpan source, Atom leftatom,Atom rightatom, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
            this.LeftBaseAtom = leftatom;
            this.RightBaseAtom = rightatom;
        }

        public Atom LeftBaseAtom { get; private set; }

        public Atom RightBaseAtom { get; private set; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var resultBox = new HorizontalBox();
            var leftmatrix = this.LeftBaseAtom == null ? StrutBox.Empty : this.LeftBaseAtom.CreateBox(environment);
            var heightdiff = leftmatrix.Depth - leftmatrix.Height;
            
            var vbarseparator = DelimiterFactory.CreateBox("vert", leftmatrix.TotalHeight, environment);
            vbarseparator.Shift = -leftmatrix.Height + heightdiff;

            var rightmatrix= this.RightBaseAtom == null ? StrutBox.Empty : this.RightBaseAtom.CreateBox(environment);
            var heightdiff1 = rightmatrix.Depth - rightmatrix.Height;//apparently, its the same as "heightdiff"
            
            leftmatrix.Shift = -(environment.MathFont.GetAxisHeight(environment.Style) - heightdiff) / 3;
            rightmatrix.Shift = -(environment.MathFont.GetAxisHeight(environment.Style) - heightdiff1) / 3;

            resultBox.Add(leftmatrix);
            resultBox.Add(vbarseparator);
            
            resultBox.Add(rightmatrix);

            return resultBox;
        }
    }
}
