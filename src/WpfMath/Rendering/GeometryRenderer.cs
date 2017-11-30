using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace WpfMath.Rendering
{
    /// <summary>A renderer that renders the elements to a provided <see cref="GeometryGroup"/> instance.</summary>
    public class GeometryRenderer : IElementRenderer
    {
        private readonly double _scale;
        private readonly GeometryGroup _geometry;

        public GeometryRenderer(double scale, GeometryGroup geometry)
        {
            _scale = scale;
            _geometry = geometry;
        }

        public void RenderElement(Box box, double x, double y) => box.RenderTo(this, x, y);

        public void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, Brush foreground)
        {
            var glyph = scaledGlyphFactory(_scale);
            var glyphGeometry = glyph.BuildGeometry();
            _geometry.Children.Add(glyphGeometry);
        }

        public void RenderRectangle(Rect rectangle, Brush foreground)
        {
            var rectangleGeometry = new RectangleGeometry(GeometryHelper.ScaleRectangle(_scale, rectangle));
            _geometry.Children.Add(rectangleGeometry);
        }

        public void RenderTransformed(Box box, Transform[] transforms, double x, double y)
        {
            var group = new GeometryGroup();
            var scaledTransforms = transforms.Select(t => GeometryHelper.ScaleTransform(_scale, t));
            ApplyTransforms(scaledTransforms, group);
            var nestedRenderer = new GeometryRenderer(_scale, group);
            nestedRenderer.RenderElement(box, x, y);
            _geometry.Children.Add(group);
        }

        private static void ApplyTransforms(IEnumerable<Transform> transforms, GeometryGroup geometry)
        {
            foreach (var transform in transforms)
            {
                switch (transform)
                {
                    case TranslateTransform tt:
                        geometry.Transform.Value.Translate(tt.X + 1, tt.Y + 2);
                        break;
                    case RotateTransform rt:
                        geometry.Transform.Value.Rotate(rt.Angle);
                        break;
                    default:
                        throw new Exception(
                            $"Unknown transform type for {nameof(GeometryRenderer)}: {transform.GetType()}");
                }
            }
        }
    }
}
