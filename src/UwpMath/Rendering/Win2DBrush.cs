using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

using System;

using Windows.UI;

using XamlMath.Colors;
using XamlMath.Rendering;

namespace UwpMath.Rendering;

public sealed record Win2DBrush : GenericBrush<ICanvasBrush>
{
    private Win2DBrush(ICanvasBrush brush) : base(brush)
    {
    }

    public static Win2DBrush FromBrush(ICanvasBrush value) => new(value);
}

public sealed class Win2DBrushFactory : IBrushFactory
{
    public static Win2DBrushFactory Instance => field ??= new();

    public ICanvasResourceCreator? ResourceCreator { get; set; }

    public IBrush FromColor(RgbaColor color)
    {
#if DEBUG
        if (ResourceCreator is null)
            throw new InvalidOperationException();
#endif
        return new CanvasSolidColorBrush(ResourceCreator, Color.FromArgb(color.A, color.R, color.G, color.B)).ToPlatform();
    }
}
