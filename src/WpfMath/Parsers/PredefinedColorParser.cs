using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Xml.Linq;
using System.Reflection;

namespace WpfMath.Parsers
{
    internal class PredefinedColorParser
    {
        public static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedColors.xml";
        private XElement rootElement;

        public PredefinedColorParser()
        {
            var doc = XDocument.Load(new System.IO.StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)));
            this.rootElement = doc.Root;
        }

        public void Parse(IDictionary<string, Color> predefinedColors)
        {
            foreach (var colorElement in rootElement.Elements("color"))
            {
                var name = colorElement.AttributeValue("name");
                var r = colorElement.AttributeValue("r");
                var g = colorElement.AttributeValue("g");
                var b = colorElement.AttributeValue("b");
                predefinedColors.Add(name, Color.FromRgb(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b)));
            }
        }
    }
}
