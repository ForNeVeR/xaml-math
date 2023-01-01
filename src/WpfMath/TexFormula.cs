using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using WpfMath.Atoms;
using WpfMath.Boxes;
using WpfMath.Fonts;

namespace WpfMath
{
    // Represents mathematical formula that can be rendered.
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

        [Obsolete("Use extension methods from WpfMath.Rendering.TeXFormulaExtensions and WpfMath.Rendering.WpfTeXFormulaExtensions instead.")]
        public TexRenderer GetRenderer(TexStyle style, // TODO: Revise internal usages of this method.
            double scale,
            string? systemTextFontName,
            Brush? background = null,
            Brush? foreground = null)
        {
            var mathFont = new DefaultTexFont(WpfMathFontProvider.Instance, scale);
            var textFont = systemTextFontName == null ? (ITeXFont)mathFont : GetSystemFont(systemTextFontName, scale);
            var environment = new TexEnvironment(style, mathFont, textFont, background, foreground);
            return new TexRenderer(CreateBox(environment), scale);
        }

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

        public void SetForeground(Brush brush)
        {
            if (this.RootAtom is StyledAtom sa)
            {
                this.RootAtom = sa with { Foreground = brush };
            }
            else
            {
                this.RootAtom = new StyledAtom(this.RootAtom?.Source, this.RootAtom, null, brush);
            }
        }

        public void SetBackground(Brush brush)
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

        internal Box CreateBox(TexEnvironment environment)
        {
            if (this.RootAtom == null)
                return StrutBox.Empty;
            else
                return this.RootAtom.CreateBox(environment);
        }

        internal static SystemFont GetSystemFont(string fontName, double size)
        {
            var fontFamily = System.Windows.Media.Fonts.SystemFontFamilies.First(
                ff => ff.ToString() == fontName || ff.FamilyNames.Values?.Contains(fontName) == true);
            return new SystemFont(size, fontFamily);
        }
    }
}
