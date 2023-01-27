using System.Collections.Generic;
using System.Linq;

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

        protected abstract RgbaColor? ParseComponents(List<string> components);

        public RgbaColor? Parse(IEnumerable<string> components)
        {
            var componentList = components.ToList();
            return componentList.Count == _componentCount ? ParseComponents(componentList) : null;
        }
    }
}
