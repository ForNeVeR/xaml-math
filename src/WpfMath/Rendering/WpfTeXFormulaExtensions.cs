using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XamlMath;
using XamlMath.Rendering;

namespace WpfMath.Rendering;

public static class WpfTeXFormulaExtensions
{
    /// <summary>Default DPI for WPF.</summary>
    private const int DefaultDpi = 96;

    public static Geometry RenderToGeometry(
        this TexFormula formula,
        TexEnvironment environment,
        double scale = 20.0,
        double x = 0.0,
        double y = 0.0)
    {
        var geometry = new GeometryGroup();
        var renderer = new GeometryElementRenderer(geometry, scale);
        formula.RenderTo(renderer, environment, x, y);
        return geometry;
    }

    /// <summary>Renders the formula to a WPF bitmap.</summary>
    /// <param name="formula">The formula to render.</param>
    /// <param name="environment">The environment with rendering parameters.</param>
    /// <param name="scale">Formula text scale./</param>
    /// <param name="x">A physical X coordinate of the top left corner in the resulting bitmap.</param>
    /// <param name="y">A physical Y coordinate of the top left corner in the resulting bitmap.</param>
    /// <param name="dpi">The resulting image DPI.</param>
    public static BitmapSource RenderToBitmap(
        this TexFormula formula,
        TexEnvironment environment,
        double scale = 20.0,
        double x = 0,
        double y = 0,
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
            formula.RenderTo(drawingContext, environment, scale, x / scale, y / scale);

        var bounds = visual.ContentBounds;
        if (bounds is { X: >= 0, Y: >= 0 }) return;

        using (var drawingContext = visual.RenderOpen())
        {
            drawingContext.PushTransform(
                new TranslateTransform(Math.Max(0.0, -bounds.X), Math.Max(0.0, -bounds.Y)));
            formula.RenderTo(drawingContext, environment, scale, x / scale, y / scale);
        }
    }

    /// <summary>
    /// Renders the <paramref name="formula"/> to the <paramref name="drawingContext"/>.
    /// </summary>
    /// <param name="formula">The formula to render.</param>
    /// <param name="drawingContext">The target drawing context.</param>
    /// <param name="environment">The environment with rendering parameters.</param>
    /// <param name="scale">Formula text scale./</param>
    /// <param name="x">Logical X coordinate of the top left corner of the formula.</param>
    /// <param name="y">Logical Y coordinate of the top left corner of the formula.</param>
    public static void RenderTo(
        this TexFormula formula,
        DrawingContext drawingContext,
        TexEnvironment environment,
        double scale = 20.0,
        double x = 0.0,
        double y = 0.0)
    {
        formula.RenderTo(new WpfElementRenderer(drawingContext, scale), environment, x, y);
    }
}
