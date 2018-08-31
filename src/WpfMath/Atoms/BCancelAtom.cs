using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents a box overlayed with a down diagonal strike.
    /// </summary>
    internal class BCancelAtom:Atom
    {
        public BCancelAtom(SourceSpan source, Atom atom, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
            this.BaseAtom = atom;
        }

        public Atom BaseAtom { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;
            var axis = texFont.GetAxisHeight(style);
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);

            var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);

            var ddrule = new DownDiagonalRule(environment, 1.3, baseBox.TotalWidth, baseBox.TotalHeight, 0);
            var resbx = new HorizontalBox();
            resbx.Add(baseBox);
            resbx.Add(new StrutBox(-baseBox.TotalWidth, 0, 0, 0));
            resbx.Add(ddrule);
            return resbx;
        }
    }
}
