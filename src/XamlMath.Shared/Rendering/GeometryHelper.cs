namespace XamlMath.Rendering
{
    internal static class GeometryHelper
    {
        public static Rectangle ScaleRectangle(double scale, Rectangle rectangle) =>
            new(rectangle.X * scale, rectangle.Y * scale, rectangle.Width * scale, rectangle.Height * scale);
    }
}
