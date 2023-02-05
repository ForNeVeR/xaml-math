using System;
using System.Collections.Generic;
using AvaloniaMath.Rendering;
using XamlMath;

namespace AvaloniaMath.Parsers;

public static class AvaloniaTeXFormulaParser
{
    public static TexFormulaParser Instance { get; }
    static AvaloniaTeXFormulaParser()
    {
        var predefinedFormulae = LoadPredefinedFormulae();
        Instance = new TexFormulaParser(AvaloniaBrushFactory.Instance, predefinedFormulae);
    }

    private static IReadOnlyDictionary<string, Func<SourceSpan, TexFormula?>> LoadPredefinedFormulae()
    {
        var predefinedFormulaParser = new TexPredefinedFormulaParser(AvaloniaBrushFactory.Instance);
        var predefinedFormulae = new Dictionary<string, Func<SourceSpan, TexFormula?>>();
        predefinedFormulaParser.Parse(predefinedFormulae);
        return predefinedFormulae;
    }
}
