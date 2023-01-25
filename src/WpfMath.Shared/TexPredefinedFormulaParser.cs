using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using WpfMath.Data;
using WpfMath.Parsers.PredefinedFormulae;
using WpfMath.Rendering;
using WpfMath.Utils;

namespace WpfMath
{
    // Parses definitions of predefined formulas from XML file.
    internal class TexPredefinedFormulaParser
    {
        private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedTexFormulas.xml";

        private readonly IDictionary<string, Type> typeMappings;
        private readonly IDictionary<string, IArgumentValueParser> argValueParsers;
        private readonly IDictionary<string, IActionParser> actionParsers;

        private Type[] GetArgumentTypes(IEnumerable<XElement> args)
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

        private object?[] GetArgumentValues(IEnumerable<XElement> args, PredefinedFormulaContext context)
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

        private XElement rootElement;

        public TexPredefinedFormulaParser(IBrushFactory brushFactory)
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

            actionParsers.Add("CreateFormula", new CreateTeXFormulaParser(this, brushFactory));
            actionParsers.Add("MethodInvocation", new MethodInvocationParser(this, brushFactory));
            actionParsers.Add("Return", new ReturnParser());

            argValueParsers.Add("Formula", new TeXFormulaValueParser());
            argValueParsers.Add("string", new StringValueParser());
            argValueParsers.Add("double", new DoubleValueParser());
            argValueParsers.Add("int", new IntValueParser());
            argValueParsers.Add("bool", new BooleanValueParser());
            argValueParsers.Add("char", new CharValueParser());
            argValueParsers.Add("Unit", new EnumParser(typeof(TexUnit)));
            argValueParsers.Add("AtomType", new EnumParser(typeof(TexAtomType)));

            using var resource = typeof(WpfMathResourceMarker).Assembly.ReadResource(resourceName);
            var doc = XDocument.Load(resource);
            this.rootElement = doc.Root;
        }

        // TODO[#339]: Review this API
        public void Parse(Dictionary<string, Func<SourceSpan, TexFormula?>> predefinedTeXFormulas)
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
                        predefinedTeXFormulas.Add(formulaName, source => this.ParseFormula(source, formulaElement, predefinedTeXFormulas));
                    }
                }
            }
        }

        private TexFormula? ParseFormula(SourceSpan source, XElement formulaElement, Dictionary<string, Func<SourceSpan, TexFormula?>> allFormulas)
        {
            var context = new PredefinedFormulaContext();
            foreach (var element in formulaElement.Elements())
            {
                var parser = actionParsers[element.Name.ToString()];
                if (parser == null)
                    continue;

                parser.Parse(source, element, context, allFormulas);
                if (parser is ReturnParser)
                    return ((ReturnParser)parser).Result;
            }
            return null;
        }

        private record MethodInvocationParser(TexPredefinedFormulaParser Parent, IBrushFactory BrushFactory) : IActionParser
        {
            public void Parse(
                SourceSpan source,
                XElement element,
                PredefinedFormulaContext context,
                Dictionary<string, Func<SourceSpan, TexFormula?>> allFormulas)
            {
                var methodName = element.AttributeValue("name");
                var objectName = element.AttributeValue("formula");
                var args = element.Elements("Argument");

                var formula = context[objectName];
                Debug.Assert(formula != null);

                var argTypes = Parent.GetArgumentTypes(args);
                var argValues = Parent.GetArgumentValues(args, context);

                var helper = new TexFormulaHelper(
                    formula,
                    source,
                    BrushFactory,
                    allFormulas);
                var methodInvocation = typeof(TexFormulaHelper).GetMethod(methodName, argTypes)!;

                methodInvocation.Invoke(helper, argValues);
            }
        }

        private record CreateTeXFormulaParser(TexPredefinedFormulaParser Parent, IBrushFactory BrushFactory) : IActionParser
        {
            public void Parse(
                SourceSpan source,
                XElement element,
                PredefinedFormulaContext context,
                Dictionary<string, Func<SourceSpan, TexFormula?>> allFormulas)
            {
                var name = element.AttributeValue("name");
                var args = element.Elements("Argument");

                var argValues = Parent.GetArgumentValues(args, context);

                Debug.Assert(argValues.Length == 1 || argValues.Length == 0);
                TexFormula formula;
                if (argValues.Length == 1)
                {
                    var parser = new TexFormulaParser(BrushFactory, allFormulas);
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

            public void Parse(
                SourceSpan source,
                XElement element,
                PredefinedFormulaContext context,
                Dictionary<string, Func<SourceSpan, TexFormula?>> allFormulas)
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
            public object? Parse(string value, PredefinedFormulaContext context)
            {
                if (value == null)
                    return null;

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
            void Parse(
                SourceSpan source,
                XElement element,
                PredefinedFormulaContext context,
                Dictionary<string, Func<SourceSpan, TexFormula?>> allFormulas);
        }

        private interface IArgumentValueParser
        {
            object? Parse(string value, PredefinedFormulaContext context);
        }
    }
}
