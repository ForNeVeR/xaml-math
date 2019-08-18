using System;
using System.Collections.Generic;
using System.Text;
using WpfMath.Atoms;
using WpfMath.Exceptions;

namespace WpfMath.Parsers
{
    /// <summary>A parser for matrix-like constructs.</summary>
    internal class MatrixCommandParser : ICommandParser
    {
        private readonly string _leftDelimiterSymbolName;
        private readonly string _rightDelimiterSymbolName;
        private readonly MatrixCellAlignment _cellAlignment;

        public MatrixCommandParser(
            string leftDelimiterSymbolName,
            string rightDelimiterSymbolName,
            MatrixCellAlignment cellAlignment)
        {
            _leftDelimiterSymbolName = leftDelimiterSymbolName;
            _rightDelimiterSymbolName = rightDelimiterSymbolName;
            _cellAlignment = cellAlignment;
        }

        public CommandProcessingResult ProcessCommand(CommandContext context)
        {
            var position = context.ArgumentsStartPosition;
            var source = context.CommandSource;

            if (position == source.Length)
                throw new TexParseException("illegal end!");

            var matrixSource = TexFormulaParser.ReadElement(source, ref position);

            var cells = ReadMatrixCells(context.Parser, context.Formula, matrixSource);
            var matrix = new MatrixAtom(matrixSource, cells, _cellAlignment);

            SymbolAtom GetDelimiter(string name) =>
                name == null
                    ? null
                    : TexFormulaParser.GetDelimiterSymbol(name, null) ??
                      throw new TexParseException($"The delimiter {name} could not be found");

            var leftDelimiter = GetDelimiter(_leftDelimiterSymbolName);
            var rightDelimiter = GetDelimiter(_rightDelimiterSymbolName);

            var atom = leftDelimiter == null && rightDelimiter == null
                ? (Atom) matrix
                : new FencedAtom(
                    matrixSource,
                    matrix,
                    leftDelimiter,
                    rightDelimiter);
            return new CommandProcessingResult(atom, position);
        }

        /// <summary>
        /// Retrives the cells of a matrix from a given source.
        /// </summary>
        /// <param name="matrixsource">The source to retrieve the 2D-Array of atoms from.</param>
        private List<List<Atom>> ReadMatrixCells(TexFormulaParser parser, TexFormula formula, SourceSpan matrixsource)
        {
            // rowindent: how many characters the next row should skip for its sourcespan to start
            var rowdata = new List<List<(StringBuilder builder, int rowindent)>>
            {
                new List<(StringBuilder, int)> {(new StringBuilder(), 0)}
            };
            int rows = 0;
            int cols = 0;

            //to ensure multiple row separators aren't used
            string rowseparationstyle = null;
            for (int i = 0; i < matrixsource.ToString().Length; i++)
            {
                var curchar = matrixsource.ToString()[i];
                var nextchar = i < matrixsource.ToString().Length - 1 ? matrixsource.ToString()[i + 1] : matrixsource.ToString()[i];
                var thirdchar = i < matrixsource.ToString().Length - 2 ? matrixsource.ToString()[i + 2] : matrixsource.ToString()[i];

                if (curchar == '\\' && nextchar == '\\')
                {
                    if (rowseparationstyle == null || rowseparationstyle == "slash")
                    {
                        if (i + 2 == matrixsource.ToString().Length || String.IsNullOrWhiteSpace(matrixsource.ToString().Substring(i + 2)))
                        {
                            i = matrixsource.ToString().Length - 1;
                        }
                        else
                        {
                            rowdata.Add(new List<(StringBuilder, int)> { (new StringBuilder(), 2) });
                            rows++;
                            cols = 0;
                            i++;
                            rowseparationstyle = "slash";
                        }
                    }
                    else
                    {
                        throw new TexParseException("Multiple row separator styles cannot be used.");
                    }
                }
                else if (curchar == '\\' && nextchar == 'c' && thirdchar == 'r')
                {
                    if (rowseparationstyle == null || rowseparationstyle == "carriagereturn")
                    {
                        if (i + 3 == matrixsource.ToString().Length || String.IsNullOrWhiteSpace(matrixsource.ToString().Substring(i + 3)))
                        {
                            i = matrixsource.ToString().Length - 1;
                        }
                        else
                        {
                            rowdata.Add(new List<(StringBuilder, int)> {(new StringBuilder(), 3)});
                            rows++;
                            cols = 0;
                            i += 2;
                            rowseparationstyle = "carriagereturn";
                        }

                    }
                    else
                    {
                        throw new TexParseException("Multiple row separator styles cannot be used.");
                    }
                }
                else if (curchar == '\\' && Char.IsSymbol(nextchar))
                {
                    rowdata[rows][cols].builder.Append($"{curchar}{nextchar}");
                    i++;
                }
                else if (curchar == TexFormulaParser.leftGroupChar)
                {
                    var nestedSpan = TexFormulaParser.ReadElementGroup(
                        matrixsource,
                        ref i,
                        TexFormulaParser.leftGroupChar,
                        TexFormulaParser.rightGroupChar);

                    rowdata[rows][cols].builder.Append("{" + nestedSpan + "}");
                    // Compensate for i getting incremented in the loop:
                    --i; // TODO[F]: Fix this by rethinking the parser
                }
                else if (curchar == '&')
                {
                    //create a new column in the current row.
                    rowdata[rows].Add((new StringBuilder(), 1));
                    cols++;
                }
                else
                {
                    rowdata[rows][cols].builder.Append(curchar.ToString());
                }
            }

            List<List<Atom>> matrixcells = new List<List<Atom>>();
            int matrixsrcstart = 0;
            int columnscount = 0;
            for (int i = 0; i < rowdata.Count; i++)
            {
                var rowitem = rowdata[i];
                if (rowitem.Count > 0)
                {
                    List<Atom> rowcellatoms = new List<Atom>();
                    for (int j = 0; j < rowitem.Count; j++)
                    {
                        var (cellitem, rowindent) = rowdata[i][j];
                        matrixsrcstart += rowindent;
                        if (cellitem.ToString().Trim().Length > 0 || cellitem.Length > 0)
                        {
                            var cellsource = matrixsource.Segment(matrixsrcstart, cellitem.Length);// new SourceSpan(cellitem, matrixsrcstart, cellitem.Length);
                            var cellformula = parser.Parse(cellsource, formula.TextStyle);
                            if (cellformula.RootAtom == null)
                            {
                                cellsource = new SourceSpan(" ", 0, 1);
                                rowcellatoms.Add(new NullAtom(cellsource));
                            }
                            else
                            {
                                rowcellatoms.Add(cellformula.RootAtom);
                            }

                        }
                        else
                        {
                            //Compensate by adding an invincible whitespace
                            //(This allows for empty cells instead of relying on the user to request a phantom atom or space atom)
                            var cellsource = new SourceSpan(" ", 0, 1);
                            rowcellatoms.Add(new NullAtom(cellsource));
                        }

                        matrixsrcstart += cellitem.Length;
                    }

                    matrixcells.Add(rowcellatoms);
                    columnscount = rowcellatoms.Count;
                }

            }

            int colsvalid = 0;
            foreach (var item in matrixcells)
            {
                if (item.Count == columnscount)
                {
                    colsvalid++;
                }
            }

            if (colsvalid == matrixcells.Count)
            {
                return matrixcells;
            }
            else
            {
                throw new TexParseException("The column numbers are not equal.");
            }
        }
    }
}
