using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing other atom with custom left and right types.
    internal class TypedAtom : Atom
    {
        public TypedAtom(Atom atom, TexAtomType leftType, TexAtomType rightType)
        {
            this.Atom = atom;
            this.LeftType = leftType;
            this.RightType = rightType;
        }

        public Atom Atom
        {
            get;
            private set;
        }

        public TexAtomType LeftType
        {
            get;
            private set;
        }

        public TexAtomType RightType
        {
            get;
            private set;
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            return this.Atom.CreateBox(environment);
        }

        public override TexAtomType GetLeftType()
        {
            return this.LeftType;
        }

        public override TexAtomType GetRightType()
        {
            return this.RightType;
        }
    }
}
