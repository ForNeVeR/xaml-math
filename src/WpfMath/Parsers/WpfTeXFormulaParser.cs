using System;
using System.Collections.Generic;
using WpfMath.Rendering;
using XamlMath;

namespace WpfMath.Parsers;

public static class WpfTeXFormulaParser
{
    public static TexFormulaParser Instance { get; }
    static WpfTeXFormulaParser()
    {
        var predefinedFormulae = LoadPredefinedFormulae();
        Instance = new TexFormulaParser(WpfBrushFactory.Instance, predefinedFormulae);
    }

    private static IReadOnlyDictionary<string, Func<SourceSpan, TexFormula?>> LoadPredefinedFormulae()
    {
        var predefinedFormulaParser = new TexPredefinedFormulaParser(WpfBrushFactory.Instance);
        var predefinedFormulae = new Dictionary<string, Func<SourceSpan, TexFormula?>>();
        predefinedFormulaParser.Parse(predefinedFormulae);
        return predefinedFormulae;
    }
}
