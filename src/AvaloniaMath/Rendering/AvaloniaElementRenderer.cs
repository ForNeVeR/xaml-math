using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using AvaloniaMath.Fonts;
using XamlMath;
using XamlMath.Boxes;
using XamlMath.Rendering;
using XamlMath.Rendering.Transformations;
using IBrush = XamlMath.Rendering.IBrush;
using Point = XamlMath.Rendering.Point;
using IAvaloniaBrush = Avalonia.Media.IBrush;

namespace AvaloniaMath.Rendering;

/// <summary>The renderer that uses Avalonia drawing context.</summary>
internal sealed class AvaloniaElementRenderer : IElementRenderer
{
    private readonly DrawingContext _foregroundContext;
    private readonly double _scale;
    private readonly IAvaloniaBrush _background;
    private readonly IAvaloniaBrush _foreground;

    public AvaloniaElementRenderer(
        DrawingContext foregroundContext, double scale, IAvaloniaBrush? background, IAvaloniaBrush? foreground)
    {
        _foregroundContext = foregroundContext;
        _scale = scale;
        _background = background ?? Brushes.White;
        _foreground = foreground ?? Brushes.Black;
    }

    public void RenderElement(Box box, double x, double y)
    {
        RenderBackground(box, x, y);
        box.RenderTo(this, x, y);
    }

    public void RenderLine(Point point0, Point point1, IBrush? foreground)
    {
        point0 = GeometryHelper.ScalePoint(_scale, point0);
        point1 = GeometryHelper.ScalePoint(_scale, point1);
        var pen = new Pen(foreground.ToAvalonia() ?? _foreground);
        _foregroundContext.DrawLine(pen, point0.ToAvalonia(), point1.ToAvalonia());
    }

    public void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground)
    {
        var glyph = info.GetGlyphRun(x, y, _scale);
        _foregroundContext.DrawGlyphRun(foreground.ToAvalonia() ?? _foreground, glyph);
    }

    public void RenderRectangle(Rectangle rectangle, IBrush? foreground)
    {
        var scaledRectangle = GeometryHelper.ScaleRectangle(_scale, rectangle).ToAvalonia();
        _foregroundContext.FillRectangle(foreground.ToAvalonia() ?? _foreground, scaledRectangle);
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
                box.Background.ToAvalonia() ?? _background,
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
                var tt = (Transformation.Translate)transformation;
                return new TranslateTransform(tt.X, tt.Y);
            case TransformationKind.Rotate:
                var rt = (Transformation.Rotate)transformation;
                return new RotateTransform(rt.RotationDegrees);
            default:
                throw new NotSupportedException($"Unknown {nameof(Transformation)} kind: {transformation.Kind}");
        }
    }
}
