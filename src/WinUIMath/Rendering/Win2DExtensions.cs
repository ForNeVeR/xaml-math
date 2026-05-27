using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml.Media;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

using WinUIMath.Fonts;

using XamlMath.Rendering;

namespace WinUIMath.Rendering;

public static class Win2DExtensions
{
    [return: NotNullIfNotNull(nameof(brush))]
    public static IBrush? ToPlatform(this ICanvasBrush? brush) => brush == null ? null : Win2DBrush.FromBrush(brush);

    public static ICanvasBrush? ToWin2D(this IBrush? brush) => (brush as Win2DBrush)?.Value;

    public static Vector2 ToWin2D(this Point point) => new((float) point.X, (float) point.Y);

    public static void DrawGlyphRun(this CanvasDrawingSession drawingSession, Win2DGlyphRun glyphRun, ICanvasBrush brush)
    {
        CanvasGlyph[] glyphs = [.. glyphRun.GlyphIndices.Select(index => new CanvasGlyph
        {
            Index = index
        })];
        drawingSession.DrawGlyphRun(glyphRun.BaselineOrigin, glyphRun.FontFace, glyphRun.FontSize, glyphs, false, 0, brush);
    }

    public static ICanvasBrush? ToWin2DBrush(this Brush brush, ICanvasResourceCreator resourceCreator)
    {
        return brush switch
        {
            SolidColorBrush solidColorBrush => new CanvasSolidColorBrush(resourceCreator, solidColorBrush.Color)
            {
                Opacity = (float) solidColorBrush.Opacity
            },
            LinearGradientBrush linearGradientBrush => new CanvasLinearGradientBrush(resourceCreator, ToWin2DGradientStops(linearGradientBrush.GradientStops))
            {
                Opacity = (float) linearGradientBrush.Opacity,
                StartPoint = ToVector2(linearGradientBrush.StartPoint),
                EndPoint = ToVector2(linearGradientBrush.EndPoint)
            },
            RadialGradientBrush radialGradientBrush => new CanvasRadialGradientBrush(resourceCreator, ToWin2DGradientStops(radialGradientBrush.GradientStops))
            {
                Opacity = (float) radialGradientBrush.Opacity
            },
            _ => null,
        };
    }

    private static Vector2 ToVector2(Windows.Foundation.Point point) => new((float) point.X, (float) point.Y);

    private static CanvasGradientStop[] ToWin2DGradientStops(IEnumerable<GradientStop> stops) => [.. stops.Select(stop => new CanvasGradientStop
    {
        Color = stop.Color,
        Position = (float) stop.Offset
    })];
}
