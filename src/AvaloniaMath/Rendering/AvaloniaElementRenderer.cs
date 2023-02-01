using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using AvaloniaMath.Fonts;
using WpfMath;
using WpfMath.Boxes;
using WpfMath.Rendering;
using WpfMath.Rendering.Transformations;
using IBrush = WpfMath.Rendering.IBrush;

namespace AvaloniaMath.Rendering;

/// <summary>The renderer that uses Avalonia drawing context.</summary>
internal class AvaloniaElementRenderer : IElementRenderer
{
    private static readonly ISolidColorBrush DefaultForegroundBrush = Brushes.Black;

    private readonly DrawingContext _foregroundContext;
    private readonly double _scale;

    public AvaloniaElementRenderer(DrawingContext foregroundContext, double scale)
    {
        _foregroundContext = foregroundContext;
        _scale = scale;
    }

    public void RenderElement(Box box, double x, double y)
    {
        RenderBackground(box, x, y);
        box.RenderTo(this, x, y);
    }

    public void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground)
    {
        var glyph = info.GetGlyphRun(x, y, _scale);
        _foregroundContext.DrawGlyphRun(foreground.ToAvalonia() ?? DefaultForegroundBrush, glyph);
    }

    public void RenderRectangle(Rectangle rectangle, IBrush? foreground)
    {
        var scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle).ToAvalonia();
        _foregroundContext.FillRectangle(foreground.ToAvalonia() ?? DefaultForegroundBrush, scaledRectangle);
    }

    public void RenderTransformed(Box box, IEnumerable<Transformation> transforms, double x, double y)
    {
        var scaledTransformations = transforms.Select(t => t.Scale(_scale)).ToList();
        foreach (var transformation in scaledTransformations)
        {
            //TODO[#356]         _drawingContext.PushTransform(ToTransform(transformation));
        }

        RenderElement(box, x, y);

        for (var i = 0; i < scaledTransformations.Count; ++i)
        {
            //TODO[#356]         _drawingContext.Pop();
        }
    }

    public void FinishRendering()
    { }

    private void RenderBackground(Box box, double x, double y)
    {
        if (box.Background != null)
        {
            // Fill background of box with color:
            _foregroundContext.FillRectangle(
                box.Background.ToAvalonia(),
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
}
