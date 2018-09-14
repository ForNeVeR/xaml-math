using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WpfMath
{
    internal static class XmlUtilities
    {
        public static bool AttributeBooleanValue(this XElement element, string attributeName, bool? defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                if (defaultValue != null)
                    return defaultValue.Value;
                throw new InvalidOperationException();
            }
            return bool.Parse(attribute.Value);
        }

        public static int AttributeInt32Value(this XElement element, string attributeName, int? defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                if (defaultValue != null)
                    return defaultValue.Value;
                throw new InvalidOperationException();
            }
            return int.Parse(attribute.Value, CultureInfo.InvariantCulture);
        }

        public static double AttributeDoubleValue(this XElement element, string attributeName, double? defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                if (defaultValue != null)
                    return defaultValue.Value;
                throw new InvalidOperationException();
            }
            return double.Parse(attribute.Value, CultureInfo.InvariantCulture);
        }

        public static string AttributeValue(this XElement element, string attributeName, string defaultValue = null)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                if (defaultValue != null)
                    return defaultValue;
                throw new InvalidOperationException();
            }
            return attribute.Value;
        }
        
        /// <summary>
        /// Returns a value that specifies if the <paramref name="inputNode"/> contains the given <paramref name="attribute"/>.
        /// </summary>
        /// <param name="inputNode">The <see cref="XmlNode"/> used to search for the <see cref="XmlAttribute"/>.</param>
        /// <param name="elementName"></param>
        /// <param name="attribute">The <see cref="XmlAttribute"/> to search for.</param>
        /// <returns></returns>
        public static bool Attribute_Exists(this XmlNode inputNode, string elementName, string attribute)
        {
            int i = 0;
            if (inputNode.Name == elementName && inputNode.Attributes.Count > 0)
            {
                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    if (item.Name == attribute)
                    {
                        i += 1;
                    }
                    else
                    {
                        continue;
                    }
                }
                return i > 0;
            }
            else
            {
                if (inputNode.Name != elementName)
                {
                    throw new InvalidOperationException($"The element: {inputNode.Name} is not the same as {elementName}.");
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the value of the specified <paramref name="attribute"/>.
        /// </summary>
        /// <param name="inputNode"></param>
        /// <param name="elementName"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string Attribute_Value(this XmlNode inputNode, string attribute,string defaultvalue=null)
        {
            string attributeValue = "";
            if ( inputNode.Attributes.Count > 0)
            {

                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    if (item.Name == attribute)
                    {
                        attributeValue = item.Value;
                    }
                    else
                    {
                        continue;
                    }
                }
                if (attributeValue.Length==0)
                {
                    attributeValue = defaultvalue;
                }
                return attributeValue;
            }
            else
            {

                attributeValue = defaultvalue;
                return attributeValue;
            }
        }

        public static int Attribute_IntValue(this XmlNode inputNode, string attribute, int defaultvalue=0)
        {
            int attributeValue = 0;
            if ( inputNode.Attributes.Count > 0)
            {

                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    if (item.Name == attribute)
                    {
                        if (int.TryParse(item.Value,out int result))
                        {
                            attributeValue = result;
                        }
                        
                    }
                    else
                    {
                        continue;
                    }
                }
                if (attributeValue == 0)
                {
                    attributeValue = defaultvalue;
                }
                return attributeValue;
            }
            else
            {
                attributeValue = defaultvalue;
                return attributeValue;
            }
        }

        public static double Attribute_DoubleValue(this XmlNode inputNode, string attribute, double defaultvalue=0.0)
        {
            double attributeValue = 0.0;
            if ( inputNode.Attributes.Count > 0)
            {

                foreach (XmlAttribute item in inputNode.Attributes)
                {
                    if (item.Name == attribute)
                    {
                        if (double.TryParse(item.Value, out double result))
                        {
                            attributeValue = result;
                        }

                    }
                    else
                    {
                        continue;
                    }
                }
                if (attributeValue == 0.0)
                {
                    attributeValue = defaultvalue;
                }
                return attributeValue;
            }
            else
            {
                attributeValue = defaultvalue;
                return attributeValue;
            }
        }

        public static bool Attribute_BoolValue(this XmlNode inputNode, string attribute)
        {
            bool attributeValue = false;
            foreach (XmlAttribute item in inputNode.Attributes)
            {
                if (item.Name == attribute)
                {
                    if (item.Value.ToLower() == "true")
                    {
                        attributeValue = true;
                    }
                    else
                    {
                        attributeValue = false;
                    }
                }
                else
                {
                    continue;
                }
            }

            return attributeValue;
        }

        public static XmlNode GetXmlNode(this XmlNode inputNode,string elementName)
        {
            XmlNode resultNode = null;
            if (inputNode.HasChildNodes)
            {
                foreach (XmlNode item in inputNode.ChildNodes)
                {
                    if (item.Name==elementName)
                    {
                        resultNode = item.Clone();
                    }
                }
                return resultNode;
            }
            else
            {
                return null;
                //throw new InvalidOperationException("Cannot retrieve an xml node from a node that has no child nodes.");
            }
        }

        public static List<XmlNode> GetXmlNodes(this XmlNode inputNode, string elementName)
        {
            List<XmlNode> resultNodes = new List<XmlNode>();
            
            foreach (XmlNode item in inputNode.ChildNodes)
            {
                if (item!=null&&item.Name == elementName)
                {
                    resultNodes.Add ( item.Clone());
                }
            }
            return resultNodes;
        }

        public static List<XmlNode> GetXmlNodes(this XmlNode inputNode)
        {
            List<XmlNode> resultNodes = new List<XmlNode>();

            foreach (XmlNode item in inputNode.ChildNodes)
            {
                resultNodes.Add(item.Clone());
            }
            return resultNodes;
        }
        
    }
}
