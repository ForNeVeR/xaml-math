using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

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

        protected abstract Color? ParseComponents(List<string> components);

        public Color? Parse(IEnumerable<string> components)
        {
            var componentList = components.ToList();
            return componentList.Count == _componentCount ? ParseComponents(componentList) : null;
        }
    }
}
