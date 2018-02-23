namespace WpfMath
{
    // Atom representing fraction, with or without separation line.
    internal class FractionAtom : Atom
    {
        private static TexAlignment CheckAlignment(TexAlignment alignment)
        {
            if (alignment == TexAlignment.Left || alignment == TexAlignment.Right)
                return alignment;
            else
                return TexAlignment.Center;
        }

        private TexAlignment numeratorAlignment;
        private TexAlignment denominatorAlignment;

        private double lineThickness;
        private TexUnit lineThicknessUnit;

        private bool useDefaultThickness;
        private double? lineRelativeThickness;

        public FractionAtom(Atom numerator, Atom denominator, double relativeThickness,
            TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
            : this(numerator, denominator, true, numeratorAlignment, denominatorAlignment)
        {
            this.lineRelativeThickness = relativeThickness;
        }

        public FractionAtom(Atom numerator, Atom denominator, bool drawLine,
            TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
            : this(numerator, denominator, drawLine)
        {
            this.numeratorAlignment = CheckAlignment(numeratorAlignment);
            this.denominatorAlignment = CheckAlignment(denominatorAlignment);
        }

        public FractionAtom(Atom numerator, Atom denominator, bool drawLine)
            : this(numerator, denominator, drawLine, TexUnit.Pixel, 0d)
        {
        }

        public FractionAtom(Atom numerator, Atom denominator, TexUnit unit, double thickness,
            TexAlignment numeratorAlignment, TexAlignment denominatorAlignment)
            : this(numerator, denominator, unit, thickness)
        {
            this.numeratorAlignment = CheckAlignment(numeratorAlignment);
            this.denominatorAlignment = CheckAlignment(denominatorAlignment);
        }

        public FractionAtom(Atom numerator, Atom denominator, TexUnit unit, double thickness)
            : this(numerator, denominator, false, unit, thickness)
        {
        }

        protected FractionAtom(Atom numerator, Atom denominator, bool useDefaultThickness, TexUnit unit, double thickness)
        {
            SpaceAtom.CheckUnit(unit);

            this.Type = TexAtomType.Inner;
            this.Numerator = numerator;
            this.Denominator = denominator;
            this.numeratorAlignment = TexAlignment.Center;
            this.denominatorAlignment = TexAlignment.Center;
            this.useDefaultThickness = useDefaultThickness;
            this.lineThicknessUnit = unit;
            this.lineThickness = thickness;
        }

        public Atom Numerator
        {
            get;
            private set;
        }

        public Atom Denominator
        {
            get;
            private set;
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;

            // set thickness to default if default value should be used
            double lineHeight;
            var defaultLineThickness = texFont.GetDefaultLineThickness(style);
            if (this.useDefaultThickness)
                lineHeight = this.lineRelativeThickness.HasValue ? this.lineRelativeThickness.Value * defaultLineThickness :
                    defaultLineThickness;
            else
                lineHeight = new SpaceAtom(this.lineThicknessUnit, 0, this.lineThickness, 0).CreateBox(environment).Height;

            // Create boxes for numerator and demoninator atoms, and make them of equal width.
            var numeratorBox = this.Numerator == null ? StrutBox.Empty :
                this.Numerator.CreateBox(environment.GetNumeratorStyle());
            var denominatorBox = this.Denominator == null ? StrutBox.Empty :
                this.Denominator.CreateBox(environment.GetDenominatorStyle());

            if (numeratorBox.Width < denominatorBox.Width)
                numeratorBox = new HorizontalBox(numeratorBox, denominatorBox.Width, numeratorAlignment);
            else
                denominatorBox = new HorizontalBox(denominatorBox, numeratorBox.Width, denominatorAlignment);

            // Calculate preliminary shift-up and shift-down amounts.
            double shiftUp, shiftDown;
            if (style < TexStyle.Text)
            {
                shiftUp = texFont.GetNum1(style);
                shiftDown = texFont.GetDenom1(style);
            }
            else
            {
                shiftDown = texFont.GetDenom2(style);
                if (lineHeight > 0)
                    shiftUp = texFont.GetNum2(style);
                else
                    shiftUp = texFont.GetNum3(style);
            }

            // Create result box.
            var resultBox = new VerticalBox();

            // add box for numerator.
            resultBox.Add(numeratorBox);

            // Calculate clearance and adjust shift amounts.
            var axis = texFont.GetAxisHeight(style);

            if (lineHeight > 0)
            {
                // Draw fraction line.

                // Calculate clearance amount.
                double clearance;
                if (style < TexStyle.Text)
                    clearance = 3 * lineHeight;
                else
                    clearance = lineHeight;

                // Adjust shift amounts.
                var delta = lineHeight / 2;
                var kern1 = shiftUp - numeratorBox.Depth - (axis + delta);
                var kern2 = axis - delta - (denominatorBox.Height - shiftDown);
                var delta1 = clearance - kern1;
                var delta2 = clearance - kern2;
                if (delta1 > 0)
                {
                    shiftUp += delta1;
                    kern1 += delta1;
                }
                if (delta2 > 0)
                {
                    shiftDown += delta2;
                    kern2 += delta2;
                }

                resultBox.Add(new StrutBox(0, kern1, 0, 0));
                resultBox.Add(new HorizontalRule(environment, lineHeight, numeratorBox.Width, 0));
                resultBox.Add(new StrutBox(0, kern2, 0, 0));
            }
            else
            {
                // Do not draw fraction line.

                // Calculate clearance amount.
                double clearance;
                if (style < TexStyle.Text)
                    clearance = 7 * defaultLineThickness;
                else
                    clearance = 3 * defaultLineThickness;

                // Adjust shift amounts.
                var kern = shiftUp - numeratorBox.Depth - (denominatorBox.Height - shiftDown);
                var delta = (clearance - kern) / 2;
                if (delta > 0)
                {
                    shiftUp += delta;
                    shiftDown += delta;
                    kern += 2 * delta;
                }

                resultBox.Add(new StrutBox(0, kern, 0, 0));
            }

            // add box for denominator.
            resultBox.Add(denominatorBox);

            // Adjust height and depth of result box.
            resultBox.Height = shiftUp + numeratorBox.Height;
            resultBox.Depth = shiftDown + denominatorBox.Depth;

            return resultBox;
        }
    }
}
