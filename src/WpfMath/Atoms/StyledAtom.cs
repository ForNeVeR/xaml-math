using System.Windows.Media;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    // Atom specifying graphical style.
    internal record StyledAtom : Atom, IRow
    {
        public StyledAtom(SourceSpan? source, Atom? atom, Brush? backgroundColor, Brush? foregroundColor)
            : base(source)
        {
            this.RowAtom = new RowAtom(source, atom);
            this.Background = backgroundColor;
            this.Foreground = foregroundColor;
        }

        // RowAtom to which colors are applied.
        public RowAtom RowAtom { get; init; }

        public Brush? Background { get; init; }

        public Brush? Foreground { get; init; }

        public Atom WithPreviousAtom(DummyAtom? previousAtom) =>
            this with { RowAtom = (RowAtom) RowAtom.WithPreviousAtom(previousAtom) };

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var newEnvironment = environment.Clone();
            if (this.Foreground != null)
                newEnvironment.Foreground = this.Foreground;
            var childBox = this.RowAtom.CreateBox(newEnvironment);
            if (Background != null)
                childBox.Background = Background;
            return childBox;
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
