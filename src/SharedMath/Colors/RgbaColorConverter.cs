using System;
using System.ComponentModel;
using System.Globalization;

namespace WpfMath.Colors
{
    public sealed class RgbaColorConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => destinationType == typeof(string)
                ? value.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
    }
}
