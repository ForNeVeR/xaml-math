using System.Collections.Generic;
using System.Linq;
using XamlMath.Atoms;
using XamlMath.Exceptions;

namespace XamlMath.Parsers.Matrices;

/// <summary>Command for \intertext{...} inside align/gathered environments.</summary>
internal sealed class IntertextCommand : ICommandParser
{
    private readonly List<List<Atom>> _rows;

    public IntertextCommand(List<List<Atom>> rows)
    {
        _rows = rows;
    }

    public CommandProcessingResult ProcessCommand(CommandContext context)
    {
        var source = context.CommandSource;
        var position = context.ArgumentsStartPosition;

        // Read the text argument
        var afterText = TexFormulaParser.ReadElement(source, position);
        position = afterText.position;

        // Parse the text content using \text style so it renders as normal text
        var textFormula = context.Parser.Parse(
            afterText.source,
            "text",
            context.Environment.CreateChildEnvironment());

        if (textFormula.RootAtom == null)
            throw new TexParseException("\\intertext requires text content");

        // Finish the current cell
        var currentAtom = context.Formula.RootAtom ?? new NullAtom();
        context.Formula.RootAtom = null;
        var lastRow = _rows.Last();
        lastRow.Add(currentAtom);

        // Add a new row with the intertext content
        _rows.Add(new List<Atom> { textFormula.RootAtom });

        // Start the next row
        _rows.Add(new List<Atom>());

        return new CommandProcessingResult(null, position);
    }
}
