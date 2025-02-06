using XamlMath.Exceptions;

namespace XamlMath.Parsers;

/// <summary>This command will process a <code>\begin … \end</code> environment block.</summary>
internal sealed class ProcessEnvironmentCommand : ICommandParser
{
    public CommandProcessingResult ProcessCommand(CommandContext context)
    {
        var position = context.ArgumentsStartPosition;
        var afterElement = TexFormulaParser.ReadElement(context.CommandSource, position);
        position = afterElement.position;
        var environmentName = afterElement.source.ToString();
        if (environmentName == "") throw new TexParseException(@"Empty environment name for the \begin command.");
        if (!StandardCommands.Environments.TryGetValue(environmentName, out var environmentParser))
            throw new TexParseException(@$"Unknown environment name for the \begin command: ""{environmentName}"".");

        var environmentContext = GetEnvironmentContext(context, environmentName, ref position);
        var result = environmentParser.ProcessEnvironment(environmentContext);

        return new CommandProcessingResult(result.Atom, position, result.AppendMode);
    }

    private static EnvironmentContext GetEnvironmentContext(
        CommandContext commandContext,
        string environmentName,
        ref int position)
    {
        var bodyStartPosition = position;
        var bodyEndPosition = bodyStartPosition;
        var nestingLevel = 1;
        var source = commandContext.CommandSource;
        while (position < source.Length)
        {
            bodyEndPosition = position;

            var afterRead = TexFormulaParser.ReadElement(source, position);
            position = afterRead.position;
            var element = afterRead.source;
            switch (element.ToString())
            {
                case @"\begin": ++nestingLevel; break;
                case @"\end": --nestingLevel; break;
            }

            if (nestingLevel == 0) break;
        }

        if (nestingLevel != 0)
            throw new TexParseException(@$"No matching \end found for command ""\begin{{{environmentName}}}"".");

        var afterEnd = TexFormulaParser.ReadElement(source, position);
        position = afterEnd.position;
        var endLabel = afterEnd.source.ToString();
        if (endLabel != environmentName)
            throw new TexParseException(
                $@"""\end{{{endLabel}}}"" doesn't correspond to earlier ""\begin{{{environmentName}}}"".");

        var environmentSource = source.Segment(commandContext.CommandNameStartPosition, position - commandContext.CommandNameStartPosition);
        var body = source.Segment(bodyStartPosition, bodyEndPosition - bodyStartPosition);

        return new EnvironmentContext(
            commandContext.Parser,
            commandContext.Formula,
            commandContext.Environment,
            environmentSource,
            body);
    }
}
