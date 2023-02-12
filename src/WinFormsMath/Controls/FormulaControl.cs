using WinFormsMath.Parsers;
using WinFormsMath.Rendering;
using XamlMath.Rendering;

namespace WinFormsMath.Controls;

public class FormulaControl : Control
{
    public string FormulaText { get; set; } // TODO: Naming?

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        var texFormula = WinFormsFormulaParser.Instance.Parse(FormulaText);
        var environment = WinFormsTeXEnvironment.Create();

        var renderer = new WinFormsRenderer(e.Graphics);
        texFormula.RenderTo(renderer, environment, 0.0, 0.0);
    }
}
