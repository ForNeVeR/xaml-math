using System.Collections.Generic;
using XamlMath.Rendering;

namespace XamlMath.Boxes;

/// <summary>Box representing single character.</summary>
internal sealed class CharBox : Box
{
    public CharBox(TexEnvironment environment, CharInfo charInfo)
        : base(environment)
    {
        this.Character = charInfo;
        this.Width = charInfo.Metrics.Width;
        this.Height = charInfo.Metrics.Height;
        this.Depth = charInfo.Metrics.Depth;
        this.Italic = charInfo.Metrics.Italic;
    }

    public CharInfo Character { get; }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
        renderer.RenderCharacter(Character, x, y, this.Foreground);
    }

    public override int GetLastFontId()
    {
        return this.Character.FontId;
    }
}
