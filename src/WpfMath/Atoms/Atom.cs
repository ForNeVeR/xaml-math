using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    // Atom (smallest unit) of TexFormula.
    internal abstract record Atom(TexAtomType Type, SourceSpan? Source)
    {
        // TODO: Merge this constructor with the main one
        public Atom(SourceSpan? source, TexAtomType type = TexAtomType.Ordinary)
            : this(type, source) {}

        public Box CreateBox(TexEnvironment environment)
        {
            var box = this.CreateBoxCore(environment);
            if (box.Source == null)
            {
                box.Source = this.Source;
            }

            return box;
        }

        protected abstract Box CreateBoxCore(TexEnvironment environment);

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
