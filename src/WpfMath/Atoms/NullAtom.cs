using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    internal class NullAtom : Atom
    {
        public NullAtom(SourceSpan source = null, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
        }

        public static Box NullBox =>new StrutBox(0, 0, 0, 0); 

        protected override Box CreateBoxCore(TexEnvironment environment)=> new StrutBox(0, 0, 0, 0);

    }
}
