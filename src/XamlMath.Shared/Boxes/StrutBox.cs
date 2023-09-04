using XamlMath.Rendering;

namespace XamlMath.Boxes;

// Box representing whitespace.
internal sealed class StrutBox : Box
{
    public static StrutBox Empty { get; } = new StrutBox(0, 0, 0, 0);

    public StrutBox(double width, double height, double depth, double shift)
    {
        this.Width = width;
        this.Height = height;
        this.Depth = depth;
        this.Shift = shift;
    }

    public override void RenderTo(IElementRenderer renderer, double x, double y)
    {
    }

    public override int GetLastFontId()
    {
        return TexFontUtilities.NoFontId;
    }
}
