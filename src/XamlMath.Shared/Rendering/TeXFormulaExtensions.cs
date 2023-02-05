using XamlMath.Boxes;

namespace XamlMath.Rendering;

public static class TeXFormulaExtensions
{
    /// <summary>Renders a <paramref name="formula"/> to the passed <paramref name="renderer"/>.</summary>
    /// <param name="environment">Rendering parameters.</param>
    /// <param name="x">Logical X coordinate of the top left corner of the formula.</param>
    /// <param name="y">Logical Y coordinate of the top left corner of the formula.</param>
    public static void RenderTo(
        this TexFormula formula,
        IElementRenderer renderer,
        TexEnvironment environment,
        double x,
        double y)
    {
        var box = formula.CreateBox(environment);
        Render(box, renderer, x, y);
    }

    internal static void Render(Box box, IElementRenderer renderer, double x, double y)
    {
        renderer.RenderElement(box, x, y + box.Height);
        renderer.FinishRendering();
    }
}
