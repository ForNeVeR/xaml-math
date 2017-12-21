using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Media;
using System.Xml.Linq;
using System.Reflection;

namespace WpfMath
{
    internal class PredefinedColorParser
    {
        public static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedColors.xml";
        private XElement rootElement;

        public PredefinedColorParser()
        {
            var assembly = typeof(GlueBox).GetTypeInfo().Assembly;
            var doc = XDocument.Load(new System.IO.StreamReader(assembly.GetManifestResourceStream(resourceName)));
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
