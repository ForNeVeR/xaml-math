using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WpfMath.Boxes;
using WpfMath.Rendering.Transformations;

namespace WpfMath.Rendering
{
    /// <summary>The renderer that uses WPF drawing context.</summary>
    /// <remarks>
    /// The WPF renderer draws all the image elements into two layers: the background and the foreground. Background
    /// gets drawn immediately into the target context. Foreground gets drawn onto a separate <see cref="DrawingGroup"/>
    /// that gets merged into the target context during the <see cref="WpfElementRenderer.FinishRendering"/> call.
    /// </remarks>
    internal class WpfElementRenderer : IElementRenderer
    {
        private readonly DrawingContext _targetContext;
        private readonly double _scale;

        private readonly DrawingGroup _foregroundGroup = new DrawingGroup();
        private readonly DrawingContext _foregroundContext;

        public WpfElementRenderer(DrawingContext targetContext, double scale)
        {
            _targetContext = targetContext;
            _scale = scale;

            _foregroundContext = _foregroundGroup.Append();
        }

        public void RenderElement(Box box, double x, double y)
        {
            var guidelines = GenerateGuidelines(box, x, y);
            _foregroundContext.PushGuidelineSet(guidelines);
            _targetContext.PushGuidelineSet(guidelines);

            RenderBackground(box, x, y);
            box.RenderTo(this, x, y);

            _targetContext.Pop();
            _foregroundContext.Pop();
        }

        public void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, Brush foreground)
        {
            var glyphRun = scaledGlyphFactory(_scale);
            _foregroundContext.DrawGlyphRun(foreground, glyphRun);
        }

        public void RenderRectangle(Rect rectangle, Brush foreground)
        {
            var scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle);
            _foregroundContext.DrawRectangle(foreground, null, scaledRectangle);
        }

        public void RenderTransformed(Box box, Transformation[] transforms, double x, double y)
        {
            var scaledTransformations = transforms.Select(t => t.Scale(_scale)).ToList();
            foreach (var transformation in scaledTransformations)
            {
                _foregroundContext.PushTransform(ToTransform(transformation));
            }

            RenderElement(box, x, y);

            for (var i = 0; i < scaledTransformations.Count; ++i)
            {
                _foregroundContext.Pop();
            }
        }

        public void FinishRendering()
        {
            _foregroundContext.Close();
            _targetContext.DrawDrawing(_foregroundGroup);
        }

        private void RenderBackground(Box box, double x, double y)
        {
            if (box.Background != null)
            {
                _targetContext.DrawRectangle(
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
