using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

// Atom specifying graphical style.
internal class StyledAtom : Atom, IRow
{
    public StyledAtom(Atom atom, Brush backgroundColor, Brush foregroundColor)
    {
        this.RowAtom = new RowAtom(atom);
        this.Background = backgroundColor;
        this.Foreground = foregroundColor;
    }

    public DummyAtom PreviousAtom
    {
        get { return this.RowAtom.PreviousAtom; }
        set { this.RowAtom.PreviousAtom = value; }
    }

    // RowAtom to which colors are applied.
    public RowAtom RowAtom
    {
        get;
        private set;
    }

    public Brush Background
    {
        get;
        set;
    }

    public Brush Foreground
    {
        get;
        set;
    }

    public override Box CreateBox(TexEnvironment environment)
    {
        var newEnvironment = environment.Clone();
        if (this.Background != null)
            newEnvironment.Background = this.Background;
        if (this.Foreground != null)
            newEnvironment.Foreground = this.Foreground;
        return this.RowAtom.CreateBox(newEnvironment);
    }

    public override TexAtomType GetLeftType()
    {
        return this.RowAtom.GetLeftType();
    }

    public override TexAtomType GetRightType()
    {
        return this.RowAtom.GetRightType();
    }

    public StyledAtom Clone()
    {
        return new StyledAtom(this.RowAtom, this.Background, this.Foreground);
    }
}
