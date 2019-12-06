using System.Collections.Generic;
using WpfMath.Boxes;
namespace WpfMath.Atoms
{
    // Atom (smallest unit) of TexFormula.
    internal abstract class Atom
    {
        protected Atom(SourceSpan source, IEnumerable<Atom> children, TexAtomType type = TexAtomType.Ordinary)
        {
            this.Source = source;
            this.Type = type;
            this.Children = children;
        }

        protected Atom(SourceSpan source, TexAtomType type = TexAtomType.Ordinary): this(source, new List<Atom>(), type)
        {
        }

        public TexAtomType Type { get; }

        public SourceSpan Source { get; set; }

        public IEnumerable<Atom> Children { get; protected set; }

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
