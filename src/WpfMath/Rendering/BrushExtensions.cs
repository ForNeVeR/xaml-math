using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

namespace WpfMath.Rendering;

internal static class BrushExtensions
{
    public static Brush? ToWpf(this IPlatformBrush? brush) => ((WpfBrush?)brush)?.Value;
    [return: NotNullIfNotNull(nameof(brush))]
    public static IPlatformBrush? ToPlatform(this Brush? brush) => brush == null ? null : WpfBrush.FromBrush(brush);
}
