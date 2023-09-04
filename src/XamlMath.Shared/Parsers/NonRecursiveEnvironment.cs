using System.Collections.Generic;

namespace XamlMath.Parsers;

internal abstract class NonRecursiveEnvironment : ICommandEnvironment
{
    private readonly ICommandEnvironment _environment;

    public NonRecursiveEnvironment(
        ICommandEnvironment environment,
        IReadOnlyDictionary<string, ICommandParser> availableCommands)
    {
        _environment = environment;
        AvailableCommands = availableCommands;
    }

    public IReadOnlyDictionary<string, ICommandParser> AvailableCommands { get; }

    public ICommandEnvironment CreateChildEnvironment() => _environment;

    public abstract bool ProcessUnknownCharacter(TexFormula formula, char character);
}
