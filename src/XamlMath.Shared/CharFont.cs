namespace XamlMath;

/// <summary>Single character together with specific font.</summary>
public class CharFont
{
    public CharFont(char character, int fontId)
    {
        this.Character = character;
        this.FontId = fontId;
    }

    public char Character { get; }

    public int FontId { get; }
}
