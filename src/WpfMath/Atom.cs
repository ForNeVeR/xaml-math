namespace WpfMath
{
    // Atom (smallest unit) of TexFormula.
    internal abstract class Atom
    {
        protected Atom(SourceSpan source, TexAtomType type = TexAtomType.Ordinary)
        {
            this.Source = source;
            this.Type = type;
        }

        public TexAtomType Type { get; }

        public SourceSpan Source { get; }

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
