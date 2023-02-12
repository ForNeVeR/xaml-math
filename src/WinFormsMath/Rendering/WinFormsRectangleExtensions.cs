namespace WinFormsMath.Rendering;

internal static class WinFormsRectangleExtensions
{
    public static RectangleF ToWinForms(this XamlMath.Rendering.Rectangle rectangle) =>
        new((float)rectangle.X, (float)rectangle.Y, (float)rectangle.Width, (float)rectangle.Height);
}
