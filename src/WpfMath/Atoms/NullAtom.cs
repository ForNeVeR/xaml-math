using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    internal class NullAtom : Atom
    {
        public NullAtom(SourceSpan source = null, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
        }

        protected override Box CreateBoxCore(TexEnvironment environment) => new StrutBox(0, 0, 0, 0);
    }
}
