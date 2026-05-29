using System;
using System.Collections.Generic;
using System.Linq;

using Windows.UI.Xaml.Media;

using XamlMath;
using XamlMath.Boxes;
using XamlMath.Rendering;
using XamlMath.Rendering.Transformations;

namespace UwpMath.Rendering;

internal class GeometryElementRenderer : IElementRenderer
{
    public GeometryElementRenderer(GeometryGroup geometry, double scale)
    {
        _geometry = geometry;
        _scale = scale;
    }

    private readonly GeometryGroup _geometry;
    private readonly double _scale;

    public void RenderElement(Box box, double x, double y) => box.RenderTo(this, x, y);
    public void RenderLine(Point point0, Point point1, IBrush? foreground)
    {
        point0 = GeometryHelper.ScalePoint(_scale, point0);
        point1 = GeometryHelper.ScalePoint(_scale, point1);
        var lineGeometry = new LineGeometry
        {
            StartPoint = point0.ToUwp(),
            EndPoint = point1.ToUwp()
        };
        _geometry.Children.Add(lineGeometry);
    }

    public void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground)
    {

    }

    public void RenderRectangle(Rectangle rectangle, IBrush? foreground)
    {
        var rectangleGeometry = new RectangleGeometry { Rect = GeometryHelper.ScaleRectangle(_scale, rectangle).ToWin2D() };
        _geometry.Children.Add(rectangleGeometry);
    }

    public void RenderTransformed(Box box, IEnumerable<Transformation> transforms, double x, double y)
    {
        var group = new GeometryGroup();
        var scaledTransforms = transforms.Select(t => t.Scale(_scale));
        ApplyTransformations(scaledTransforms, group);
        var nestedRenderer = new GeometryElementRenderer(group, _scale);
        nestedRenderer.RenderElement(box, x, y);
        _geometry.Children.Add(group);
    }

    public void FinishRendering() { }

    private static void ApplyTransformations(IEnumerable<Transformation> transformations, GeometryGroup geometry)
    {
        foreach (var transformation in transformations)
        {
            ApplyTransformation(transformation, geometry);
        }
    }

    private static void ApplyTransformation(Transformation transformation, GeometryGroup geometry)
    {
        Matrix matrix = new TransformGroup { Children = [geometry.Transform] }.Value;
        switch (transformation.Kind)
        {
            case TransformationKind.Translate:
                var tt = (Transformation.Translate)transformation;
                matrix.Translate(tt.X, tt.Y);
                break;
            case TransformationKind.Rotate:
                var rt = (Transformation.Rotate)transformation;
                matrix.Rotate(rt.RotationDegrees);
                break;
            default:
                throw new NotSupportedException($"Unknown {nameof(Transformation)} kind: {transformation.Kind}");
        }
    }
}
