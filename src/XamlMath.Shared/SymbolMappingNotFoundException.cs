using System;

namespace XamlMath
{
    public class SymbolMappingNotFoundException : Exception
    {
        internal SymbolMappingNotFoundException(string symbolName)
            : base(string.Format("Cannot find mapping for the symbol with name '{0}'.", symbolName))
        {
        }
    }
}
