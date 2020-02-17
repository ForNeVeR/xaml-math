using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class HtmlColorParser : SingleComponentColorParser
    {
        protected override RgbaColor? ParseSingleComponent(string component)
            => ColorHelpers.TryParseHtmlColor(component, out var color)
                ? color
                : (RgbaColor?)null;
    }
}
