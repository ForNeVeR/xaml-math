using Windows.Foundation;

using XamlMath.Rendering;

namespace UwpMath.Rendering;

internal static class UwpRectangleExtensions
{
    public static Rect ToWin2D(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
}
