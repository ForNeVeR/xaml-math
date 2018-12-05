using System;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    // Atom representing big operator with optional limits.
    internal class BigOperatorAtom : Atom
    {
        private static Box ChangeWidth(Box box, double maxWidth)
        {
            // Centre specified box in new box of specified width, if necessary.
            if (Math.Abs(maxWidth - box.Width) > TexUtilities.FloatPrecision)
                return new HorizontalBox(box, maxWidth, TexAlignment.Center);
            else
                return box;
        }

        public BigOperatorAtom(
            SourceSpan source,
            Atom baseAtom,
            Atom lowerLimitAtom,
            Atom upperLimitAtom,
            bool? useVerticalLimits = null)
            : this(source, baseAtom, lowerLimitAtom, upperLimitAtom)
        {
            this.UseVerticalLimits = useVerticalLimits;
        }

        public BigOperatorAtom(SourceSpan source, Atom baseAtom, Atom lowerLimitAtom, Atom upperLimitAtom)
            : base(source, TexAtomType.BigOperator)
        {
            this.BaseAtom = baseAtom;
            this.LowerLimitAtom = lowerLimitAtom;
            this.UpperLimitAtom = upperLimitAtom;
            this.UseVerticalLimits = null;
        }

        // Atom representing big operator.
        public Atom BaseAtom { get; }

        // Atoms representing lower and upper limits.
        public Atom LowerLimitAtom { get; }

        public Atom UpperLimitAtom { get; }

        // True if limits should be drawn over and under the base atom; false if they should be drawn as scripts.
        public bool? UseVerticalLimits { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;

            if ((this.UseVerticalLimits.HasValue && !this.UseVerticalLimits.Value) ||
                (!this.UseVerticalLimits.HasValue && style >= TexStyle.Text))
                // Attach atoms for limits as scripts.
                return new ScriptsAtom(this.Source, this.BaseAtom, this.LowerLimitAtom, this.UpperLimitAtom)
                    .CreateBox(environment);

            // Create box for base atom.
            Box baseBox;
            double delta;

            if (this.BaseAtom is SymbolAtom && this.BaseAtom.Type == TexAtomType.BigOperator)
            {
                // Find character of best scale for operator symbol.
                var opChar = texFont.GetCharInfo(((SymbolAtom)this.BaseAtom).Name, style).Value;
                if (style < TexStyle.Text && texFont.HasNextLarger(opChar))
                    opChar = texFont.GetNextLargerCharInfo(opChar, style);
                var charBox = new CharBox(environment, opChar) { Source = this.BaseAtom.Source };
                charBox.Shift = -(charBox.Height + charBox.Depth) / 2 -
                    environment.MathFont.GetAxisHeight(environment.Style);
                baseBox = new HorizontalBox(charBox);

                delta = opChar.Metrics.Italic;
                if (delta > TexUtilities.FloatPrecision)
                    baseBox.Add(new StrutBox(delta, 0, 0, 0));
            }
            else
            {
                baseBox = new HorizontalBox(this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment));
                delta = 0;
            }

            // Create boxes for upper and lower limits.
            var upperLimitBox = this.UpperLimitAtom == null ? null : this.UpperLimitAtom.CreateBox(
                environment.GetSuperscriptStyle());
            var lowerLimitBox = this.LowerLimitAtom == null ? null : this.LowerLimitAtom.CreateBox(
                environment.GetSubscriptStyle());

            // Make all component boxes equally wide.
            var maxWidth = Math.Max(Math.Max(baseBox.Width, upperLimitBox == null ? 0 : upperLimitBox.Width),
                lowerLimitBox == null ? 0 : lowerLimitBox.Width);
            if (baseBox != null)
                baseBox = ChangeWidth(baseBox, maxWidth);
            if (upperLimitBox != null)
                upperLimitBox = ChangeWidth(upperLimitBox, maxWidth);
            if (lowerLimitBox != null)
                lowerLimitBox = ChangeWidth(lowerLimitBox, maxWidth);

            var resultBox = new VerticalBox();
            var opSpacing5 = texFont.GetBigOpSpacing5(style);
            var kern = 0d;

            // Create and add box for upper limit.
            if (this.UpperLimitAtom != null)
            {
                resultBox.Add(new StrutBox(0, opSpacing5, 0, 0));
                upperLimitBox.Shift = delta / 2;
                resultBox.Add(upperLimitBox);
                kern = Math.Max(texFont.GetBigOpSpacing1(style), texFont.GetBigOpSpacing3(style) -
                    upperLimitBox.Depth);
                resultBox.Add(new StrutBox(0, kern, 0, 0));
            }

            // Add box for base atom.
            resultBox.Add(baseBox);

            // Create and add box for lower limit.
            if (this.LowerLimitAtom != null)
            {
                resultBox.Add(new StrutBox(0, Math.Max(texFont.GetBigOpSpacing2(style), texFont.GetBigOpSpacing4(style) -
                    lowerLimitBox.Height), 0, 0));
                lowerLimitBox.Shift = -delta / 2;
                resultBox.Add(lowerLimitBox);
                resultBox.Add(new StrutBox(0, opSpacing5, 0, 0));
            }

            // Adjust height and depth of result box.
            var baseBoxHeight = baseBox.Height;
            var totalHeight = resultBox.Height + resultBox.Depth;
            if (upperLimitBox != null)
                baseBoxHeight += opSpacing5 + kern + upperLimitBox.Height + upperLimitBox.Depth;
            resultBox.Height = baseBoxHeight;
            resultBox.Depth = totalHeight - baseBoxHeight;

            return resultBox;
        }
    }
}
