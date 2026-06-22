using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using UwpMath.Fonts;

using Windows.UI;
using Windows.UI.Xaml;

using XamlMath;
using XamlMath.Boxes;
using XamlMath.Rendering;
using XamlMath.Rendering.Transformations;

using UwpRect = Windows.Foundation.Rect;

namespace UwpMath.Rendering;

internal class UwpElementRenderer : IElementRenderer
{
    public UwpElementRenderer(CanvasDrawingSession drawingSession)
    {
        _drawingSession = drawingSession;
        _brushFactory = UwpBrushFactory.Instance;
    }

    private readonly CanvasDrawingSession _drawingSession;
    private readonly UwpBrushFactory _brushFactory;

    public double Scale { get; set; }

    public void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground)
    {
        ICanvasBrush? win2DBrush = GetWin2DBrush(foreground);
        if (win2DBrush is null)
            return;

        Win2DGlyphRun glyphRun = info.GetGlyphRun(x, y, Scale);
        _drawingSession.DrawGlyphRun(glyphRun, win2DBrush);
    }

    public void RenderElement(Box box, double x, double y)
    {
        if (box.Background is not null)
        {
            _drawingSession.FillRectangle(new UwpRect(Scale * x, Scale * (y - box.Height), Scale * box.TotalWidth, Scale * box.TotalHeight), GetWin2DBrush(box.Background));
        }
        box.RenderTo(this, x, y);
    }

    public void RenderLine(Point point0, Point point1, IBrush? foreground)
    {
        point0 = GeometryHelper.ScalePoint(Scale, point0);
        point1 = GeometryHelper.ScalePoint(Scale, point1);
        _drawingSession.DrawLine(point0.ToWin2D(), point1.ToWin2D(), GetWin2DBrush(foreground), 1);
    }

    public void RenderRectangle(Rectangle rectangle, IBrush? foreground)
    {
        Rectangle scaledRectangle = GeometryHelper.ScaleRectangle(Scale, rectangle);
        _drawingSession.FillRectangle(scaledRectangle.ToWin2D(), GetWin2DBrush(foreground));
    }

    public void RenderTransformed(Box box, IEnumerable<Transformation> transforms, double x, double y)
    {
        List<Transformation> scaledTransformations = [.. transforms.Select(t => t.Scale(Scale))];
        Matrix3x2 transform = Matrix3x2.Identity;
        foreach (Transformation t in scaledTransformations)
        {
            transform = ToTransform(t) * transform;
        }
        Matrix3x2 originalTransform = _drawingSession.Transform;
        _drawingSession.Transform = transform;
        RenderElement(box, x, y);
        _drawingSession.Transform = originalTransform;
    }

    public void FinishRendering()
    {
        _drawingSession.Dispose();
    }

    private ICanvasBrush? GetWin2DBrush(IBrush? brush)
    {
        brush ??= Application.Current.RequestedTheme switch
        {
            ApplicationTheme.Light => _brushFactory.FromColor(Colors.Black.ToPlatform()),
            ApplicationTheme.Dark => _brushFactory.FromColor(Colors.White.ToPlatform()),
            _ => _brushFactory.FromColor(Color.FromArgb(byte.MaxValue, 128, 128, 128).ToPlatform())
        };
        UwpBrush UwpBrush = (UwpBrush)brush;
        return UwpBrush.Value.ToWin2DBrush(_drawingSession);
    }

    private static Matrix3x2 ToTransform(Transformation transformation)
    {
        switch (transformation.Kind)
        {
            case TransformationKind.Translate:
                var tt = (Transformation.Translate)transformation;
                return Matrix3x2.CreateTranslation((float)tt.X, (float)tt.Y);
            case TransformationKind.Rotate:
                var rt = (Transformation.Rotate)transformation;
                return Matrix3x2.CreateRotation((float)rt.RotationDegrees * MathF.PI / 180);
            default:
                throw new NotSupportedException($"Unknown {nameof(Transformation)} kind: {transformation.Kind}");
        }
    }
}
