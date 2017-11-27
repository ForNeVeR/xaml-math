using System;
using System.Windows;
using System.Windows.Media;

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
        void RenderElement(Box box, double x, double y);

        /// <summary>Renders a glyph run (e.g. a character).</summary>
        /// <param name="scaledGlyphFactory">Function to generate a glyph run for the chosen scale.</param>
        /// <param name="x">An X coordinate of the top left corner.</param>
        /// <param name="y">An Y coordinate of the top left corner.</param>
        /// <param name="foreground">Glyph foreground color.</param>
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
        void RenderTransformed(Box box, Transform[] transforms, double x, double y);
    }
}
