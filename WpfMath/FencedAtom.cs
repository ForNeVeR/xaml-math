using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing base atom surrounded by delimeters.
    internal class FencedAtom : Atom
    {
        private const int delimeterFactor = 901;
        private const double delimeterShortfall = 0.5;

        private static void CentreBox(Box box, double axis)
        {
            var totalHeight = box.Height + box.Depth;
            box.Shift = -(totalHeight / 2 - box.Height) - axis;
        }

        public FencedAtom(Atom baseAtom, WpfMath.SymbolAtom leftDelimeter, WpfMath.SymbolAtom rightDelimeter)
        {
            this.BaseAtom = baseAtom ?? new WpfMath.RowAtom();
            this.LeftDelimeter = leftDelimeter;
            this.RightDelimeter = rightDelimeter;
        }

        public Atom BaseAtom
        {
            get;
            private set;
        }

        private WpfMath.SymbolAtom LeftDelimeter
        {
            get;
            set;
        }

        private WpfMath.SymbolAtom RightDelimeter
        {
            get;
            set;
        }

        public override Box CreateBox(WpfMath.TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var style = environment.Style;

            // Create box for base atom.
            var baseBox = this.BaseAtom.CreateBox(environment);

            // Create result box.
            var resultBox = new WpfMath.HorizontalBox();
            var axis = texFont.GetAxisHeight(style);
            var delta = Math.Max(baseBox.Height - axis, baseBox.Depth + axis);
            var minHeight = Math.Max((delta / 500) * delimeterFactor, 2 * delta - delimeterShortfall);

            // Create and add box for left delimeter.
            if (LeftDelimeter != null)
            {
                var leftDelimeterBox = DelimiterFactory.CreateBox(this.LeftDelimeter.Name, minHeight, environment);
                CentreBox(leftDelimeterBox, axis);
                resultBox.Add(leftDelimeterBox);
            }

            // add glueElement between left delimeter and base Atom, unless base Atom is whitespace.
            if (!(this.BaseAtom is WpfMath.SpaceAtom))
                resultBox.Add(Glue.CreateBox(TexAtomType.Opening, this.BaseAtom.GetLeftType(), environment));

            // add box for base Atom.
            resultBox.Add(baseBox);

            // add glueElement between right delimeter and base Atom, unless base Atom is whitespace.
            if (!(this.BaseAtom is WpfMath.SpaceAtom))
                resultBox.Add(Glue.CreateBox(this.BaseAtom.GetRightType(), TexAtomType.Closing, environment));

            // Create and add box for right delimeter.
            if (this.RightDelimeter != null)
            {
                var rightDelimeterBox = DelimiterFactory.CreateBox(this.RightDelimeter.Name, minHeight, environment);
                CentreBox(rightDelimeterBox, axis);
                resultBox.Add(rightDelimeterBox);
            }

            return resultBox;
        }

        public override TexAtomType GetLeftType()
        {
            return TexAtomType.Opening;
        }

        public override TexAtomType GetRightType()
        {
            return TexAtomType.Closing;
        }
    }
}
