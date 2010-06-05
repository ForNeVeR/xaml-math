using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfMath
{
    // Represents mathematical formula that can be rendered.
    public sealed class TexFormula
    {
        public TexFormula(IList<TexFormula> formulaList)
        {
            Debug.Assert(formulaList != null);

            if (formulaList.Count == 1)
                Add(formulaList[0]);
            else
                this.RootAtom = new WpfMath.RowAtom(formulaList);
        }

        public TexFormula(TexFormula formula)
        {
            Debug.Assert(formula != null);

            Add(formula);
        }

        public TexFormula()
        {
        }

        public string TextStyle
        {
            get;
            set;
        }

        internal Atom RootAtom
        {
            get;
            set;
        }

        public WpfMath.TexRenderer GetRenderer(TexStyle style, double scale)
        {
            var environment = new WpfMath.TexEnvironment(style, new DefaultTexFont(scale));
            return new WpfMath.TexRenderer(CreateBox(environment), scale);
        }


        public void Add(TexFormula formula)
        {
            Debug.Assert(formula != null);
            Debug.Assert(formula.RootAtom != null);

            if (formula.RootAtom is WpfMath.RowAtom)
                Add(new WpfMath.RowAtom(formula.RootAtom));
            else
                Add(formula.RootAtom);
        }

        internal void Add(Atom atom)
        {
            Debug.Assert(atom != null);
            if (this.RootAtom == null)
            {
                this.RootAtom = atom;
            }
            else
            {
                if (!(this.RootAtom is WpfMath.RowAtom))
                    this.RootAtom = new WpfMath.RowAtom(RootAtom);
                ((WpfMath.RowAtom)RootAtom).Add(atom);
            }
        }

        public void SetForeground(Brush brush)
        {
            if (this.RootAtom is WpfMath.StyledAtom)
            {
                this.RootAtom = ((WpfMath.StyledAtom)this.RootAtom).Clone();
                ((WpfMath.StyledAtom)this.RootAtom).Foreground = brush;
            }
            else
            {
                this.RootAtom = new WpfMath.StyledAtom(this.RootAtom, null, brush);
            }
        }

        public void SetBackground(Brush brush)
        {
            if (this.RootAtom is WpfMath.StyledAtom)
            {
                this.RootAtom = ((WpfMath.StyledAtom)this.RootAtom).Clone();
                ((WpfMath.StyledAtom)this.RootAtom).Background = brush;
            }
            else
            {
                this.RootAtom = new WpfMath.StyledAtom(this.RootAtom, brush, null);
            }
        }

        internal Box CreateBox(WpfMath.TexEnvironment environment)
        {
            if (this.RootAtom == null)
                return WpfMath.StrutBox.Empty;
            else
                return this.RootAtom.CreateBox(environment);
        }
    }
}
