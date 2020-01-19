using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;

namespace WpfMath
{
    // Parses definitions of predefined formulas from XML file.
    internal class TexPredefinedFormulaParser
    {
        public static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedTexFormulas.xml";

        private static IDictionary<string, Type> typeMappings;
        private static IDictionary<string, ArgumentValueParser> argValueParsers;
        private static IDictionary<string, ActionParser> actionParsers;
        private static TexFormulaParser formulaParser;

        static TexPredefinedFormulaParser()
        {
            typeMappings = new Dictionary<string, Type>();
            argValueParsers = new Dictionary<string, ArgumentValueParser>();
            actionParsers = new Dictionary<string, ActionParser>();
            formulaParser = new TexFormulaParser();
            var sharedCacheFormulas = new Dictionary<string, TexFormula>();

            typeMappings.Add("Formula", typeof(TexFormula));
            typeMappings.Add("string", typeof(string));
            typeMappings.Add("double", typeof(double));
            typeMappings.Add("int", typeof(int));
            typeMappings.Add("bool", typeof(bool));
            typeMappings.Add("char", typeof(char));
            typeMappings.Add("Color", typeof(Color));
            typeMappings.Add("Unit", typeof(TexUnit));
            typeMappings.Add("AtomType", typeof(TexAtomType));

            actionParsers.Add("CreateFormula", new CreateTeXFormulaParser(sharedCacheFormulas));
            actionParsers.Add("MethodInvocation", new MethodInvocationParser(sharedCacheFormulas));
            actionParsers.Add("Return", new ReturnParser(sharedCacheFormulas));

            argValueParsers.Add("Formula", new TeXFormulaValueParser(sharedCacheFormulas));
            argValueParsers.Add("string", new StringValueParser(sharedCacheFormulas));
            argValueParsers.Add("double", new DoubleValueParser(sharedCacheFormulas));
            argValueParsers.Add("int", new IntValueParser(sharedCacheFormulas));
            argValueParsers.Add("bool", new BooleanValueParser(sharedCacheFormulas));
            argValueParsers.Add("char", new CharValueParser(sharedCacheFormulas));
            argValueParsers.Add("Color", new ColorConstantValueParser(sharedCacheFormulas));
            argValueParsers.Add("Unit", new EnumParser(typeof(TexUnit), sharedCacheFormulas));
            argValueParsers.Add("AtomType", new EnumParser(typeof(TexAtomType), sharedCacheFormulas));
        }

        private static Type[] GetArgumentTypes(IEnumerable<XElement> args)
        {
            var result = new List<Type>();
            foreach (var curArg in args)
            {
                var typeName = curArg.AttributeValue("type");
                var type = typeMappings[typeName];
                Debug.Assert(type != null);
                result.Add(type);
            }

            return result.ToArray();
        }

        private static object[] GetArgumentValues(IEnumerable<XElement> args)
        {
            var result = new List<object>();
            foreach (var curArg in args)
            {
                var typeName = curArg.AttributeValue("type");
                var value = curArg.AttributeValue("value");

                var parser = ((ArgumentValueParser)argValueParsers[typeName]);
                result.Add(parser.Parse(value, typeName));
            }

            return result.ToArray();
        }

        private XElement rootElement;

        public TexPredefinedFormulaParser()
        {
            var doc = XDocument.Load(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)));
            this.rootElement = doc.Root;
        }

        public void Parse(IDictionary<string, Func<SourceSpan, TexFormula>> predefinedTeXFormulas)
        {
            var rootEnabled = rootElement.AttributeBooleanValue("enabled", true);
            if (rootEnabled)
            {
                foreach (var formulaElement in rootElement.Elements("Formula"))
                {
                    var enabled = formulaElement.AttributeBooleanValue("enabled", true);
                    if (enabled)
                    {
                        var formulaName = formulaElement.AttributeValue("name");
                        predefinedTeXFormulas.Add(formulaName, source => this.ParseFormula(source, formulaElement));
                    }
                }
            }
        }

        public TexFormula ParseFormula(SourceSpan source, XElement formulaElement)
        {
            foreach (var element in formulaElement.Elements())
            {
                var parser = actionParsers[element.Name.ToString()];
                if (parser == null)
                    continue;

                parser.Parse(source, element);
                if (parser is ReturnParser)
                    return ((ReturnParser)parser).Result;
            }
            return null;
        }

        public class MethodInvocationParser : ActionParser
        {
            public MethodInvocationParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override void Parse(SourceSpan source, XElement element)
            {
                var methodName = element.AttributeValue("name");
                var objectName = element.AttributeValue("formula");
                var args = element.Elements("Argument");

                var formula = this.SharedCacheFormulas[objectName];
                Debug.Assert(formula != null);

                var argTypes = GetArgumentTypes(args);
                var argValues = GetArgumentValues(args);

                var helper = new TexFormulaHelper(formula, source);
                typeof(TexFormulaHelper).GetMethod(methodName, argTypes).Invoke(helper, argValues);
            }
        }

        public class CreateTeXFormulaParser : ActionParser
        {
            public CreateTeXFormulaParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override void Parse(SourceSpan source, XElement element)
            {
                var name = element.AttributeValue("name");
                var args = element.Elements("Argument");

                var argTypes = GetArgumentTypes(args);
                var argValues = GetArgumentValues(args);

                Debug.Assert(argValues.Length == 1 || argValues.Length == 0);
                TexFormula formula = null;
                if (argValues.Length == 1)
                {
                    var parser = new TexFormulaParser();
                    formula = parser.Parse((string)argValues[0]);
                }
                else
                {
                    formula = new TexFormula { Source = source };
                }

                this.SharedCacheFormulas.Add(name, formula);
            }
        }

        public class ReturnParser : ActionParser
        {
            public ReturnParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public TexFormula Result
            {
                get;
                private set;
            }

            public override void Parse(SourceSpan source, XElement element)
            {
                var name = element.AttributeValue("name");
                var result = this.SharedCacheFormulas[name];
                Debug.Assert(result != null);
                this.Result = result;
                this.SharedCacheFormulas.Clear();
            }
        }

        public class DoubleValueParser : ArgumentValueParser
        {
            public DoubleValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override object Parse(string value, string type)
            {
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        public class CharValueParser : ArgumentValueParser
        {
            public CharValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override object Parse(string value, string type)
            {
                Debug.Assert(value.Length == 1);
                return value[0];
            }
        }

        public class BooleanValueParser : ArgumentValueParser
        {
            public BooleanValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override object Parse(string value, string type)
            {
                return bool.Parse(value);
            }
        }

        public class IntValueParser : ArgumentValueParser
        {
            public IntValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override object Parse(string value, string type)
            {
                return int.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        public class StringValueParser : ArgumentValueParser
        {
            public StringValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override object Parse(string value, string type)
            {
                return value;
            }
        }

        public class TeXFormulaValueParser : ArgumentValueParser
        {
            public TeXFormulaValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override object Parse(string value, string type)
            {
                if (value == null)
                    return null;

                var formula = this.SharedCacheFormulas[value];
                Debug.Assert(formula != null);
                return (TexFormula)formula;
            }
        }

        public class ColorConstantValueParser : ArgumentValueParser
        {
            public ColorConstantValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
            }

            public override object Parse(string value, string type)
            {
                return typeof(Color).GetField(value).GetValue(null);
            }
        }

        public class EnumParser : ArgumentValueParser
        {
            private Type enumType;

            public EnumParser(Type enumType, IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {
                this.enumType = enumType;
            }

            public override object Parse(string value, string type)
            {
                return Enum.Parse(this.enumType, value);
            }
        }

        public abstract class ActionParser : ParserBase
        {
            protected  ActionParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {}

            public abstract void Parse(SourceSpan source, XElement element);
        }

        public abstract class ArgumentValueParser : ParserBase
        {
            protected ArgumentValueParser(IDictionary<string, TexFormula> sharedCacheFormulas)
                : base(sharedCacheFormulas)
            {}

            public abstract object Parse(string value, string type);
        }

        public abstract class ParserBase
        {
            public ParserBase(IDictionary<string, TexFormula> sharedCacheFormulas)
            {
                this.SharedCacheFormulas = sharedCacheFormulas;
            }

            public IDictionary<string, TexFormula> SharedCacheFormulas { get; }
        }
    }
}
