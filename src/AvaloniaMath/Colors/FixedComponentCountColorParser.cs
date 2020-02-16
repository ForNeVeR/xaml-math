using System.Collections.Generic;
using Avalonia.Media;

namespace WpfMath.Colors
{
    /// <summary>A color parser that requires an exact number of components.</summary>
    public abstract class FixedComponentCountColorParser : IColorParser
    {
        private readonly int _componentCount;

        protected FixedComponentCountColorParser(int componentCount)
        {
            _componentCount = componentCount;
        }

        protected abstract Color? ParseComponents(IReadOnlyList<string> components);

        public Color? Parse(IReadOnlyList<string> components)
            => components.Count == _componentCount ? ParseComponents(components) : null;
    }
}
