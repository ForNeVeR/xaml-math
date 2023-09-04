using System.Collections.Generic;
using XamlMath.Atoms;
using XamlMath.Boxes;
using XamlMath.Parsers.Matrices;

namespace XamlMath.Parsers;

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
            var topAtom = new List<Atom?> { topFormula.RootAtom };
            var bottomAtom = new List<Atom?> { bottomFormula.RootAtom };
            var atoms = new List<List<Atom?>> { topAtom, bottomAtom };
            var matrixAtom = new MatrixAtom(atomSource, atoms, MatrixCellAlignment.Center);
            var left = new SymbolAtom(atomSource, "(", TexAtomType.Opening, true);
            var right = new SymbolAtom(atomSource, ")", TexAtomType.Closing, true);
            var fencedAtom = new FencedAtom(atomSource, matrixAtom, left, right);
            return new CommandProcessingResult(fencedAtom, position);
        }
    }

    private sealed class CancelCommand : ICommandParser
    {
        public static CancelCommand BCancel { get; } = new(StrokeBoxMode.Back);
        public static CancelCommand Cancel { get; } = new(StrokeBoxMode.Normal);
        public static CancelCommand XCancel { get; } = new(StrokeBoxMode.Both);

        private CancelCommand(StrokeBoxMode strokeBoxMode)
        {
            _strokeBoxMode = strokeBoxMode;
        }

        private readonly StrokeBoxMode _strokeBoxMode;

        public CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var source = context.CommandSource;
            var position = context.ArgumentsStartPosition;
            var contentFormula = context.Parser.Parse(TexFormulaParser.ReadElement(source, ref position),
                                                      context.Formula.TextStyle,
                                                      context.Environment.CreateChildEnvironment());

            var start = context.CommandNameStartPosition;
            var atomSource = source.Segment(start, position - start);
            var cancelAtom = new CancelAtom(atomSource, contentFormula.RootAtom, _strokeBoxMode);

            return new CommandProcessingResult(cancelAtom, position);
        }
    }

    /// <summary>
    /// This command will parse the remaining part of an input string, and add it onto a new line of a formula. The
    /// new line is created as a <see cref="MatrixAtom"/>; the command will try to reuse existing atoms if possible.
    /// </summary>
    private class NewLineCommand : ICommandParser
    {
        public CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var source = context.CommandSource;
            var prevFormulaAtom = context.Formula.RootAtom;

            var nextLineAtom = context.Parser.Parse(
                source.Segment(context.ArgumentsStartPosition),
                context.Formula.TextStyle,
                context.Environment).RootAtom;

            // An optimization: if the new content itself is a matrix with suitable parameters, then we won't
            // wrap it into another formula, but will combine it with the content on top.
            var newMatrix = nextLineAtom is MatrixAtom m
                && m.MatrixCellAlignment == MatrixCellAlignment.Left
                && m.HorizontalPadding == MatrixAtom.DefaultPadding
                && m.VerticalPadding == MatrixAtom.DefaultPadding
                ? m
                : null;

            var topRow = new[] {prevFormulaAtom};
            var rows = new List<IEnumerable<Atom?>> {topRow};
            if (newMatrix != null)
            {
                rows.AddRange(newMatrix.MatrixCells);
            }
            else
            {
                var bottomRow = new[] {nextLineAtom};
                rows.Add(bottomRow);
            }

            // We'll always use source = null for the resulting matrix, because it's a structural element and not a
            // useful atom generated from any particular sources.
            var atom = new MatrixAtom(null, rows, MatrixCellAlignment.Left);
            var position = source.Length; // we always parse the provided source until the end
            return new CommandProcessingResult(atom, position, AtomAppendMode.Replace);
        }
    }

    internal static readonly IReadOnlyDictionary<string, ICommandParser> Dictionary =
        new Dictionary<string, ICommandParser>
        {
            [@"\"] = new NewLineCommand(),
            ["binom"] = new BinomCommand(),
            ["cancel"] = CancelCommand.Cancel,
            ["bcancel"] = CancelCommand.BCancel,
            ["xcancel"] = CancelCommand.XCancel,
            ["cases"] = MatrixCommandParser.Cases,
            ["matrix"] = MatrixCommandParser.Matrix,
            ["pmatrix"] = MatrixCommandParser.PMatrix,
            ["underline"] = new UnderlineCommand(),
            ["begin"] = new ProcessEnvironmentCommand()
        };

    internal static readonly IReadOnlyDictionary<string, IEnvironmentParser> Environments =
        new Dictionary<string, IEnvironmentParser>
        {
            ["align"] = MatrixCommandParser.Align,
            ["pmatrix"] = MatrixCommandParser.PMatrix
        };
}
