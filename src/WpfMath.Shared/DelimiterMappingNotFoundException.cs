using System;

namespace WpfMath
{
    public class DelimiterMappingNotFoundException : Exception
    {
        internal DelimiterMappingNotFoundException(char delimiter)
            : base(string.Format("Cannot find delimeter mapping for the character '{0}'.", delimiter))
        {
        }
    }
}
