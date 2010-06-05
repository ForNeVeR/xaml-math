using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

// Represents mathematical formula that can be rendered.
public sealed class TexFormula
{
    public TexFormula(IList<TexFormula> formulaList)
    {
        Debug.Assert(formulaList != null);

        if (formulaList.Count == 1)
            Add(formulaList[0]);
        else
            this.RootAtom = new RowAtom(formulaList);
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

    public TexRenderer GetRenderer(TexStyle style, double scale)
    {
        var environment = new TexEnvironment(style, new DefaultTexFont(scale));
        return new TexRenderer(CreateBox(environment), scale);
    }

    public void Add(TexFormula formula)
    {
        Debug.Assert(formula != null);
        Debug.Assert(formula.RootAtom != null);

        if (formula.RootAtom is RowAtom)
            Add(new RowAtom(formula.RootAtom));
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
            if (!(this.RootAtom is RowAtom))
                this.RootAtom = new RowAtom(RootAtom);
            ((RowAtom)RootAtom).Add(atom);
        }
    }

    public void SetForeground(Brush brush)
    {
        if (this.RootAtom is StyledAtom)
        {
            this.RootAtom = ((StyledAtom)this.RootAtom).Clone();
            ((StyledAtom)this.RootAtom).Foreground = brush;
        }
        else
        {
            this.RootAtom = new StyledAtom(this.RootAtom, null, brush);
        }
    }

    public void SetBackground(Brush brush)
    {
        if (this.RootAtom is StyledAtom)
        {
            this.RootAtom = ((StyledAtom)this.RootAtom).Clone();
            ((StyledAtom)this.RootAtom).Background = brush;
        }
        else
        {
            this.RootAtom = new StyledAtom(this.RootAtom, brush, null);
        }
    }

    internal Box CreateBox(TexEnvironment environment)
    {
        if (this.RootAtom == null)
            return StrutBox.Empty;
        else
            return this.RootAtom.CreateBox(environment);
    }
}
