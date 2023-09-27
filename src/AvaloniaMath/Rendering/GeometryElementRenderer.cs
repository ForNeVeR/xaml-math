using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using XamlMath;
using XamlMath.Boxes;
using XamlMath.Rendering;
using XamlMath.Rendering.Transformations;
using IBrush = XamlMath.Rendering.IBrush;

namespace AvaloniaMath.Rendering;

/// <summary>A renderer that renders the elements to a provided <see cref="GeometryGroup"/> instance.</summary>
// TODO[#357]: Make this work
internal sealed class GeometryElementRenderer : IElementRenderer
{
    private readonly GeometryGroup _geometry;
    private readonly double _scale;

    public GeometryElementRenderer(GeometryGroup geometry, double scale)
    {
        _geometry = geometry;
        _scale = scale;
    }

    public void RenderElement(Box box, double x, double y) => box.RenderTo(this, x, y);

    public void RenderLine(Point point0, Point point1, IBrush? foreground)
    {
        point0 = GeometryHelper.ScalePoint(_scale, point0);
        point1 = GeometryHelper.ScalePoint(_scale, point1);
        var lineGeometry = new LineGeometry(point0.ToAvalonia(), point1.ToAvalonia());
        _geometry.Children.Add(lineGeometry);
    }

    public void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground)
    {
        /*TODO[#357]       var glyph = scaledGlyphFactory(_scale);
               var glyphGeometry = glyph.BuildGeometry();
               _geometry.Children.Add(glyphGeometry);
               */
    }

    public void RenderRectangle(Rectangle rectangle, IBrush? foreground)
    {
        var rectangleGeometry = new RectangleGeometry(GeometryHelper.ScaleRectangle(_scale, rectangle).ToAvalonia());
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

    public void FinishRendering()
    { }

    private static void ApplyTransformations(IEnumerable<Transformation> transformations, GeometryGroup geometry)
    {
        /*TODO[#357]
        foreach (var transformation in transformations)
        {
            switch (transformation.Kind)
            {
                case TransformationKind.Translate:
                    var tt = (Transformation.Translate) transformation;
                    geometry.Transform.Value.Translate(tt.X, tt.Y);
                    break;
                case TransformationKind.Rotate:
                    var rt = (Transformation.Rotate) transformation;
                    geometry.Transform.Value.Rotate(rt.RotationDegrees);
                    break;
                default:
                    throw new NotSupportedException(
                        $"Unknown {nameof(Transformation)} kind: {transformation.Kind}");
            }
        }
        */
    }
}
