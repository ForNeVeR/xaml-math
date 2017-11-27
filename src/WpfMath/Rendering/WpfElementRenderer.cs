using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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

        public void RenderRectangle(Rect rectangle, Brush foreground)
        {
            var scaledRectangle = ScaleRectangle(rectangle);
            _drawingContext.DrawRectangle(foreground, null, scaledRectangle);
        }

        public void RenderTransformed(Box box, Transform[] transforms, double x, double y)
        {
            var scaledTransforms = transforms.Select(ScaleTransform).ToList();
            foreach (var transform in scaledTransforms)
            {
                _drawingContext.PushTransform(transform);
            }

            RenderElement(box, x, y);

            for (var i = 0; i < scaledTransforms.Count; ++i)
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

        /// <summary>
        /// Genetates the guidelines for WPF render to snap the box boundaries onto the device pixel grid.
        /// </summary>
        private GuidelineSet GenerateGuidelines(Box box, double x, double y) => new GuidelineSet
        {
            GuidelinesX = {_scale * x, _scale * (x + box.TotalWidth)},
            GuidelinesY = {_scale * y, _scale * (y + box.TotalHeight)}
        };

        private Rect ScaleRectangle(Rect rectangle) =>
            new Rect(rectangle.X * _scale, rectangle.Y * _scale, rectangle.Width * _scale, rectangle.Height * _scale);

        private Transform ScaleTransform(Transform transform)
        {
            switch (transform)
            {
                case TranslateTransform tt:
                    return new TranslateTransform(tt.X * _scale, tt.Y * _scale);
                default:
                    return transform;
            }
        }
    }
}
