using System;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using AvaloniaMath.Rendering;
using WpfMath.Boxes;
using WpfMath.Rendering.Transformations;

namespace WpfMath.Rendering
{
    /// <summary>The renderer that uses Avalonia drawing context.</summary>
    internal class AvaloniaElementRenderer : IElementRenderer
    {
        private readonly DrawingContext _drawingContext;
        private readonly double _scale;

        public AvaloniaElementRenderer(DrawingContext drawingContext, double scale)
        {
            _drawingContext = drawingContext;
            _scale = scale;
        }

        public void RenderElement(Box box, double x, double y)
        {
         //   var guidelines = GenerateGuidelines(box, x, y);
         //   _drawingContext.PushGuidelineSet(guidelines);

            RenderBackground(box, x, y);
            box.RenderTo(this, x, y);

          //  _drawingContext.Pop();
        }

        public void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, IBrush foreground)
        {
            var glyphRun = scaledGlyphFactory(_scale);
            //_drawingContext.DrawGlyphRun(foreground, glyphRun);

            if (glyphRun.Font.FontSize >= 0.0)
            {
                var tf = glyphRun.Font;

                var ft = new FormattedText
                {
                    Typeface = tf,
                    Text = glyphRun.Character.ToString(),
                    TextAlignment = TextAlignment.Left,
                    Wrapping = TextWrapping.NoWrap
                };

                // TODO fix baseline text display
                var origin = glyphRun.Position.WithY(glyphRun.Position.Y-tf.FontSize*0.75);

                _drawingContext.DrawText(foreground, origin, ft);

            }
        }

        public void RenderRectangle(Rect rectangle, IBrush foreground)
        {
            var scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle);
            _drawingContext.FillRectangle(foreground, scaledRectangle);
        }

        public void RenderTransformed(Box box, Transformation[] transforms, double x, double y)
        {
            var scaledTransformations = transforms.Select(t => t.Scale(_scale)).ToList();
            foreach (var transformation in scaledTransformations)
            {
       //TODO         _drawingContext.PushTransform(ToTransform(transformation));
            }

            RenderElement(box, x, y);

            for (var i = 0; i < scaledTransformations.Count; ++i)
            {
       //TODO         _drawingContext.Pop();
            }
        }

        public void FinishRendering()
        { }

        private void RenderBackground(Box box, double x, double y)
        {
            if (box.Background != null)
            {
                // Fill background of box with color:
                _drawingContext.FillRectangle(
                    ((AvaloniaBrush)box.Background).Get(),
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
    /*    private GuidelineSet GenerateGuidelines(Box box, double x, double y) => new GuidelineSet
        {
            GuidelinesX = {_scale * x, _scale * (x + box.TotalWidth)},
            GuidelinesY = {_scale * y, _scale * (y + box.TotalHeight)}
        };
        */
    }
}
