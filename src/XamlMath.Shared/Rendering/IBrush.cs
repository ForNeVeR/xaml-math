namespace XamlMath.Rendering;

/// <summary>
/// This is a marker interface for a platform-dependent Brush object. It was introduced in scope of cross-platform
/// support, to support both Avalonia and WPF-dependent Brush types in an opaque way.
/// <para/>
/// A platform-dependent inheritor of <see cref="IElementRenderer"/> is supposed to cast this to a
/// platform-dependent implementation to extract the containing value.
/// </summary>
public interface IBrush
{
}
