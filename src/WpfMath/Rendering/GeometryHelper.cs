using System.Windows;
using System.Windows.Media;

namespace WpfMath.Rendering
{
    internal static class GeometryHelper
    {
        public static Rect ScaleRectangle(double scale, Rect rectangle) =>
            new Rect(rectangle.X * scale, rectangle.Y * scale, rectangle.Width * scale, rectangle.Height * scale);
              
        public static Point ScalePoint(double scale, Point pt) => new Point(pt.X * scale, pt.Y * scale);

    }
}
