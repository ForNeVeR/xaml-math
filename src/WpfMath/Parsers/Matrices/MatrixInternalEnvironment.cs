using System.Collections.Generic;
using WpfMath.Atoms;

namespace WpfMath.Parsers.Matrices
{
    internal class MatrixInternalEnvironment : NonRecursiveEnvironment
    {
        private IReadOnlyDictionary<string, ICommandParser> GetCommands()
        {
            var nextRowCommand = new NextRowCommand(_rows);
            return new Dictionary<string, ICommandParser>
            {
                [@"\"] = nextRowCommand,
                ["cr"] = nextRowCommand
            };
        }

        private readonly List<List<Atom>> _rows;
        protected override IReadOnlyDictionary<string, ICommandParser> AddedCommands { get; }

        public MatrixInternalEnvironment(
            ICommandEnvironment parentEnvironment,
            List<List<Atom>> rows) : base(parentEnvironment.CreateChildEnvironment())
        {
            _rows = rows;
            AddedCommands = GetCommands();
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
}
