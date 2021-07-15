using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using WpfMath.Parsers.PredefinedFormulae;

namespace WpfMath
{
    // Parses definitions of predefined formulas from XML file.
    internal class TexPredefinedFormulaParser
    {
        private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedTexFormulas.xml";

        private static readonly IDictionary<string, Type> typeMappings;
        private static readonly IDictionary<string, IArgumentValueParser> argValueParsers;
        private static readonly IDictionary<string, IActionParser> actionParsers;

        static TexPredefinedFormulaParser()
        {
            typeMappings = new Dictionary<string, Type>();
            argValueParsers = new Dictionary<string, IArgumentValueParser>();
            actionParsers = new Dictionary<string, IActionParser>();

            typeMappings.Add("Formula", typeof(TexFormula));
            typeMappings.Add("string", typeof(string));
            typeMappings.Add("double", typeof(double));
            typeMappings.Add("int", typeof(int));
            typeMappings.Add("bool", typeof(bool));
            typeMappings.Add("char", typeof(char));
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

                var parser = argValueParsers[typeName];
                result.Add(parser.Parse(value, context));
            }

            return result.ToArray();
        }

        private readonly XElement rootElement;

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

        private TexFormula? ParseFormula(SourceSpan source, XElement formulaElement)
        {
            var context = new PredefinedFormulaContext();
            foreach (var element in formulaElement.Elements())
            {
                if (!actionParsers.TryGetValue(element.Name.ToString(), out var parser))
                    continue;

                parser.Parse(source, element, context);
                if (parser is ReturnParser returnParser)
                    return returnParser.Result;
            }
            return null;
        }

        private class MethodInvocationParser : IActionParser
        {
            public void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context)
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

        private class CreateTeXFormulaParser : IActionParser
        {
            public void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context)
            {
                var name = element.AttributeValue("name");
                var args = element.Elements("Argument");

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

        private class ReturnParser : IActionParser
        {
            public TexFormula? Result
            {
                get;
                private set;
            }

            public void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context)
            {
                var name = element.AttributeValue("name");
                var result = context[name];
                Debug.Assert(result != null);
                this.Result = result;
            }
        }

        private class DoubleValueParser : IArgumentValueParser
        {
            public object Parse(string value, PredefinedFormulaContext context)
            {
                return double.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        private class CharValueParser : IArgumentValueParser
        {
            public object Parse(string value, PredefinedFormulaContext context)
            {
                Debug.Assert(value.Length == 1);
                return value[0];
            }
        }

        private class BooleanValueParser : IArgumentValueParser
        {
            public object Parse(string value, PredefinedFormulaContext context)
            {
                return bool.Parse(value);
            }
        }

        private class IntValueParser : IArgumentValueParser
        {
            public object Parse(string value, PredefinedFormulaContext context)
            {
                return int.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        private class StringValueParser : IArgumentValueParser
        {
            public object Parse(string value, PredefinedFormulaContext context)
            {
                return value;
            }
        }

        private class TeXFormulaValueParser : IArgumentValueParser
        {
            public object Parse(string value, PredefinedFormulaContext context)
            {
                var formula = context[value];
                Debug.Assert(formula != null);
                return formula;
            }
        }

        private class EnumParser : IArgumentValueParser
        {
            private Type enumType;

            public EnumParser(Type enumType)
            {
                this.enumType = enumType;
            }

            public object Parse(string value, PredefinedFormulaContext context)
            {
                return Enum.Parse(this.enumType, value);
            }
        }

        private interface IActionParser
        {
            void Parse(SourceSpan source, XElement element, PredefinedFormulaContext context);
        }

        private interface IArgumentValueParser
        {
            object? Parse(string value, PredefinedFormulaContext context);
        }
    }
}
