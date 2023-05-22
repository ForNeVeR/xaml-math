using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using XamlMath.Rendering;
using WpfPoint = System.Windows.Point;

namespace WpfMath.Rendering;

public static class WpfExtensions
{
    [return: NotNullIfNotNull(nameof(brush))]
    public static IBrush? ToPlatform(this Brush? brush) => brush == null ? null : WpfBrush.FromBrush(brush);

    [return: NotNullIfNotNull(nameof(brush))]
    public static Brush? ToWpf(this IBrush? brush) => ((WpfBrush?)brush)?.Value;

    public static WpfPoint ToWpf(this Point point) => new(point.X, point.Y);
}
