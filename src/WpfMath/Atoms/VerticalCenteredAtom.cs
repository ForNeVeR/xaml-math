using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    // Atom representing other atom vertically centered with respect to axis.
    internal record VerticalCenteredAtom : Atom
    {
        public VerticalCenteredAtom(SourceSpan? source, Atom? atom)
            : base(source)
        {
            this.Atom = atom;
        }

        public Atom? Atom { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var box = this.Atom!.CreateBox(environment); // Nullable TODO: This probably needs null checking

            // Centre box relative to horizontal axis.
            var totalHeight = box.Height + box.Depth;
            var axis = environment.MathFont.GetAxisHeight(environment.Style);
            box.Shift = -(totalHeight / 2) - axis;

            return new HorizontalBox(box);
        }
    }
}
