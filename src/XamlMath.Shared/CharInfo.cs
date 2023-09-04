using XamlMath.Fonts;

namespace XamlMath;

/// <summary>Single character together with information about font and metrics.</summary>
public class CharInfo
{
    public CharInfo(char character, IFontTypeface font, double size, int fontId, TeXFontMetrics metrics)
    {
        this.Character = character;
        Font = font;
        this.Size = size;
        FontId = fontId;
        this.Metrics = metrics;
    }

    public char Character
    {
        get;
        set;
    }

    public IFontTypeface Font
    {
        get;
    }

    public double Size
    {
        get;
        set;
    }

    public TeXFontMetrics Metrics
    {
        get;
        set;
    }

    public int FontId
    {
        get;
    }

    public CharFont GetCharacterFont()
    {
        return new CharFont(Character, FontId);
    }
}
