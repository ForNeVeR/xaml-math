using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    public class TexParseException : Exception
    {
        internal TexParseException(string message)
            : base(message)
        {
        }

        internal TexParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
