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

            box.RenderTo(this, x, y);

            for (var i = 0; i < scaledTransforms.Count; ++i)
            {
                _drawingContext.Pop();
            }
        }

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
