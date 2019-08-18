using System.Collections.Generic;
using System.Linq;
using WpfMath.Atoms;

namespace WpfMath.Parsers.Matrices
{
    internal class NextRowCommand : ICommandParser
    {
        private readonly List<List<Atom>> _rows;

        public NextRowCommand(List<List<Atom>> rows)
        {
            _rows = rows;
        }

        internal static void NextCell(List<List<Atom>> rows, TexFormula formula)
        {
            var currentAtom = formula.RootAtom ?? new NullAtom();
            formula.RootAtom = null;

            var lastRow = rows.Last();
            lastRow.Add(currentAtom);
        }

        public CommandProcessingResult ProcessCommand(CommandContext context)
        {
            NextCell(_rows, context.Formula);
            _rows.Add(new List<Atom>());

            return new CommandProcessingResult(null, context.ArgumentsStartPosition);
        }
    }
}
