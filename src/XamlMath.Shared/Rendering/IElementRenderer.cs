using System.Collections.Generic;
using XamlMath.Boxes;
using XamlMath.Rendering.Transformations;

namespace XamlMath.Rendering;

/// <summary>Renderer interface for XAML-Math elements.</summary>
public interface IElementRenderer
{
    /// <summary>
    /// Renders a character denoted by <paramref name="info"/> at the chosen coordinates, using the color
    /// <paramref name="foreground"/>. Should use platform default color if passed <c>null</c>.
    /// </summary>
    void RenderCharacter(CharInfo info, double x, double y, IBrush? foreground);

    /// <summary>Renders a <see cref="Box"/> to the renderer drawing context.</summary>
    /// <param name="box">The element that should be rendered.</param>
    /// <param name="x">Logical X coordinate of the top left corner.</param>
    /// <param name="y">Logical Y coordinate of the top left corner.</param>
    /// <remarks>
    /// <para>Should be called for every element of the formula (including nested ones).</para>
    /// <para>
    ///     Usually this method should call <see cref="Box.RenderTo"/> with optional code common for all elements.
    /// </para>
    /// </remarks>
    void RenderElement(Box box, double x, double y);

    /// <summary>Renders a line</summary>
    /// <param name="point0">First line end</param>
    /// <param name="point1">Second line end</param>
    /// <param name="foreground">
    /// Line foreground color. Should use platform default color if passed <c>null</c>.
    /// </param>
    void RenderLine(Point point0, Point point1, IBrush? foreground);

    /// <summary>Renders a rectangle.</summary>
    /// <param name="rectangle">Rectangle to render.</param>
    /// <param name="foreground">
    /// Rectangle foreground color. Should use platform default color if passed <c>null</c>.
    /// </param>
    void RenderRectangle(Rectangle rectangle, IBrush? foreground);

    /// <summary>Renders a box applying the geometry transforms.</summary>
    /// <param name="box">A box to render.</param>
    /// <param name="transforms">A transform array.</param>
    /// <param name="x">An X coordinate of the top left corner.</param>
    /// <param name="y">An Y coordinate of the top left corner.</param>
    void RenderTransformed(Box box, IEnumerable<Transformation> transforms, double x, double y);

    /// <summary>
    /// Finalize the rendered image. Should always be called before using the rendering results. Should only be
    /// called once per context instance.
    /// </summary>
    /// <remarks>
    /// Required for some of the renderers to perform the final work on the image. E.g. the WPF renderer will merge
    /// the background and the foreground image layers in this method.
    /// </remarks>
    void FinishRendering();
}
