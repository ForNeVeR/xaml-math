using System.Collections.Generic;
using System.Linq;
using WpfMath.Atoms;

namespace WpfMath.Parsers.Matrices
{
    internal class NextCellCommand : ICommandParser
    {
        protected readonly List<List<Atom>> Rows;

        public NextCellCommand(List<List<Atom>> rows)
        {
            Rows = rows;
        }

        public virtual CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var currentAtom = context.Formula.RootAtom ?? new NullAtom();
            context.Formula.RootAtom = null;

            var lastRow = Rows.Last();
            lastRow.Add(currentAtom);

            return new CommandProcessingResult(null, context.ArgumentsStartPosition);
        }
    }
}
