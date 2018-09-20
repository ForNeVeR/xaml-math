using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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
            //Pen glyphPen = new Pen(foreground, 1)
            //{
            //    //DashStyle = new DashStyle(new double[] { 2, 2 }, 0.25),
            //    StartLineCap=PenLineCap.Round,
            //    LineJoin=PenLineJoin.Bevel,
            //};
            //Using the DrawGeometry might make glyphs less blurry but a little too deep/bold
            //_drawingContext.DrawGeometry(foreground, glyphPen, glyphRun.BuildGeometry());

            _drawingContext.DrawGlyphRun(foreground, glyphRun);
        }

        public void RenderRectangle(Rect rectangle, Brush foreground, Brush background)
        {
            var scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle);
            var rectGeom = new RectangleGeometry(scaledRectangle);
            Pen rectpen = new Pen(foreground, 1.2)
            {

            };
            if (background == null)
            {
                _drawingContext.DrawRectangle(foreground, null, scaledRectangle);
            }
            else
            {
                _drawingContext.DrawGeometry(background, rectpen, rectGeom);
            }
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
