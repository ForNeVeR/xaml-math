using System.Collections.Generic;
using XamlMath.Boxes;

namespace XamlMath;

// Represents glueElement for holding together boxes.
internal sealed class Glue
{
    private static readonly IReadOnlyList<Glue> glueTypes;
    private static readonly int[, ,] glueRules;

    static Glue()
    {
        var parser = new GlueSettingsParser();
        glueTypes = parser.GetGlueTypes();
        glueRules = parser.GetGlueRules();
    }

    public static Box CreateBox(TexAtomType leftAtomType, TexAtomType rightAtomType, TexEnvironment environment)
    {
        leftAtomType = leftAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : leftAtomType;
        rightAtomType = rightAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : rightAtomType;
        var glueType = glueRules[(int)leftAtomType, (int)rightAtomType, (int)environment.Style / 2];
        return glueTypes[glueType].CreateBox(environment);
    }

    public Glue(double space, double stretch, double shrink, string name)
    {
        this.Space = space;
        this.Stretch = stretch;
        this.Shrink = shrink;
        this.Name = name;
    }

    public double Space { get; }
    public double Stretch { get; }
    public double Shrink { get; }
    public string Name { get; }

    private Box CreateBox(TexEnvironment environment)
    {
        var texFont = environment.MathFont;
        var quad = texFont.GetQuad(texFont.GetMuFontId(), environment.Style);
        return new GlueBox((this.Space / 18.0f) * quad, (this.Stretch / 18.0f) * quad, (this.Shrink / 18.0f) * quad);
    }
}
