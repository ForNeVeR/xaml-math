namespace XamlMath.Exceptions;

public sealed class TextStyleMappingNotFoundException : TexException
{
    internal TextStyleMappingNotFoundException(string textStyleName)
        : base(string.Format("Cannot find mapping for the style with name '{0}'.", textStyleName))
    {
    }
}
