namespace WpfMath
{
    // Atom representing other atom vertically centered with respect to axis.
    internal class VerticalCenteredAtom : Atom
    {
        public VerticalCenteredAtom(Atom atom)
        {
            this.Atom = atom;
        }

        public Atom Atom
        {
            get;
            private set;
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var box = this.Atom.CreateBox(environment);

            // Centre box relative to horizontal axis.
            var totalHeight = box.Height + box.Depth;
            var axis = environment.MathFont.GetAxisHeight(environment.Style);
            box.Shift = -(totalHeight / 2) - axis;

            return new HorizontalBox(box);
        }
    }
}
