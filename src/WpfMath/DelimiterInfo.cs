using WpfMath.Atoms;

namespace WpfMath
{
    /// <summary>
    /// Information about the body between a pair of delimiters.
    /// </summary>
    internal class DelimiterInfo
    {
        public Atom Body { get; }

        public SymbolAtom ClosingDelimiter { get; }

        public DelimiterInfo(Atom body, SymbolAtom closingDelimiter)
        {
            Body = body;
            ClosingDelimiter = closingDelimiter;
        }
    }
}
