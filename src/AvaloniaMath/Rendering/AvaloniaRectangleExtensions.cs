using Avalonia;
using XamlMath.Rendering;

namespace AvaloniaMath.Rendering;

internal static class AvaloniaRectangleExtensions
{
    public static Rect ToAvalonia(this Rectangle rectangle) =>
        new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
}
