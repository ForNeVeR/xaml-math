using System.Collections.Generic;
using XamlMath.Atoms;

namespace XamlMath.Parsers.Matrices;

internal sealed class MatrixInternalEnvironment : NonRecursiveEnvironment
{
    private static IReadOnlyDictionary<string, ICommandParser> GetCommands(List<List<Atom>> rows)
    {
        var nextRowCommand = new NextRowCommand(rows);
        return new Dictionary<string, ICommandParser>
        {
            [@"\"] = nextRowCommand,
            ["cr"] = nextRowCommand
        };
    }

    private readonly List<List<Atom>> _rows;

    public MatrixInternalEnvironment(
        ICommandEnvironment parentEnvironment,
        List<List<Atom>> rows) : base(parentEnvironment.CreateChildEnvironment(), GetCommands(rows))
    {
        _rows = rows;
    }

    public override bool ProcessUnknownCharacter(TexFormula formula, char character)
    {
        if (character == '&')
        {
            NextRowCommand.NextCell(_rows, formula);
            return true;
        }

        return false;
    }
}
