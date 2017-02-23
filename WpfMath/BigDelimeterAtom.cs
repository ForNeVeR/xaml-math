using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing big delimeter (e.g. brackets).
    internal class BigDelimeterAtom : Atom
    {
        public BigDelimeterAtom(SymbolAtom delimeterAtom, int size)
        {
            this.DelimeterAtom = delimeterAtom;
            this.Size = size;
        }

        public SymbolAtom DelimeterAtom
        {
            get;
            private set;
        }

        public int Size
        {
            get;
            private set;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            // TODO
            var resultBox = (Box)null; // DelimiterFactory.CreateBox(this.DelimeterAtom, this.Size, environment);
            resultBox.Shift = -(resultBox.Height + resultBox.Depth) / 2 -
                environment.TexFont.GetAxisHeight(environment.Style);
            return resultBox;
        }
    }
}
