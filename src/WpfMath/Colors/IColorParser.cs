using System.Collections.Generic;
using System.Windows.Media;

namespace WpfMath.Colors
{
    /// <summary>A parser for colors in commands like \color and \colorbox.</summary>
    public interface IColorParser
    {
        /// <summary>Parses the color components.</summary>
        /// <param name="components">A sequence of the components that were separated by comma.</param>
        /// <returns>Either a parsed color or <c>null</c> if it cannot be parsed.</returns>
        Color? Parse(IEnumerable<string> components);
    }
}
