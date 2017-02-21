using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing other atom with delimeter and script atoms over or under it.
    internal class OverUnderDelimiter : Atom
    {
        private static double GetMaxWidth(Box baseBox, Box delimeterBox, Box scriptBox)
        {
            var maxWidth = Math.Max(baseBox.Width, delimeterBox.Height + delimeterBox.Depth);
            if (scriptBox != null)
                maxWidth = Math.Max(maxWidth, scriptBox.Width);
            return maxWidth;
        }

        public OverUnderDelimiter(Atom baseAtom, Atom script, SymbolAtom symbol, TexUnit kernUnit, double kern, bool over)
        {
            this.Type = TexAtomType.Inner;
            this.BaseAtom = baseAtom;
            this.Script = script;
            this.Symbol = symbol;
            this.Kern = new SpaceAtom(kernUnit, 0, kern, 0);
            this.Over = over;
        }

        public Atom BaseAtom
        {
            get;
            private set;
        }

        private Atom Script
        {
            get;
            set;
        }

        private SymbolAtom Symbol
        {
            get;
            set;
        }

        // Kern between delimeter symbol and script.
        private SpaceAtom Kern
        {
            get;
            set;
        }

        // True to place delimeter symbol Over base; false to place delimeter symbol under base.
        public bool Over
        {
            get;
            set;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            // Create boxes for base, delimeter, and script atoms.
            var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);
            var delimeterBox = DelimiterFactory.CreateBox(this.Symbol.Name, baseBox.Width, environment);
            var scriptBox = this.Script == null ? null : this.Script.CreateBox(this.Over ?
                environment.GetSuperscriptStyle() : environment.GetSubscriptStyle());

            // Create centered horizontal box if any box is smaller than maximum width.
            var maxWidth = GetMaxWidth(baseBox, delimeterBox, scriptBox);
            if (Math.Abs(maxWidth - baseBox.Width) > TexUtilities.FloatPrecision)
                baseBox = new HorizontalBox(baseBox, maxWidth, TexAlignment.Center);
            if (Math.Abs(maxWidth - delimeterBox.Height - delimeterBox.Depth) > TexUtilities.FloatPrecision)
                delimeterBox = new VerticalBox(delimeterBox, maxWidth, TexAlignment.Center);
            if (scriptBox != null && Math.Abs(maxWidth - scriptBox.Width) > TexUtilities.FloatPrecision)
                scriptBox = new HorizontalBox(scriptBox, maxWidth, TexAlignment.Center);

            return new OverUnderBox(baseBox, delimeterBox, scriptBox, this.Kern.CreateBox(environment).Height, this.Over);
        }
    }
}
