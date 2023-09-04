using System;
using XamlMath.Rendering;

namespace XamlMath.Boxes;

/// <summary>Box containing horizontal stack of child boxes.</summary>
internal sealed class HorizontalBox : Box
{
    private double childBoxesTotalWidth = 0.0;

    public HorizontalBox(Box box, double width, TexAlignment alignment)
        : this()
    {
        var extraWidth = width - box.Width;
        if (alignment == TexAlignment.Center)
        {
            var strutBox = new StrutBox(extraWidth / 2, 0, 0, 0);
            this.Add(strutBox);
            this.Add(box);
            this.Add(strutBox);
        }
        else if (alignment == TexAlignment.Left)
        {
            this.Add(box);
            this.Add(new StrutBox(extraWidth, 0, 0, 0));
        }
        else if (alignment == TexAlignment.Right)
        {
            this.Add(new StrutBox(extraWidth, 0, 0, 0));
            this.Add(box);
        }
    }

    public HorizontalBox(Box box)
        : this()
    {
        this.Add(box);
    }

    public HorizontalBox(IBrush? foreground, IBrush? background)
        : base(foreground, background)
    {
    }

    public HorizontalBox()
        : base()
    {
    }

    public sealed override void Add(Box box)
    {
        base.Add(box);

        childBoxesTotalWidth += box.Width;
        Width = Math.Max(Width, childBoxesTotalWidth);
        Height = Math.Max(Height, box.Height - box.Shift);
        Depth = Math.Max(Depth, box.Depth + box.Shift);
        Italic = Math.Max(Italic, box.Italic);
    }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
        var curX = x;
        foreach (var box in this.Children)
        {
            renderer.RenderElement(box, curX, y + box.Shift);
            curX += box.Width;
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
