using System.Windows;

namespace WpfMath.Rendering;

internal static class RectangleExtensions
{
    public static Rect ToWpf(this Rectangle rectangle) =>
        new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
}
