using System.Collections.Generic;

namespace XamlMath.Parsers.PredefinedFormulae;

/// <summary>
/// This is a context for parsing a predefined formula. <c>PredefinedTexFormulas.xml</c> allows the users to
/// introduce their own named formulae via <example>&lt;CreateFormula name="f"&gt;</example>, and then call methods
/// on them.
/// <para/>
/// This context is the storage of these named formula values.
/// </summary>
internal sealed class PredefinedFormulaContext
{
    private readonly Dictionary<string, TexFormula> _formulae = new();
    public void AddFormula(string name, TexFormula formula) => _formulae.Add(name, formula);
    public TexFormula this[string name] => _formulae[name];
}
