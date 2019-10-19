using System.Collections.Generic;
using System.Windows.Media;

namespace WpfMath.Colors
{
    /// <summary>A base class for color parsers that only require one component.</summary>
    internal abstract class SingleComponentColorParser : IColorParser
    {
        public Color? Parse(IEnumerable<string> components)
        {
            // Return a color iff components contain only one element.
            var firstItem = true;
            Color? color = null;
            foreach (var component in components)
            {
                if (!firstItem)
                    return null;

                color = ParseSingleComponent(component);

                firstItem = false;
            }

            return color;
        }

        protected abstract Color? ParseSingleComponent(string component);
    }
}
