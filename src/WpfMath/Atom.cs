namespace WpfMath
{
    // Atom (smallest unit) of TexFormula.
    internal abstract class Atom
    {
        protected Atom(TexAtomType type)
        {
            this.Type = type;
        }

        protected Atom() : this(TexAtomType.Ordinary)
        {
        }

        public SourceSpan Source
        {
            get;
            set;
        }

        public Box CreateBox(TexEnvironment environment)
        {
            var box = CreateBoxCore(environment);
            if (box.Source == null)
                box.Source = Source;
            return box;
        }

        public abstract Atom Copy();

        protected virtual Atom CopyTo(Atom atom)
        {
            atom.Type = Type;
            atom.Source = Source;
            return atom;
        }

        protected abstract Box CreateBoxCore(TexEnvironment environment);
        public TexAtomType Type { get; }

        public abstract Box CreateBox(TexEnvironment environment);

        // Gets type of leftmost child item.
        public virtual TexAtomType GetLeftType()
        {
            return this.Type;
        }

        // Gets type of leftmost child item.
        public virtual TexAtomType GetRightType()
        {
            return this.Type;
        }
    }
}
