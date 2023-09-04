using XamlMath.Rendering;

namespace XamlMath.Boxes;

/// <summary>Box representing horizontal line.</summary>
internal sealed class HorizontalRule : Box
{
    public HorizontalRule(TexEnvironment environment, double thickness, double width, double shift)
    {
        this.Width = width;
        this.Height = thickness;
        this.Shift = shift;
        this.Foreground = environment.Foreground;
        this.Background = environment.Background;	//Not strictly necessary
    }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
        var rectangle = new Rectangle(x, y - this.Height, this.Width, this.Height);
        renderer.RenderRectangle(rectangle, Foreground);
    }

    public override int GetLastFontId()
    {
        return TexFontUtilities.NoFontId;
    }
}
