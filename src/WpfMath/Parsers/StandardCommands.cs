using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WpfMath.Atoms;
using WpfMath.Exceptions;
using WpfMath.Parsers.Matrices;

namespace WpfMath.Parsers
{
    internal static class StandardCommands
    {
        /// <summary>
        /// This command will parse a \begin \end environment. Doesn't support nested environments
        /// </summary>
        private class BeginCommand : ICommandParser
        {
            public CommandProcessingResult ProcessCommand(CommandContext context)
            {
                int position = context.ArgumentsStartPosition;
                SourceSpan source = context.CommandSource;
                if (position == source.Length)
                    throw new TexParseException("illegal end!");
                if (!source.ToString().Contains("\\end")) throw new TexParseException("No matching \\end found!");

                SourceSpan element = TexFormulaParser.ReadElement(source, ref position);
                string argument = element.ToString();
                int endIndex = -1;
                for (; position < source.End; position++)
                {
                    if (position + 4 >= source.Length || source[source.Length - 1] != '}') throw new TexParseException("Unfinished command \\end!");
                    if (source[position] == '\\' && source[position + 1] == 'e' && source[position + 2] == 'n' && source[position + 3] == 'd')
                    {
                        /* int i = 0;
                        foreach (char c in "{" + argument + "}") // Trying to implement nested environments
                        {
                            i++;
                            if (c != source[position + 3 + i])
                            {
                                throw 
                            }
                        } */
                        endIndex = position;
                        break;
                    }
                }

                string newContent = source.ToString().Substring(context.ArgumentsStartPosition + element.Length + 2, endIndex - (context.ArgumentsStartPosition + element.Length) - 2);

                newContent = "\\" + argument + "{" + newContent + "}";
                TexFormula content = context.Parser.Parse(newContent);
                return new CommandProcessingResult(content.RootAtom, position + 3 + argument.Length + 3);
            }
        }

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

        public static IReadOnlyDictionary<string, ICommandParser> Dictionary = new Dictionary<string, ICommandParser>
        {
            [@"\"] = new NewLineCommand(),
            ["binom"] = new BinomCommand(),
            ["cases"] = new MatrixCommandParser("lbrace", null, MatrixCellAlignment.Left),
            ["matrix"] = new MatrixCommandParser(null, null, MatrixCellAlignment.Center),
            ["pmatrix"] = new MatrixCommandParser("lbrack", "rbrack", MatrixCellAlignment.Center),
            ["underline"] = new UnderlineCommand(),
            ["begin"] = new BeginCommand()
        };
    }
}
