using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace WpfMath
{
    // Parses information about glue settings from XML file.
    internal class GlueSettingsParser
    {
        private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "GlueSettings.xml";

        private static IDictionary<string, TexAtomType> typeMappings;
        private static IDictionary<string, TexStyle> styleMappings;

        static GlueSettingsParser()
        {
            typeMappings = new Dictionary<string, TexAtomType>();
            styleMappings = new Dictionary<string, TexStyle>();

            SetTypeMappings();
            SetStyleMappings();
        }

        private static Glue CreateGlue(XElement type, string name)
        {
            var names = new string[] { "space", "stretch", "shrink" };
            var values = new double[names.Length];
            for (int i = 0; i < names.Length; i++)
                values[i] = type.AttributeDoubleValue(names[i], 0d);
            return new Glue(values[0], values[1], values[2], name);
        }

        private static void SetTypeMappings()
        {
            typeMappings.Add("ord", TexAtomType.Ordinary);
            typeMappings.Add("op", TexAtomType.BigOperator);
            typeMappings.Add("bin", TexAtomType.BinaryOperator);
            typeMappings.Add("rel", TexAtomType.Relation);
            typeMappings.Add("open", TexAtomType.Opening);
            typeMappings.Add("close", TexAtomType.Closing);
            typeMappings.Add("punct", TexAtomType.Punctuation);
            typeMappings.Add("inner", TexAtomType.Inner);
        }

        private static void SetStyleMappings()
        {
            styleMappings.Add("display", (TexStyle)((int)TexStyle.Display / 2));
            styleMappings.Add("text", (TexStyle)((int)TexStyle.Text / 2));
            styleMappings.Add("script", (TexStyle)((int)TexStyle.Script / 2));
            styleMappings.Add("script_script", (TexStyle)((int)TexStyle.ScriptScript / 2));
        }

        private IList<Glue> glueTypes;
        private IDictionary<string, int> glueTypeMappings;

        private XElement rootElement;

        public GlueSettingsParser()
        {
            var doc = XDocument.Load(new System.IO.StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)));
            this.rootElement = doc.Root;
            ParseGlueTypes();
        }

        public IList<Glue> GetGlueTypes()
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

        private void ParseGlueTypes()
        {
            this.glueTypes = new List<Glue>();
            this.glueTypeMappings = new Dictionary<string, int>();

            int defaultIndex = -1;
            int index = 0;

            var glueTypesElement = rootElement.Element("GlueTypes");
            if (glueTypesElement != null)
            {
                foreach (var glueTypeElement in glueTypesElement.Elements("GlueType"))
                {
                    var name = glueTypeElement.AttributeValue("name");
                    var glue = CreateGlue(glueTypeElement, name);
                    if (name.Equals("default", StringComparison.InvariantCultureIgnoreCase))
                        defaultIndex = index;
                    glueTypes.Add(glue);
                    index++;
                }
            }

            // Create default glue type if it does not exist.
            if (defaultIndex < 0)
            {
                defaultIndex = index;
                glueTypes.Add(new Glue(0, 0, 0, "default"));
            }

            // Insure that default glue type is first in list.
            if (defaultIndex > 0)
            {
                var tempGlueType = glueTypes[defaultIndex];
                glueTypes[defaultIndex] = glueTypes[0];
                glueTypes[0] = tempGlueType;
            }

            // Create dictionary of reverse mappings.
            for (int i = 0; i < glueTypes.Count; i++)
                glueTypeMappings.Add(glueTypes[i].Name, i);
        }
    }
}
