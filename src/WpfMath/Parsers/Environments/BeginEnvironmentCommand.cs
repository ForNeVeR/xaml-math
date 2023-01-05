using WpfMath.Exceptions;

namespace WpfMath.Parsers.Environments;

/// <summary>
/// This command will parse a \begin \end environment. Doesn't support nested environments
/// </summary>
internal class BeginEnvironmentCommand : ICommandParser
{
    public CommandProcessingResult ProcessCommand(CommandContext context)
    {
        var position = context.ArgumentsStartPosition;
        var environmentName = TexFormulaParser.ReadElement(context.CommandSource, ref position).ToString();
        if (environmentName == "") throw new TexParseException(@"Empty environment name for the \begin command.");
        if (!StandardCommands.Environments.TryGetValue(environmentName, out var childParser))
            throw new TexParseException(@$"Unknown environment name for the \begin command: ""{environmentName}"".");

        var childEnvironment = GetChildEnvironment(environmentName, context);
        var childContext = context with
        {
            ArgumentsStartPosition = position,
            Environment = childEnvironment,

            // Not quite true but the best we can do: the command "begins" at the beginning of the argument name.
            CommandNameStartPosition = context.ArgumentsStartPosition
        };
        var result = childParser.ProcessCommand(childContext);
        if (!childEnvironment.Ended)
            throw new TexParseException(
                $@"No corresponding \end command encountered for environment ""{environmentName}"".");
        return result;
    }

    private EndEnvironment GetChildEnvironment(string name, CommandContext context)
    {
        var baseEnv = context.Environment.CreateChildEnvironment();
        return new EndEnvironment(name, baseEnv);
    }
}
