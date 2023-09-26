using System;

namespace XamlMath;

public sealed class SymbolMappingNotFoundException : Exception
{
    internal SymbolMappingNotFoundException(string symbolName)
        : base($"Cannot find mapping for the symbol with name '{symbolName}'.")
    {
    }
}
