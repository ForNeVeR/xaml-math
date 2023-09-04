namespace XamlMath.Boxes;

// Box representing other box with horizontal rule above it.
internal sealed class OverBar : VerticalBox
{
    public OverBar(TexEnvironment environment, Box box, double kern, double thickness)
        : base()
    {
        this.Add(new StrutBox(0, thickness, 0, 0));
        this.Add(new HorizontalRule(environment, thickness, box.Width, 0));
        this.Add(new StrutBox(0, kern, 0, 0));
        this.Add(box);
    }
}
