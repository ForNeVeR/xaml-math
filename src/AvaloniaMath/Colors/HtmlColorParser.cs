using Avalonia.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class HtmlColorParser : SingleComponentColorParser
    {
        protected override Color? ParseSingleComponent(string component)
            => ColorHelpers.TryHtmlColorParse(component, out var color)
                ? Color.FromArgb(color.a, color.r, color.g, color.b)
                : (Color?)null;
    }
}
