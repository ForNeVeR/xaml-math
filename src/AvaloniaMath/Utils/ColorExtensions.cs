namespace AvaloniaMath.Utils
{
    using Avalonia.Media;
    using WpfMath.Colors;

    internal static class ColorExtensions
    {
        public static Color ToAvaloniaColor(this ArgbColor value) =>
            Color.FromArgb(value.A, value.R, value.G, value.B);

        public static ArgbColor ToInternalColor(this Color value) =>
            new ArgbColor(value.A, value.R, value.G, value.B);
    }
}
