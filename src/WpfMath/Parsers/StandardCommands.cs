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
                //binom{2}{5}
               var source = context.CommandSource;
               var position = context.ArgumentsStartPosition;
                var top = context.Parser.Parse(
                    TexFormulaParser.ReadElement(source, ref position),
                    context.Formula.TextStyle,
                    context.Environment);
                var bottom = context.Parser.Parse(
                   TexFormulaParser.ReadElement(source, ref position),
                   context.Formula.TextStyle,
                   context.Environment);
                var start = context.CommandNameStartPosition;
                var atomSource = source.Segment(start, position - start);
                var atoms1 = new List<Atom> {top.RootAtom};
                var atoms2 = new List<Atom> { bottom.RootAtom };
                var atoms = new List<List<Atom>> { atoms1 , atoms2};
                var matrixAtom = new MatrixAtom(atomSource, atoms, MatrixCellAlignment.Center);
                var left = new SymbolAtom(source, "(", TexAtomType.Opening, true);
                var right = new SymbolAtom(source, ")", TexAtomType.Closing, true);
                var fencedAtom = new FencedAtom(atomSource, matrixAtom, left, right);
                return new CommandProcessingResult(fencedAtom, position);
            }
        }

        public static IReadOnlyDictionary<string, ICommandParser> Dictionary = new Dictionary<string, ICommandParser>
        {
            ["cases"] = new MatrixCommandParser("lbrace", null, MatrixCellAlignment.Left),
            ["matrix"] = new MatrixCommandParser(null, null, MatrixCellAlignment.Center),
            ["pmatrix"] = new MatrixCommandParser("lbrack", "rbrack", MatrixCellAlignment.Center),
            ["underline"] = new UnderlineCommand(),
            ["binom"] = new BinomCommand()
        };
    }
}
