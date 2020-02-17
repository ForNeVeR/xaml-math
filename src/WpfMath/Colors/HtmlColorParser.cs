using System.Windows.Media;
using WpfMath.Utils;

namespace WpfMath.Colors
{
    internal class HtmlColorParser : SingleComponentColorParser
    {
        protected override Color? ParseSingleComponent(string component)
            => ColorHelpers.TryParseHtmlColor(component, out var color)
                ? Color.FromArgb(color.A, color.R, color.G, color.B)
                : (Color?)null;
    }
}
