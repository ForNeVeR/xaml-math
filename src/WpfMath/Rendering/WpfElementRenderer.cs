using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfMath.Boxes;
using WpfMath.Rendering.Transformations;

namespace WpfMath.Rendering
{
    /// <summary>The renderer that uses WPF drawing context.</summary>
    internal class WpfElementRenderer : IElementRenderer
    {
        private readonly DrawingContext _drawingContext;
        private readonly double _scale;

        public WpfElementRenderer(DrawingContext drawingContext, double scale)
        {
            _drawingContext = drawingContext;
            _scale = scale;
        }

        public void RenderElement(Box box, double x, double y)
        {
            var guidelines = GenerateGuidelines(box, x, y);
            _drawingContext.PushGuidelineSet(guidelines);

            RenderBackground(box, x, y);
            box.RenderTo(this, x, y);

            _drawingContext.Pop();
        }

        public void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, Brush foreground)
        {
            var glyphRun = scaledGlyphFactory(_scale);
            _drawingContext.DrawGlyphRun(foreground, glyphRun);
        }

        public void RenderRectangle(Rect rectangle, Brush foreground, Brush background, double rotationAngle = 0)
        {
            var scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle);
            var rectGeom = new RectangleGeometry(scaledRectangle);
            Pen rectpen = new Pen(foreground, 0.8);
            if (background == null)
            {
                _drawingContext.DrawRectangle(foreground, null, scaledRectangle);
            }
            else
            {
                _drawingContext.DrawGeometry(background, rectpen, rectGeom);
            }
        }
        
        public void RenderRoundedRectangle(Rect rectangle,Brush foreground,Brush background,double radX,double radY)
        {
            Rect scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle);
            Pen rectpen = new Pen(foreground, 1.2);

            _drawingContext.DrawRoundedRectangle(background, rectpen, scaledRectangle, radX, radY);
        }

        public void RenderEllipse(Rect rectangle, Brush foreground, Brush background)
        {
            var scaledEllipse = GeometryHelper.ScaleRectangle(_scale, rectangle);
            var cPt = new Point(scaledEllipse.X+ (scaledEllipse.Width / 2), scaledEllipse.Y+ (scaledEllipse.Height / 2));
            Pen ellpen = new Pen(foreground, 1);
            var radius_X = scaledEllipse.Width / 2;
            var radius_Y = scaledEllipse.Height / 2;
            _drawingContext.DrawEllipse(background, ellpen, cPt, radius_X,radius_Y);
        }

        /// <summary>
        /// Renders an image to the <see cref="GeometryGroup"/>.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="imagesrc"></param>
        public void RenderImage(Rect rectangle, string imagesrc)
        {
            var scaledImg = GeometryHelper.ScaleRectangle(_scale, rectangle);
            
            ImageSource  img= new BitmapImage(new Uri(imagesrc));
            _drawingContext.DrawImage(img, scaledImg);
        }

        public void RenderLine(Point startPt, Point endPt, Brush foreground)
        {
            var scaledSP = GeometryHelper.ScalePoint(_scale, startPt);
            var scaledEP = GeometryHelper.ScalePoint(_scale, endPt);
            Pen linePen = new Pen(foreground, 1);
            _drawingContext.DrawLine(linePen,scaledSP,scaledEP);
        }

        public void RenderTransformed(Box box, Transformation[] transforms, double x, double y)
        {
            var scaledTransformations = transforms.Select(t => t.Scale(_scale)).ToList();
            foreach (var transformation in scaledTransformations)
            {
                _drawingContext.PushTransform(ToTransform(transformation));
            }

            RenderElement(box, x, y);

            for (var i = 0; i < scaledTransformations.Count; ++i)
            {
                _drawingContext.Pop();
            }
        }

        private void RenderBackground(Box box, double x, double y)
        {
            if (box.Background != null)
            {
                // Fill background of box with color:
                _drawingContext.DrawRectangle(
                    box.Background,
                    null,
                    new Rect(_scale * x, _scale * (y - box.Height),
                        _scale * box.TotalWidth,
                        _scale * box.TotalHeight));
            }
        }

        private static Transform ToTransform(Transformation transformation)
        {
            switch (transformation.Kind)
            {
                case TransformationKind.Translate:
                    var tt = (Transformation.Translate) transformation;
                    return new TranslateTransform(tt.X, tt.Y);
                case TransformationKind.Rotate:
                    var rt = (Transformation.Rotate) transformation;
                    return new RotateTransform(rt.RotationDegrees);
                default:
                    throw new NotSupportedException($"Unknown {nameof(Transformation)} kind: {transformation.Kind}");
            }
        }

        /// <summary>
        /// Generates the guidelines for WPF render to snap the box boundaries onto the device pixel grid.
        /// </summary>
        private GuidelineSet GenerateGuidelines(Box box, double x, double y) => new GuidelineSet
        {
            GuidelinesX = {_scale * x, _scale * (x + box.TotalWidth)},
            GuidelinesY = {_scale * y, _scale * (y + box.TotalHeight)}
        };
    }
}
