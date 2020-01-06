using System.Collections.Generic;
using WpfMath.Atoms;
using WpfMath.Parsers.Matrices;

namespace WpfMath.Parsers
{
    internal static class StandardCommands
    {
        private class UnderlineCommand : ICommandParser
        {
            public CommandProcessingResult ProcessCommand(CommandContext context)
            {
                var source = context.CommandSource;
                var position = context.ArgumentsStartPosition;
                var underlineFormula = context.Parser.Parse(
                    TexFormulaParser.ReadElement(source, ref position),
                    context.Formula.TextStyle,
                    context.Environment);
                var start = context.CommandNameStartPosition;
                var atomSource = source.Segment(start, position - start);
                var atom = new UnderlinedAtom(atomSource, underlineFormula.RootAtom);
                return new CommandProcessingResult(atom, position);
            }
        }

        private class BinomCommand : ICommandParser
        {
            public CommandProcessingResult ProcessCommand(CommandContext context)
            {
                var source = context.CommandSource;
                var position = context.ArgumentsStartPosition;
                var topFormula = context.Parser.Parse(
                            TexFormulaParser.ReadElement(source, ref position),
                            context.Formula.TextStyle,
                            context.Environment.CreateChildEnvironment());
                var bottomFormula = context.Parser.Parse(
                            TexFormulaParser.ReadElement(source, ref position),
                            context.Formula.TextStyle,
                            context.Environment.CreateChildEnvironment());
                var start = context.CommandNameStartPosition;
                var atomSource = source.Segment(start, position - start);
                var topAtom = new List<Atom> { topFormula.RootAtom };
                var bottomAtom = new List<Atom> { bottomFormula.RootAtom };
                var atoms = new List<List<Atom>> { topAtom, bottomAtom };
                var matrixAtom = new MatrixAtom(atomSource, atoms, MatrixCellAlignment.Center);
                var left = new SymbolAtom(atomSource, "(", TexAtomType.Opening, true);
                var right = new SymbolAtom(atomSource, ")", TexAtomType.Closing, true);
                var fencedAtom = new FencedAtom(atomSource, matrixAtom, left, right);
                return new CommandProcessingResult(fencedAtom, position);
            }
        }

        private class NewLineCommand : ICommandParser
        {
            public CommandProcessingResult ProcessCommand(CommandContext context)
            {
                var source = context.CommandSource;
                var position = context.ArgumentsStartPosition;
                var prevFormulaAtom = context.Formula.RootAtom;

                var topRow = new List<Atom>();
                topRow.Add(prevFormulaAtom);
                var rows = new List<List<Atom>> {topRow};

                var newContent = context.Parser.Parse(
                    TexFormulaParser.ReadElement(source, ref position),
                    context.Formula.TextStyle,
                    context.Environment);

                var bottomRow = new List<Atom>();
                bottomRow.Add(newContent.RootAtom);
                rows.Add(bottomRow);

                var start = context.CommandNameStartPosition;
                var atomSource = source.Segment(start, position - start);
                var atom = new MatrixAtom(atomSource, rows, MatrixCellAlignment.Center);
                return new CommandProcessingResult(atom, position, AtomAppendMode.Replace);
            }
        }

        public static IReadOnlyDictionary<string, ICommandParser> Dictionary = new Dictionary<string, ICommandParser>
        {
            [@"\"] = new NewLineCommand(),
            ["binom"] = new BinomCommand(),
            ["cases"] = new MatrixCommandParser("lbrace", null, MatrixCellAlignment.Left),
            ["matrix"] = new MatrixCommandParser(null, null, MatrixCellAlignment.Center),
            ["pmatrix"] = new MatrixCommandParser("lbrack", "rbrack", MatrixCellAlignment.Center),
            ["underline"] = new UnderlineCommand()
        };
    }
}
