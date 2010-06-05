using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

public class TexRenderer
{
    internal TexRenderer(Box box, double scale)
    {
        this.Box = box;
        this.Scale = scale;
    }

    public Box Box
    {
        get;
        set;
    }

    public double Scale
    {
        get;
        private set;
    }

    public Size RenderSize
    {
        get
        {
            return new Size(this.Box.Width * this.Scale, this.Box.TotalHeight * this.Scale);
        }
    }

    public double Baseline
    {
        get
        {
            return this.Box.Height / this.Box.TotalHeight * this.Scale;
        }
    }

    public void Render(DrawingContext drawingContext, double x, double y)
    {
        this.Box.Draw(drawingContext, this.Scale, x / this.Scale, y / this.Scale + this.Box.Height);
    }
}
