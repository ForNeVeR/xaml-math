using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing other atom that is not rendered.
    internal class PhantomAtom : Atom, WpfMath.IRow
    {
        private bool useWidth;
        private bool useHeight;
        private bool useDepth;

        public PhantomAtom(Atom baseAtom)
            : this(baseAtom, true, true, true)
        {
        }

        public PhantomAtom(Atom baseAtom, bool useWidth, bool useHeight, bool useDepth)
            : base()
        {
            this.RowAtom = baseAtom == null ? new WpfMath.RowAtom() : new WpfMath.RowAtom(baseAtom);
            this.useWidth = useWidth;
            this.useHeight = useHeight;
            this.useDepth = useDepth;
        }

        public DummyAtom PreviousAtom
        {
            get { return this.RowAtom.PreviousAtom; }
            set { this.RowAtom.PreviousAtom = value; }
        }

        public WpfMath.RowAtom RowAtom
        {
            get;
            private set;
        }

        public override Box CreateBox(WpfMath.TexEnvironment environment)
        {
            var resultBox = this.RowAtom.CreateBox(environment);
            return new WpfMath.StrutBox((this.useWidth ? resultBox.Width : 0), (this.useHeight ? resultBox.Height : 0),
                (this.useDepth ? resultBox.Depth : 0), resultBox.Shift);
        }

        public override TexAtomType GetLeftType()
        {
            return this.RowAtom.GetLeftType();
        }

        public override TexAtomType GetRightType()
        {
            return this.RowAtom.GetRightType();
        }
    }
}
