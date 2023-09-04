namespace XamlMath;

/// <summary>Extension character that contains character information for each of its parts.</summary>
public class ExtensionChar
{
    public ExtensionChar(CharInfo? top, CharInfo? middle, CharInfo? bottom, CharInfo? repeat)
    {
        this.Top = top;
        this.Middle = middle;
        this.Repeat = repeat;
        this.Bottom = bottom;
    }

    public CharInfo? Top { get; }
    public CharInfo? Middle { get; }
    public CharInfo? Bottom { get; }
    public CharInfo? Repeat { get; }
}
