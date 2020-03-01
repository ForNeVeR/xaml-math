using System.Collections.Generic;

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

        protected abstract ArgbColor? ParseComponents(IReadOnlyList<string> components);

        public ArgbColor? Parse(IReadOnlyList<string> components)
            => components.Count == _componentCount ? ParseComponents(components) : null;
    }
}
