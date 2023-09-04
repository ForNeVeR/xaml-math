namespace XamlMath;

/// <summary>Specifies font metrics for single character.</summary>
public class TeXFontMetrics
{
    public TeXFontMetrics(double width, double height, double depth, double italicWidth, double scale)
    {
        this.Width = width * scale;
        this.Height = height * scale;
        this.Depth = depth * scale;
        this.Italic = italicWidth * scale;
    }

    public double Width
    {
        get;
        set;
    }

    public double Height
    {
        get;
        set;
    }

    public double Depth
    {
        get;
        set;
    }

    public double Italic
    {
        get;
        set;
    }
}
