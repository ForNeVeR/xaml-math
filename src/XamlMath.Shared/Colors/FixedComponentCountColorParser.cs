using System.Collections.Generic;

namespace XamlMath.Colors;

/// <summary>A color parser that requires an exact number of components.</summary>
public abstract class FixedComponentCountColorParser : IColorParser
{
    private readonly int _componentCount;

    protected FixedComponentCountColorParser(int componentCount)
    {
        _componentCount = componentCount;
    }

    protected abstract RgbaColor? ParseComponents(IReadOnlyList<string> components);

    public RgbaColor? Parse(IReadOnlyList<string> components)
    {
        return components.Count == _componentCount ? ParseComponents(components) : null;
    }
}
