namespace XamlMath.Fonts;

public interface IFontProvider
{
    /// <summary>Reads a TeX font of specified file name from the application's resources.</summary>
    /// <param name="fontFileName"></param>
    IFontTypeface ReadFontFile(string fontFileName);
}
