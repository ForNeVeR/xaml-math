using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfMath.Rendering;

public static class WpfTeXFormulaExtensions
{
    /// <summary>Default DPI for WPF.</summary>
    private const int DefaultDpi = 96;

    public static Geometry RenderToGeometry( // TODO: Tests for this method.
        this TexFormula formula,
        TexEnvironment environment,
        double scale, // TODO: Get rid of this; it is already encoded in the renderer anyway.
        double x = 0.0,
        double y = 0.0)
    {
        var geometry = new GeometryGroup();
        var renderer = new GeometryElementRenderer(geometry, scale);
        formula.RenderTo(renderer, environment, scale, x, y);
        return geometry;
    }

    public static BitmapSource RenderToBitmap( // TODO: Tests for this method.
        this TexFormula formula,
        TexEnvironment environment,
        double scale, // TODO: Get rid of this; it is already encoded in the renderer anyway.
        double x,
        double y,
        double dpi = DefaultDpi)
    {
        var visual = new DrawingVisual();
        RenderWithPositiveCoordinates(formula, environment, visual, scale, x, y);

        var bounds = visual.ContentBounds;
        var width = (int)Math.Ceiling((bounds.Right + x) * dpi / DefaultDpi);
        var height = (int)Math.Ceiling((bounds.Bottom + y) * dpi / DefaultDpi);
        var bitmap = new RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.Default);
        bitmap.Render(visual);

        return bitmap;
    }

    private static void RenderWithPositiveCoordinates(
        TexFormula formula,
        TexEnvironment environment,
        DrawingVisual visual,
        double scale,
        double x,
        double y)
    {
        using (var drawingContext = visual.RenderOpen())
            formula.RenderTo(drawingContext, environment, scale, x, y);

        var bounds = visual.ContentBounds;
        if (bounds.X >= 0 && bounds.Y >= 0) return;

        using (var drawingContext = visual.RenderOpen())
        {
            drawingContext.PushTransform(
                new TranslateTransform(Math.Max(0.0, -bounds.X), Math.Max(0.0, -bounds.Y)));
            formula.RenderTo(drawingContext, environment, scale, x, y);
        }
    }

    public static void RenderTo( // TODO: Tests for this method.
        this TexFormula formula,
        DrawingContext drawingContext,
        TexEnvironment environment,
        double scale,
        double x,
        double y)
    {
        formula.RenderTo(new WpfElementRenderer(drawingContext, scale), environment, scale, x, y);
    }
}
