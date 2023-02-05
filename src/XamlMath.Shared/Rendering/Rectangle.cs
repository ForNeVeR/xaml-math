namespace XamlMath.Rendering;

public record struct Point(double X, double Y);

public record struct Size(double Width, double Height);

public readonly record struct Rectangle(Point TopLeft, Size Size)
{
    public Rectangle(double x, double y, double width, double height) : this(new Point(x, y), new Size(width, height))
    {
    }

    public double X => TopLeft.X;
    public double Y => TopLeft.Y;
    public double Width => Size.Width;
    public double Height => Size.Height;
}
