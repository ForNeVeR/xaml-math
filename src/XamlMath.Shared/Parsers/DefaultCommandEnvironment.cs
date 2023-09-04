using System.Collections.Generic;

namespace XamlMath.Parsers;

internal sealed class DefaultCommandEnvironment : ICommandEnvironment
{
    public static readonly ICommandEnvironment Instance = new DefaultCommandEnvironment();

    public IReadOnlyDictionary<string, ICommandParser> AvailableCommands { get; } =
        new Dictionary<string, ICommandParser>();

    public ICommandEnvironment CreateChildEnvironment() => Instance;

    public bool ProcessUnknownCharacter(TexFormula formula, char character) => false;
}
