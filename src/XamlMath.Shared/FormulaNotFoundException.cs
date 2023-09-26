using System;

namespace XamlMath;

public sealed class FormulaNotFoundException : Exception
{
    internal FormulaNotFoundException(string formulaName)
        : base($"Cannot find predefined formula with name '{formulaName}'.")
    {
    }
}
