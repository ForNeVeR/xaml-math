namespace WpfMath.Example;

public class Preset
{
    public Preset(string name, string formula)
    {
        Name = name;
        Formula = formula;
    }

    public string Name { get; }
    public string Formula { get; }
}
