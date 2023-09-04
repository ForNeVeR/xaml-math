using System.Collections.Generic;

namespace XamlMath.Parsers;

/// <summary>
/// An environment in which the command parsing is performed. This environment may provide additional commands for
/// the current parser context.
/// <para/>
/// Environment may be recursive and non-recursive, it decides whether it should be recursive or provide any other
/// kind of environment for child contexts itself.
/// </summary>
internal interface ICommandEnvironment
{
    /// <summary>Commands from the current environment.</summary>
    IReadOnlyDictionary<string, ICommandParser> AvailableCommands { get; }

    /// <summary>This method gets called when the environment is about to be applied recursively.</summary>
    /// <returns>
    /// A child environment that will be applied to the child parsing context (e.g. a nested element group).
    /// </returns>
    ICommandEnvironment CreateChildEnvironment();

    /// <summary>Processes an unknown character found during parsing.</summary>
    /// <param name="character">The character that wasn't resolved during parsing.</param>
    /// <returns>
    /// Should return <c>true</c> if the character was processed by this method. Otherwise, parser will throw an
    /// exception.
    /// </returns>
    bool ProcessUnknownCharacter(TexFormula formula, char character);
}
