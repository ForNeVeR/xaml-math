using System;
using System.Collections.Generic;

using WinUIMath.Rendering;

using XamlMath;

namespace WinUIMath.Parsers;

public static class Win2DTeXFormulaParser
{
    public static TexFormulaParser Instance { get; }

    static Win2DTeXFormulaParser()
    {
        var predefinedFormulae = LoadPredefinedFormulae();
        Instance = new TexFormulaParser(Win2DBrushFactory.Instance, predefinedFormulae);
    }

    private static IReadOnlyDictionary<string, Func<SourceSpan, TexFormula?>> LoadPredefinedFormulae()
    {
        var predefinedFormulaParser = new TexPredefinedFormulaParser(Win2DBrushFactory.Instance);
        var predefinedFormulae = new Dictionary<string, Func<SourceSpan, TexFormula?>>();
        predefinedFormulaParser.Parse(predefinedFormulae);
        return predefinedFormulae;
    }
}
