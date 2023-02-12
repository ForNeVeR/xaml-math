using System.Diagnostics.CodeAnalysis;
using XamlMath.Colors;
using XamlMath.Rendering;

namespace WinFormsMath.Rendering;

public record WinFormsBrush : GenericBrush<Brush>
{
    private WinFormsBrush(Brush brush) : base(brush)
    {
    }

    public static WinFormsBrush FromBrush(Brush value) => new(value);
}

public class WinFormsBrushFactory : IBrushFactory
{
    public static readonly WinFormsBrushFactory Instance = new();

    private WinFormsBrushFactory() {}

    public IBrush FromColor(RgbaColor color) =>
        new SolidBrush(
            Color.FromArgb(color.A, color.R, color.G, color.B)).ToPlatform();
}

public static class WinFormsBrushExtensions
{
    public static Brush? ToWinForms(this IBrush? brush) => ((WinFormsBrush?)brush)?.Value;
    [return: NotNullIfNotNull(nameof(brush))]
    public static IBrush? ToPlatform(this Brush? brush) => brush == null ? null : WinFormsBrush.FromBrush(brush);
}
