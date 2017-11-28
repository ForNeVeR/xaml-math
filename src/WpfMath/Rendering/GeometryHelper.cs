using System.Windows;
using System.Windows.Media;

namespace WpfMath.Rendering
{
    internal static class GeometryHelper
    {
        public static Rect ScaleRectangle(double scale, Rect rectangle) =>
            new Rect(rectangle.X * scale, rectangle.Y * scale, rectangle.Width * scale, rectangle.Height * scale);

        public static Transform ScaleTransform(double scale, Transform transform)
        {
            switch (transform)
            {
                case TranslateTransform tt:
                    return new TranslateTransform(tt.X * scale, tt.Y * scale);
                default:
                    return transform;
            }
        }
    }
}
