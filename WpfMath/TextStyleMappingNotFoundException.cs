using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    public class TextStyleMappingNotFoundException : Exception
    {
        internal TextStyleMappingNotFoundException(string textStyleName)
            : base(string.Format("Cannot find mapping for the style with name '{0}'.", textStyleName))
        {
        }
    }
}
