namespace XamlMath.Colors;

public record struct RgbaColor(byte R, byte G, byte B, byte A = 0xff)
{
    public static RgbaColor FromArgb(byte a, byte r, byte g, byte b) => new(r, g, b, a);
    public static RgbaColor FromRgb(byte r, byte g, byte b) => new(r, g, b);
}
