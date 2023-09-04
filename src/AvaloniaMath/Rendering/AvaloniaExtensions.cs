using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using XamlMath.Rendering;
using IBrush = XamlMath.Rendering.IBrush;

namespace AvaloniaMath.Rendering;

public static class AvaloniaExtensions
{
    [return: NotNullIfNotNull(nameof(brush))]
    public static IBrush? ToPlatform(this Brush? brush) => brush == null ? null : AvaloniaBrush.FromBrush(brush);

    [return: NotNullIfNotNull(nameof(brush))]
    public static Avalonia.Media.IBrush? ToAvalonia(this IBrush? brush) => ((AvaloniaBrush?)brush)?.Value;

    public static Avalonia.Point ToAvalonia(this Point point) => new(point.X, point.Y);
}
