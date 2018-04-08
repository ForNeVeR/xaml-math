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
