using System;
using System.ComponentModel;
using System.Globalization;

namespace WpfMath.Colors
{
    public sealed class ArgbColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(ArgbColor) || base.CanConvertFrom(context, sourceType);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
            destinationType == typeof(string) || base.CanConvertTo(context, destinationType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
            value is string argbColor
                ? ArgbColor.Parse(argbColor)
                : (object)null;

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            destinationType == typeof(string)
                ? value.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
    }
}
