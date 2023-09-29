using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using XamlMath.Data;
using XamlMath.Parsers.PredefinedFormulae;
using XamlMath.Rendering;
using XamlMath.Utils;

namespace XamlMath;

// Parses definitions of predefined formulas from XML file.
internal sealed class TexPredefinedFormulaParser
{
    private static readonly string resourceName = TexUtilities.ResourcesDataDirectory + "PredefinedTexFormulas.xml";

    private static readonly IReadOnlyDictionary<string, Type> typeMappings;
    private static readonly IReadOnlyDictionary<string, IArgumentValueParser> argValueParsers;

    private readonly IReadOnlyDictionary<string, IActionParser> actionParsers;

    private IEnumerable<Type> GetArgumentTypes(IEnumerable<XElement> args)
    {
        foreach (var curArg in args)
        {
            var typeName = curArg.AttributeValue("type");
            var type = typeMappings[typeName];
            Debug.Assert(type != null);
            yield return type;
        }
    }

    private IEnumerable<object?> GetArgumentValues(IEnumerable<XElement> args, PredefinedFormulaContext context)
    {
        foreach (var curArg in args)
        {
            var typeName = curArg.AttributeValue("type");
            var value = curArg.AttributeValue("value");
            var parser = argValueParsers[typeName];
            yield return parser.Parse(value, context);
        }
    }

    private readonly XElement rootElement;

    static TexPredefinedFormulaParser()
    {
        typeMappings = new Dictionary<string, Type>
        {
            ["Formula"] = typeof(TexFormula),
            ["string"] = typeof(string),
            ["double"] = typeof(double),
            ["int"] = typeof(int),
            ["bool"] = typeof(bool),
            ["char"] = typeof(char),
            ["Unit"] = typeof(TexUnit),
            ["AtomType"] = typeof(TexAtomType),
        };

        argValueParsers = new Dictionary<string, IArgumentValueParser>
        {
            ["Formula"] = new TeXFormulaValueParser(),
            ["string"] = new StringValueParser(),
            ["double"] = new DoubleValueParser(),
            ["int"] = new IntValueParser(),
            ["bool"] = new BooleanValueParser(),
            ["char"] = new CharValueParser(),
            ["Unit"] = new EnumParser(typeof(TexUnit)),
            ["AtomType"] = new EnumParser(typeof(TexAtomType)),
        };
    }

    public TexPredefinedFormulaParser(IBrushFactory brushFactory)
    {

        actionParsers = new Dictionary<string, IActionParser>
        {
            ["CreateFormula"] = new CreateTeXFormulaParser(this, brushFactory),
            ["MethodInvocation"] = new MethodInvocationParser(this, brushFactory),
            ["Return"] = new ReturnParser(),
        };

        using var resource = typeof(XamlMathResourceMarker).Assembly.ReadResource(resourceName);
        var doc = XDocument.Load(resource);
        this.rootElement = doc.Root;
    }

    // TODO[#339]: Review this API
    public void Parse(Dictionary<string, Func<SourceSpan, TexFormula?>> predefinedTeXFormulas)
    {
        var rootEnabled = rootElement.AttributeBooleanValue("enabled", true);

        if (!rootEnabled)
            return;

        foreach (var formulaElement in rootElement.Elements("Formula"))
        {
            var enabled = formulaElement.AttributeBooleanValue("enabled", true);

            if (!enabled)
                continue;

            var formulaName = formulaElement.AttributeValue("name");
            predefinedTeXFormulas.Add(formulaName, source => this.ParseFormula(source, formulaElement, predefinedTeXFormulas));
        }
    }

    private TexFormula? ParseFormula(SourceSpan source, XElement formulaElement, Dictionary<string, Func<SourceSpan, TexFormula?>> allFormulas)
    {
        var context = new PredefinedFormulaContext();
        foreach (var element in formulaElement.Elements())
        {
            if (!actionParsers.TryGetValue(element.Name.ToString(), out var parser))
                continue;
            parser.Parse(source, element, context, allFormulas);
            if (parser is ReturnParser returnParser)
                return returnParser.Result;
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

            var argTypes = Parent.GetArgumentTypes(args).ToArray();
            var argValues = Parent.GetArgumentValues(args, context).ToArray();

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

            var argValues = Parent.GetArgumentValues(args, context).ToArray();

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

    private sealed class ReturnParser : IActionParser
    {
        public TexFormula? Result { get; private set; }

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

    private sealed class DoubleValueParser : IArgumentValueParser
    {
        public object Parse(string value, PredefinedFormulaContext context)
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }
    }

    private sealed class CharValueParser : IArgumentValueParser
    {
        public object Parse(string value, PredefinedFormulaContext context)
        {
            Debug.Assert(value.Length == 1);
            return value[0];
        }
    }

    private sealed class BooleanValueParser : IArgumentValueParser
    {
        public object Parse(string value, PredefinedFormulaContext context)
        {
            return bool.Parse(value);
        }
    }

    private sealed class IntValueParser : IArgumentValueParser
    {
        public object Parse(string value, PredefinedFormulaContext context)
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }
    }

    private sealed class StringValueParser : IArgumentValueParser
    {
        public object Parse(string value, PredefinedFormulaContext context)
        {
            return value;
        }
    }

    private sealed class TeXFormulaValueParser : IArgumentValueParser
    {
        public object Parse(string value, PredefinedFormulaContext context)
        {
            var formula = context[value];
            Debug.Assert(formula != null);
            return formula;
        }
    }

    private sealed class EnumParser : IArgumentValueParser
    {
        private readonly Type enumType;

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
