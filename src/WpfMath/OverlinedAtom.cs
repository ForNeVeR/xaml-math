namespace WpfMath
{
    // Atom representing other atom with horizontal rule above it.
    internal class OverlinedAtom : Atom
    {
        public OverlinedAtom(Atom baseAtom)
            : base()
        {
            this.Type = TexAtomType.Ordinary;
            this.BaseAtom = baseAtom;
        }

        public Atom BaseAtom
        {
            get;
            private set;
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            // Create box for base atom, in cramped style.
            var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment.GetCrampedStyle());

            // Create result box.
            var defaultLineThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);
            var resultBox = new OverBar(environment, baseBox, 3 * defaultLineThickness, defaultLineThickness);

            // Adjust height and depth of result box.
            resultBox.Height = baseBox.Height + 5 * defaultLineThickness;
            resultBox.Depth = baseBox.Depth;

            return resultBox;
        }
    }
}
