using WpfMath.Atoms;

namespace WpfMath.Parsers;

internal record EnvironmentContext(
    TexFormulaParser Parser,
    TexFormula Formula,
    ICommandEnvironment Environment,
    SourceSpan EnvironmentSource,
    SourceSpan EnvironmentBodySource
);

internal record EnvironmentProcessingResult(
    Atom? Atom,
    AtomAppendMode AppendMode = AtomAppendMode.Add
);

internal interface IEnvironmentParser
{
    EnvironmentProcessingResult ProcessEnvironment(EnvironmentContext context);
}
