using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Xml.Linq;
using WpfMath.Parsers.PredefinedFormulae;

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

            typeMappings.Add("Formula", typeof(TexFormula));
            typeMappings.Add("string", typeof(string));
            typeMappings.Add("double", typeof(double));
            typeMappings.Add("int", typeof(int));
            typeMappings.Add("bool", typeof(bool));
            typeMappings.Add("char", typeof(char));
            typeMappings.Add("Color", typeof(Color));
            typeMappings.Add("Unit", typeof(TexUnit));
            typeMappings.Add("AtomType", typeof(TexAtomType));

            actionParsers.Add("CreateFormula", new CreateTeXFormulaParser());
            actionParsers.Add("MethodInvocation", new MethodInvocationParser());
            actionParsers.Add("Return", new ReturnParser());

            argValueParsers.Add("Formula", new TeXFormulaValueParser());
            argValueParsers.Add("string", new StringValueParser());
            argValueParsers.Add("double", new DoubleValueParser());
            argValueParsers.Add("int", new IntValueParser());
            argValueParsers.Add("bool", new BooleanValueParser());
            argValueParsers.Add("char", new CharValueParser());
            argValueParsers.Add("Color", new ColorConstantValueParser());
            argValueParsers.Add("Unit", new EnumParser(typeof(TexUnit)));
            argValueParsers.Add("AtomType", new EnumParser(typeof(TexAtomType)));
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

        private static object?[] GetArgumentValues(IEnumerable<XElement> args, PredefinedFormulaContext context)
        {
            var result = new List<object?>();
            foreach (var curArg in args)
            {
                var typeName = curArg.AttributeValue("type");
                var value = curArg.AttributeValue("value");

                var parser = ((ArgumentValueParser)argValueParsers[typeName]);
                result.Add(parser.Parse(value, typeName, context));
            }

            return result.ToArray();
        }

        private XElement rootElement;

        public TexPredefinedFormulaParser()
        {
            var doc = XDocument.Load(new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!));
            this.rootElement = doc.Root;
        }

        public void Parse(IDictionary<string, Func<SourceSpan, TexFormula?>> predefinedTeXFormulas)
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

        public TexFormula? ParseFormula(SourceSpan source, XElement formulaElement)
        {
            var context = new PredefinedFormulaContext();
            foreach (var element in formulaElement.Elements())
            {
                var parser = actionParsers[element.Name.ToString()];
                if (parser == null)
                    continue;

                parser.Parse(source, element, context);
                if (parser is ReturnParser)
                    return ((ReturnParser)parser).Result;
            }
            return null;
        }

        public class MethodInvocationParser : ActionParser
        {
            public override void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context)
            {
                var methodName = element.AttributeValue("name");
                var objectName = element.AttributeValue("formula");
                var args = element.Elements("Argument");

                var formula = context[objectName];
                Debug.Assert(formula != null);

                var argTypes = GetArgumentTypes(args);
                var argValues = GetArgumentValues(args, context);

                var helper = new TexFormulaHelper(formula, source);
                var methodInvocation = typeof(TexFormulaHelper).GetMethod(methodName, argTypes)!;

                methodInvocation.Invoke(helper, argValues);
            }
        }

        public class CreateTeXFormulaParser : ActionParser
        {
            public override void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context)
            {
                var name = element.AttributeValue("name");
                var args = element.Elements("Argument");

                var argTypes = GetArgumentTypes(args);
                var argValues = GetArgumentValues(args, context);

                Debug.Assert(argValues.Length == 1 || argValues.Length == 0);
                TexFormula formula;
                if (argValues.Length == 1)
                {
                    var parser = new TexFormulaParser();
                    formula = parser.Parse((string)argValues[0]!); // Nullable TODO: This might need null checking
                }
                else
                {
                    formula = new TexFormula { Source = source };
                }

                context.AddFormula(name, formula);
            }
        }

        public class ReturnParser : ActionParser
        {
            public TexFormula? Result
            {
                get;
                private set;
            }

            public override void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context)
            {
                var name = element.AttributeValue("name");
                var result = context[name];
                Debug.Assert(result != null);
                this.Result = result;
            }
        }

        public class DoubleValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type, PredefinedFormulaContext context)
            {
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        public class CharValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type, PredefinedFormulaContext context)
            {
                Debug.Assert(value.Length == 1);
                return value[0];
            }
        }

        public class BooleanValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type, PredefinedFormulaContext context)
            {
                return bool.Parse(value);
            }
        }

        public class IntValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type, PredefinedFormulaContext context)
            {
                return int.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        public class StringValueParser : ArgumentValueParser
        {
            public override object Parse(string value, string type, PredefinedFormulaContext context)
            {
                return value;
            }
        }

        public class TeXFormulaValueParser : ArgumentValueParser
        {
            public override object? Parse(string value, string type, PredefinedFormulaContext context)
            {
                if (value == null)
                    return null;

                var formula = context[value];
                Debug.Assert(formula != null);
                return formula;
            }
        }

        public class ColorConstantValueParser : ArgumentValueParser
        {
            public override object? Parse(string value, string type, PredefinedFormulaContext context)
            {
                return typeof(Color).GetField(value)!.GetValue(null);
            }
        }

        public class EnumParser : ArgumentValueParser
        {
            private Type enumType;

            public EnumParser(Type enumType)
            {
                this.enumType = enumType;
            }

            public override object Parse(string value, string type, PredefinedFormulaContext context)
            {
                return Enum.Parse(this.enumType, value);
            }
        }

        public abstract class ActionParser
        {
            public abstract void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context);
        }

        public abstract class ArgumentValueParser
        {
            public abstract object? Parse(string value, string type, PredefinedFormulaContext context);
        }
    }
}
