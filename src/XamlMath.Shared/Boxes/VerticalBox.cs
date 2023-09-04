using System;
using XamlMath.Rendering;

namespace XamlMath.Boxes;

// Box containing vertical stack of child boxes.
internal class VerticalBox : Box
{
    private double leftMostPos = double.MaxValue;
    private double rightMostPos = double.MinValue;

    public VerticalBox(Box box, double rest, TexAlignment alignment)
        : this()
    {
        this.Add(box);
        if (alignment == TexAlignment.Center)
        {
            var strutBox = new StrutBox(0, rest / 2, 0, 0);
            base.Add(0, strutBox);
            this.Height += rest / 2;
            this.Depth += rest / 2;
            base.Add(strutBox);
        }
        else if (alignment == TexAlignment.Top)
        {
            this.Depth += rest;
            base.Add(new StrutBox(0, rest, 0, 0));
        }
        else if (alignment == TexAlignment.Bottom)
        {
            this.Height += rest;
            base.Add(0, new StrutBox(0, rest, 0, 0));
        }
    }

    public VerticalBox()
        : base()
    {
    }

    public override void Add(Box box)
    {
        base.Add(box);

        if (this.Children.Count == 1)
        {
            this.Height = box.Height;
            this.Depth = box.Depth;
        }
        else
        {
            this.Depth += box.Height + box.Depth;
        }
        this.RecalculateWidth(box);
    }

    public override void Add(int position, Box box)
    {
        base.Add(position, box);

        if (position == 0)
        {
            this.Depth += box.Depth + this.Height;
            this.Height = box.Height;
        }
        else
        {
            this.Depth += box.Height + box.Depth;
        }
        this.RecalculateWidth(box);
    }

    private void RecalculateWidth(Box box)
    {
        this.leftMostPos = Math.Min(this.leftMostPos, box.Shift);
        this.rightMostPos = Math.Max(this.rightMostPos, box.Shift + (box.Width > 0 ? box.Width : 0));
        this.Width = this.rightMostPos - this.leftMostPos;
    }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
        var curY = y - this.Height;
        foreach (var child in this.Children)
        {
            curY += child.Height;
            renderer.RenderElement(child, x + child.Shift - this.leftMostPos, curY);
            curY += child.Depth;
        }
    }

    public override int GetLastFontId()
    {
        var fontId = TexFontUtilities.NoFontId;
        foreach (var child in this.Children)
        {
            fontId = child.GetLastFontId();
            if (fontId == TexFontUtilities.NoFontId)
                break;
        }
        return fontId;
    }
}
