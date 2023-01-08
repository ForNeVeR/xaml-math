using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WpfMath.Atoms;
using WpfMath.Boxes;
using WpfMath.Rendering;

namespace WpfMath
{
    /// <summary>Represents mathematical formula that can be rendered.</summary>
    public sealed class TexFormula
    {
        public string? TextStyle
        {
            get;
            set;
        }

        internal Atom? RootAtom
        {
            get;
            set;
        }

        public SourceSpan? Source { get; set; }

        // TODO: Document the absence of TexFormula.GetRenderer method (that should be replaced with
        // WpfMath.Rendering.TeXFormulaExtensions and WpfMath.Rendering.WpfTeXFormulaExtensions).

        public void Add(TexFormula formula, SourceSpan? source = null)
        {
            Debug.Assert(formula != null);
            Debug.Assert(formula.RootAtom != null);

            this.Add(
                formula.RootAtom is RowAtom
                    ? new RowAtom(source, formula.RootAtom)
                    : formula.RootAtom,
                source);
        }

        /// <summary>
        /// Adds an atom to the formula. If the <see cref="RootAtom"/> exists and is not a <see cref="RowAtom"/>, it
        /// will become one.
        /// </summary>
        /// <param name="atom">The atom to add.</param>
        /// <param name="rowSource">The source that will be set for the resulting row atom.</param>
        internal void Add(Atom atom, SourceSpan? rowSource)
        {
            if (this.RootAtom == null)
            {
                this.RootAtom = atom;
            }
            else
            {
                var elements = (this.RootAtom is RowAtom r
                    ? (IEnumerable<Atom>) r.Elements
                    : new[] { this.RootAtom }).ToList();
                elements.Add(atom);
                this.RootAtom = new RowAtom(rowSource, elements);
            }
        }

        public void SetForeground(IPlatformBrush brush)
        {
            if (this.RootAtom is StyledAtom sa)
            {
                this.RootAtom = sa with { Foreground = brush };
            }
            else
            {
                RootAtom = new StyledAtom(RootAtom?.Source, RootAtom, null, brush);
            }
        }

        public void SetBackground(IPlatformBrush brush)
        {
            if (this.RootAtom is StyledAtom sa)
            {
                this.RootAtom = sa with { Background = brush };
            }
            else
            {
                this.RootAtom = new StyledAtom(this.RootAtom?.Source, this.RootAtom, brush, null);
            }
        }

        public Box CreateBox(TexEnvironment environment)
        {
            if (this.RootAtom == null)
                return StrutBox.Empty;
            else
                return this.RootAtom.CreateBox(environment);
        }
    }
}
