using System;
using System.Windows;
using System.Windows.Media;
using WpfMath.Boxes;
using WpfMath.Rendering.Transformations;

namespace WpfMath.Rendering
{
    /// <summary>Renderer interface for WPF-Math elements.</summary>
    public interface IElementRenderer
    {
        /// <summary>Renders a <see cref="Box"/> to the renderer drawing context.</summary>
        /// <param name="box">The element that should be rendered.</param>
        /// <param name="x">An X coordinate of the top left corner.</param>
        /// <param name="y">An Y coordinate of the top left corner.</param>
        /// <remarks>Should be called for every element of the formula (including nested ones).</remarks>
        /// <remarks>
        /// Usually this method should call <see cref="Box.RenderTo"/> with optional code common for all elements.
        /// </remarks>
        void RenderElement(Box box, double x, double y);

        /// <summary>Renders a glyph run (e.g. a character).</summary>
        /// <param name="scaledGlyphFactory">Function to generate a glyph run for the chosen scale.</param>
        /// <param name="x">An X coordinate of the top left corner.</param>
        /// <param name="y">An Y coordinate of the top left corner.</param>
        /// <param name="foreground">Glyph foreground color.</param>
        // TODO[F]: Scale the GlyphRun in the implementations, replace the factory with the initial (unscaled) GlyphRun
        void RenderGlyphRun(Func<double, GlyphRun> scaledGlyphFactory, double x, double y, Brush foreground);

        /// <summary>Renders a rectangle.</summary>
        /// <param name="rectangle">Rectangle to render.</param>
        /// <param name="foreground">Rectangle foreground color.</param>
        void RenderRectangle(Rect rectangle, Brush foreground);

        /// <summary>Renders a box applying the geometry transforms.</summary>
        /// <param name="box">A box to render.</param>
        /// <param name="transforms">A transform array.</param>
        /// <param name="x">An X coordinate of the top left corner.</param>
        /// <param name="y">An Y coordinate of the top left corner.</param>
        void RenderTransformed(Box box, Transformation[] transforms, double x, double y);

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
}
