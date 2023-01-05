using System.Collections.Generic;
using WpfMath.Exceptions;

namespace WpfMath.Parsers.Environments;

/// <summary>
/// An environment corresponding to a <c>\begin</c> command invocation, containing a command to end it.
/// </summary>
internal class EndEnvironment : ICommandEnvironment
{
    private readonly string _name;

    public EndEnvironment(
        string name,
        ICommandEnvironment baseEnvironment
    )
    {
        _name = name;

        var availableCommands = new Dictionary<string, ICommandParser>(
            baseEnvironment.AvailableCommands.Count + 1);
        foreach (var command in baseEnvironment.AvailableCommands)
            availableCommands.Add(command.Key, command.Value);
        availableCommands["end"] = new EndCommand(this);
        AvailableCommands = availableCommands;
    }

    public bool Ended { get; private set; }

    public IReadOnlyDictionary<string, ICommandParser> AvailableCommands { get; }

    public ICommandEnvironment CreateChildEnvironment() => this;

    public bool ProcessUnknownCharacter(TexFormula formula, char character) => false;

    private record EndCommand(EndEnvironment Environment) : ICommandParser
    {
        public CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var position = context.ArgumentsStartPosition;
            var name = TexFormulaParser.ReadElement(context.CommandSource, ref position).ToString();
            if (Environment.Ended)
                throw new TexParseException(
                    $"There are multiple end commands directly inside of a \"{Environment._name}\" environment.");

            if (name != Environment._name)
                throw new TexParseException(
                    $@"""\end{{{name}}}"" doesn't correspond to earlier ""\begin{{{Environment._name}}}"".");

            Environment.Ended = true;
            // TODO[#191]: Terminate whole parsing. This will require the parent invocator to convert the result and
            // extract the actual ending position.
            return new CommandProcessingResult(null, position);
        }
    }
}
