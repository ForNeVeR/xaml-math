using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

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
