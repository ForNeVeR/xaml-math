using System;
using System.Collections.Generic;
using System.Xml.Linq;
using XamlMath.Data;
using XamlMath.Utils;

namespace XamlMath;

// Parses information about glue settings from XML file.
internal sealed class GlueSettingsParser
{
    private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "GlueSettings.xml";

    private static readonly IReadOnlyDictionary<string, TexAtomType> typeMappings;
    private static readonly IReadOnlyDictionary<string, TexStyle> styleMappings;

    static GlueSettingsParser()
    {
        typeMappings = new Dictionary<string, TexAtomType>
        {
            ["ord"] = TexAtomType.Ordinary,
            ["op"] = TexAtomType.BigOperator,
            ["bin"] = TexAtomType.BinaryOperator,
            ["rel"] = TexAtomType.Relation,
            ["open"] = TexAtomType.Opening,
            ["close"] = TexAtomType.Closing,
            ["punct"] = TexAtomType.Punctuation,
            ["inner"] = TexAtomType.Inner,
        };

        styleMappings = new Dictionary<string, TexStyle>
        {
            ["display"] = (TexStyle)((int)TexStyle.Display / 2),
            ["text"] = (TexStyle)((int)TexStyle.Text / 2),
            ["script"] = (TexStyle)((int)TexStyle.Script / 2),
            ["script_script"] = (TexStyle)((int)TexStyle.ScriptScript / 2),
        };
    }

    private static readonly IReadOnlyList<string> names = new[] { "space", "stretch", "shrink" };
    private static Glue CreateGlue(XElement type, string name)
    {
        var values = new double[names.Count];
        for (int i = 0; i < names.Count; i++)
            values[i] = type.AttributeDoubleValue(names[i], 0d);
        return new Glue(values[0], values[1], values[2], name);
    }

    private readonly IReadOnlyList<Glue> glueTypes;
    private readonly IReadOnlyDictionary<string, int> glueTypeMappings;

    private readonly XElement rootElement;

    public GlueSettingsParser()
    {
        using var resource = typeof(XamlMathResourceMarker).Assembly.ReadResource(resourceName);

        var doc = XDocument.Load(resource);
        var root = doc.Root;
        var parsedGlueTypes = ParseGlueTypes(root);

        this.rootElement = root;
        this.glueTypes = parsedGlueTypes;
        this.glueTypeMappings = CreateReverseMappings(parsedGlueTypes);
    }

    public IReadOnlyList<Glue> GetGlueTypes()
    {
        return glueTypes;
    }

    public int[, ,] GetGlueRules()
    {
        var rules = new int[typeMappings.Count, typeMappings.Count, styleMappings.Count];
        var glueTableElement = rootElement.Element("GlueTable");
        if (glueTableElement != null)
        {
            foreach (var glueElement in glueTableElement.Elements("Glue"))
            {
                var leftType = typeMappings[glueElement.AttributeValue("lefttype")];
                var rightType = typeMappings[glueElement.AttributeValue("righttype")];
                var glueType = glueTypeMappings[glueElement.AttributeValue("gluetype")];

                foreach (var styleElement in glueElement.Elements("Style"))
                {
                    var styleName = styleElement.AttributeValue("name");
                    rules[(int)leftType, (int)rightType, (int)styleMappings[styleName]] = glueType;
                }
            }
        }

        return rules;
    }

    private static IReadOnlyDictionary<string, int> CreateReverseMappings(IReadOnlyList<Glue> types)
    {
        var result = new Dictionary<string, int>();
        for (int i = 0; i < types.Count; i++)
            result.Add(types[i].Name, i);
        return result;
    }

    private static IReadOnlyList<Glue> ParseGlueTypes (XElement root)
    {
        var result = new List<Glue>();

        int defaultIndex = -1;
        int index = 0;

        var glueTypesElement = root.Element("GlueTypes");
        if (glueTypesElement != null)
        {
            foreach (var glueTypeElement in glueTypesElement.Elements("GlueType"))
            {
                var name = glueTypeElement.AttributeValue("name");
                var glue = CreateGlue(glueTypeElement, name);
                if (name.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                    defaultIndex = index;
                result.Add(glue);
                index++;
            }
        }

        // Create default glue type if it does not exist.
        if (defaultIndex < 0)
        {
            defaultIndex = index;
            result.Add(new Glue(0, 0, 0, "default"));
        }

        // Insure that default glue type is first in list.
        if (defaultIndex > 0)
        {
            var tempGlueType = result[defaultIndex];
            result[defaultIndex] = result[0];
            result[0] = tempGlueType;
        }

        return result;
    }
}
