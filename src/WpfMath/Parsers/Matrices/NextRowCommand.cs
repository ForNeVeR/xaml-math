using System.Collections.Generic;
using WpfMath.Atoms;

namespace WpfMath.Parsers.Matrices
{
    internal class NextRowCommand : NextCellCommand
    {
        public NextRowCommand(List<List<Atom>> rows) : base(rows)
        {
        }

        public override CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var result = base.ProcessCommand(context);
            Rows.Add(new List<Atom>());
            return result;
        }
    }
}
