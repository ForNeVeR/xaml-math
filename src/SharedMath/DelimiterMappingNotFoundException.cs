using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
