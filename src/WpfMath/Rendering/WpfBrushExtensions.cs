using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;
using XamlMath.Rendering;

namespace WpfMath.Rendering;

public static class WpfBrushExtensions
{
    public static Brush? ToWpf(this IBrush? brush) => ((WpfBrush?)brush)?.Value;
    [return: NotNullIfNotNull(nameof(brush))]
    public static IBrush? ToPlatform(this Brush? brush) => brush == null ? null : WpfBrush.FromBrush(brush);
}
