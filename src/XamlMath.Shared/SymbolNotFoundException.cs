using System;

namespace XamlMath
{
    public sealed class SymbolNotFoundException : Exception
    {
        internal SymbolNotFoundException(string symbolName)
            : base(string.Format("Cannot find symbol with the name '{0}'.", symbolName))
        {
        }
    }
}
