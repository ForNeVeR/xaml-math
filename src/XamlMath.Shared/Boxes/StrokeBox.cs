using System;
using XamlMath.Rendering;

namespace XamlMath.Boxes;

public class StrokeBox : Box
{
    private readonly StrokeBoxMode _mode;

    public StrokeBox(StrokeBoxMode mode)
    {
        _mode = mode;
    }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
        if (_mode.HasFlag(StrokeBoxMode.Normal))
            renderer.RenderLine(new Point(x, y + Depth), new Point(x + Width, y - Height), Foreground);

        if (_mode.HasFlag(StrokeBoxMode.Back))
            renderer.RenderLine(new Point(x, y - Height), new Point(x + Width, y + Depth), Foreground);
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

[Flags]
public enum StrokeBoxMode
{
    None = 0,
    Normal = 1,
    Back = 2,
    Both = 3
}
