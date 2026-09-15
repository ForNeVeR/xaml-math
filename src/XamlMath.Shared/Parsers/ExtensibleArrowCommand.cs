using XamlMath.Atoms;
using XamlMath.Boxes;
using XamlMath.Exceptions;

namespace XamlMath.Parsers;

internal enum ExtensibleArrowDirection
{
    Right,
    Left
}

/// <summary>Parser for \xrightarrow{label} and \xleftarrow{label} commands.</summary>
internal sealed class ExtensibleArrowCommand : ICommandParser
{
    private readonly ExtensibleArrowDirection _direction;

    public ExtensibleArrowCommand(ExtensibleArrowDirection direction)
    {
        _direction = direction;
    }

    public CommandProcessingResult ProcessCommand(CommandContext context)
    {
        var source = context.CommandSource;
        var position = context.ArgumentsStartPosition;
        var start = context.CommandNameStartPosition;

        var afterLabel = TexFormulaParser.ReadElement(source, position);
        position = afterLabel.position;

        var labelFormula = context.Parser.Parse(
            afterLabel.source,
            context.Formula.TextStyle,
            context.Environment.CreateChildEnvironment());

        var atomSource = source.Segment(start, position - start);
        var atom = new ExtensibleArrowAtom(atomSource, labelFormula.RootAtom, _direction);

        return new CommandProcessingResult(atom, position);
    }
}
