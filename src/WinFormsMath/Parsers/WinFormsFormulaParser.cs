using WinFormsMath.Rendering;
using XamlMath;

namespace WinFormsMath.Parsers;

public static class WinFormsFormulaParser
{
    public static TexFormulaParser Instance { get; }
    static WinFormsFormulaParser()
    {
        var predefinedFormulae = LoadPredefinedFormulae();
        Instance = new TexFormulaParser(WinFormsBrushFactory.Instance, predefinedFormulae);
    }

    private static IReadOnlyDictionary<string, Func<SourceSpan, TexFormula?>> LoadPredefinedFormulae()
    {
        var predefinedFormulaParser = new TexPredefinedFormulaParser(WinFormsBrushFactory.Instance);
        var predefinedFormulae = new Dictionary<string, Func<SourceSpan, TexFormula?>>();
        predefinedFormulaParser.Parse(predefinedFormulae);
        return predefinedFormulae;
    }
}
