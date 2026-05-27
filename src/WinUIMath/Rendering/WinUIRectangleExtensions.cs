using Windows.Foundation;

using XamlMath.Rendering;

namespace WinUIMath.Rendering;

internal static class WinUIRectangleExtensions
{
    public static Rect ToWin2D(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
}
