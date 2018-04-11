using System.Windows.Media;

namespace WpfMath
{
    // Atom specifying graphical style.
    internal class StyledAtom : Atom, IRow
    {
        public StyledAtom(Atom atom, Brush backgroundColor, Brush foregroundColor)
        {
            this.RowAtom = new RowAtom(atom);
            this.Background = backgroundColor;
            this.Foreground = foregroundColor;
        }

        // RowAtom to which colors are applied.
        public RowAtom RowAtom { get; }

        public Brush Background { get; }

        public Brush Foreground { get; }

        public Atom WithPreviousAtom(DummyAtom previousAtom)
        {
            var rowAtom = this.RowAtom.WithPreviousAtom(previousAtom);
            return new StyledAtom(rowAtom, this.Background, this.Foreground);
        }

        public override Atom Copy()
        {
            return CopyTo(new StyledAtom(RowAtom?.Copy(), Background, Foreground) { PreviousAtom = (DummyAtom)PreviousAtom?.Copy() });
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var newEnvironment = environment.Clone();
            if (this.Background != null)
                newEnvironment.Background = this.Background;
            if (this.Foreground != null)
                newEnvironment.Foreground = this.Foreground;
            return this.RowAtom.CreateBox(newEnvironment);
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
            Brush background = null,
            Brush foreground = null)
        {
            return new StyledAtom(
                rowAtom ?? this.RowAtom,
                background ?? this.Background,
                foreground ?? this.Foreground);
        }
    }
}
