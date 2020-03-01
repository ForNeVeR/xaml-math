using WpfMath.Boxes;
using WpfMath.Colors;

namespace WpfMath.Atoms
{
    // Atom specifying graphical style.
    internal class StyledAtom : Atom, IRow
    {
        public StyledAtom(SourceSpan source, Atom atom, ArgbColor? backgroundColor, ArgbColor? foregroundColor)
            : base(source)
        {
            this.RowAtom = new RowAtom(source, atom);
            this.Background = backgroundColor;
            this.Foreground = foregroundColor;
        }

        // RowAtom to which colors are applied.
        public RowAtom RowAtom { get; }

        public ArgbColor? Background { get; }

        public ArgbColor? Foreground { get; }

        public Atom WithPreviousAtom(DummyAtom previousAtom)
        {
            var rowAtom = this.RowAtom.WithPreviousAtom(previousAtom);
            return new StyledAtom(this.Source, rowAtom, this.Background, this.Foreground);
        }

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

        public StyledAtom Clone(
            RowAtom rowAtom = null,
            ArgbColor? background = null,
            ArgbColor? foreground = null)
        {
            return new StyledAtom(
                this.Source,
                rowAtom ?? this.RowAtom,
                background ?? this.Background,
                foreground ?? this.Foreground);
        }
    }
}
