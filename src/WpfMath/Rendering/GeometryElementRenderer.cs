using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfMath.Boxes;
using System.Windows.Shapes;
using WpfMath.Rendering.Transformations;

namespace WpfMath.Rendering
{
    /// <summary>A renderer that renders the elements to a provided <see cref="GeometryGroup"/> instance.</summary>
    public class GeometryElementRenderer : IElementRenderer
    {
        private readonly GeometryGroup _geometry;
        private readonly double _scale;

        public GeometryElementRenderer(GeometryGroup geometry, double scale)
        {
            _geometry = geometry;
            _scale = scale;
        }

        public void RenderElement(Box box, double x, double y) => box.RenderTo(this, x, y);

        public void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, Brush foreground)
        {
            var glyph = scaledGlyphFactory(_scale);
            var glyphGeometry = glyph.BuildGeometry();
            _geometry.Children.Add(glyphGeometry);
        }

        public void RenderRectangle(Rect rectangle, Brush foreground, Brush background, double rotationAngle=0)
        {
            var rectangleGeometry = new RectangleGeometry(GeometryHelper.ScaleRectangle(_scale, rectangle))
            {
                Transform = new RotateTransform(rotationAngle)
            };
            _geometry.Children.Add(rectangleGeometry);
        }
        public void RenderRoundedRectangle(Rect rectangle, Brush foreground, Brush background, double radX, double radY)
        {
            Rect scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle);
            Pen rectpen = new Pen(foreground, 0.8);
            var roundrectangleGeometry = new RectangleGeometry(scaledRectangle, radX, radY);
            _geometry.Children.Add(roundrectangleGeometry);
        }

        /// <summary>
        /// Renders an ellipse to the <see cref="GeometryGroup"/>.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="foreground"></param>
        public void RenderEllipse(Rect rectangle, Brush foreground, Brush background)
        {
            var ellipseGeometry = new EllipseGeometry(GeometryHelper.ScaleRectangle(_scale, rectangle));
            _geometry.Children.Add(ellipseGeometry);
        }

        /// <summary>
        /// Renders a rectangle(an image isn't a geometry) to the <see cref="GeometryGroup"/>.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="imagesrc"></param>
        public void RenderImage(Rect rectangle, string imagesrc)
        {
            var rectangleGeometry = new RectangleGeometry(GeometryHelper.ScaleRectangle(_scale, rectangle));

            ImageBrush imgbrs = new ImageBrush();
            Rectangle ty=new Rectangle();
            GeometryDrawing geodrw = new GeometryDrawing(imgbrs, null, rectangleGeometry);
            _geometry.Children.Add(rectangleGeometry);
        }

        public void RenderLine(Point startPoint, Point endPoint,Brush foreground)
        {
            var lineGeometry = new LineGeometry(GeometryHelper.ScalePoint(_scale,startPoint), GeometryHelper.ScalePoint(_scale, endPoint));
            _geometry.Children.Add(lineGeometry);
        }
        public void RenderTransformed(Box box, Transformation[] transforms, double x, double y)
        {
            var group = new GeometryGroup();
            var scaledTransforms = transforms.Select(t => t.Scale(_scale));
            ApplyTransformations(scaledTransforms, group);
            var nestedRenderer = new GeometryElementRenderer(group, _scale);
            nestedRenderer.RenderElement(box, x, y);
            _geometry.Children.Add(group);
        }

        private static void ApplyTransformations(IEnumerable<Transformation> transformations, GeometryGroup geometry)
        {
            foreach (var transformation in transformations)
            {
                switch (transformation.Kind)
                {
                    case TransformationKind.Translate:
                        var tt = (Transformation.Translate) transformation;
                        geometry.Transform.Value.Translate(tt.X, tt.Y);
                        break;
                    case TransformationKind.Rotate:
                        var rt = (Transformation.Rotate) transformation;
                        geometry.Transform.Value.Rotate(rt.RotationDegrees);
                        break;
                    default:
                        throw new NotSupportedException(
                            $"Unknown {nameof(Transformation)} kind: {transformation.Kind}");
                }
            }
        }
    }
}
