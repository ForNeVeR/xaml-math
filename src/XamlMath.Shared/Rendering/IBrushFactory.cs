using WpfMath.Colors;

namespace WpfMath.Rendering;

public interface IBrushFactory
{
    IBrush FromColor(RgbaColor color);
}
