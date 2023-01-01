using WpfMath.Boxes;

namespace WpfMath.Rendering;

public static class TeXFormulaExtensions // TODO: Move to the shared assembly.
{
    public static void RenderTo( // TODO: Tests for this method.
        this TexFormula formula,
        IElementRenderer renderer,
        TexEnvironment environment,
        double scale, // TODO: Get rid of this; it is already encoded in the renderer anyway.
        double x,
        double y)
    {
        var box = formula.CreateBox(environment);
        Render(box, renderer, scale, x, y);
    }

    // TODO: Merge into RenderTo after we get rid of the TexRenderer class.
    internal static void Render(Box box, IElementRenderer renderer, double scale, double x, double y)
    {
        renderer.RenderElement(box, x / scale, y / scale + box.Height);
        renderer.FinishRendering();
    }
}
