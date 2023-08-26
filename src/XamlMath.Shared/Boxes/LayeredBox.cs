using System;
using XamlMath.Rendering;

namespace XamlMath.Boxes;

internal sealed class LayeredBox : Box
{
    public override void Add(Box box)
    {
        base.Add(box);

        if (Children.Count is 1)
        {
            Width = box.Width;
            Height = box.Height - box.Shift;
            Depth = box.Depth + box.Shift;
            Italic = box.Italic;
        }
        else
        {
            Width = Math.Max(Width, box.Width);
            Height = Math.Max(Height, box.Height - box.Shift);
            Depth = Math.Max(Depth, box.Depth + box.Shift);
            Italic = Math.Max(Italic, box.Italic);
        }
    }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
        foreach (var box in Children)
        {
            renderer.RenderElement(box, x, y + box.Shift);
        }
    }

    public override int GetLastFontId()
    {
        var fontId = TexFontUtilities.NoFontId;
        foreach (var child in Children)
        {
            fontId = child.GetLastFontId();
            if (fontId == TexFontUtilities.NoFontId)
                break;
        }
        return fontId;
    }
}
