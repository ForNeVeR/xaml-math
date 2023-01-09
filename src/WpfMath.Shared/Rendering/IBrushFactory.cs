using WpfMath.Colors;

namespace WpfMath.Rendering;

public interface IBrushFactory
{
    public IPlatformBrush FromColor(RgbaColor color);
}
