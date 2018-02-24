namespace WpfMath
{
    // Atom (smallest unit) of TexFormula.
    internal abstract class Atom
    {
        public Atom()
        {
            this.Type = TexAtomType.Ordinary;
        }

        public TexAtomType Type
        {
            get;
            set;
        }

        public StringSpan Source
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
