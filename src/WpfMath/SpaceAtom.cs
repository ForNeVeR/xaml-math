using System;

namespace WpfMath
{
    // Atom representing whitespace.
    internal class SpaceAtom : Atom
    {
        // Collection of unit conversion functions.
        private static UnitConversion[] unitConversions = new UnitConversion[]
                {
                    new UnitConversion(e => e.MathFont.GetXHeight(e.Style, e.LastFontId)),

                    new UnitConversion(e => e.MathFont.GetXHeight(e.Style, e.LastFontId)),

                    new UnitConversion(e => 1.0 / e.MathFont.Size),

                    new UnitConversion(e => TexFontUtilities.PixelsPerPoint / e.MathFont.Size),

                    new UnitConversion(e => (12 * TexFontUtilities.PixelsPerPoint) / e.MathFont.Size),

                    new UnitConversion(e =>
                        {
                            var texFont = e.MathFont;
                            return texFont.GetQuad(texFont.GetMuFontId(), e.Style) / 18;
                        }),
                };

        private delegate double UnitConversion(TexEnvironment environment);

        public static void CheckUnit(TexUnit unit)
        {
            if ((int)unit < 0 || (int)unit >= unitConversions.Length)
                throw new InvalidOperationException("No conversion for this unit exists.");
        }

        // True to represent hard space (actual space character).
        private bool isHardSpace;

        private double width;
        private double height;
        private double depth;

        private TexUnit widthUnit;
        private TexUnit heightUnit;
        private TexUnit depthUnit;

        public SpaceAtom(TexUnit widthUnit, double width, TexUnit heightUnit, double height,
            TexUnit depthUnit, double depth)
            : base()
        {
            CheckUnit(widthUnit);
            CheckUnit(heightUnit);
            CheckUnit(depthUnit);

            this.isHardSpace = false;
            this.widthUnit = widthUnit;
            this.heightUnit = heightUnit;
            this.depthUnit = depthUnit;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public SpaceAtom(TexUnit unit, double width, double height, double depth)
            : base()
        {
            CheckUnit(unit);

            this.isHardSpace = false;
            this.widthUnit = unit;
            this.heightUnit = unit;
            this.depthUnit = unit;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public SpaceAtom()
            : base()
        {
            this.isHardSpace = true;
        }

        public override Atom Copy()
        {
            var atom = new SpaceAtom();

            atom.isHardSpace = isHardSpace;

            atom.width = width;
            atom.height = height;
            atom.depth = depth;

            atom.widthUnit = widthUnit;
            atom.heightUnit = heightUnit;
            atom.depthUnit = depthUnit;

            return CopyTo(atom);
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            if (isHardSpace)
                return new StrutBox(environment.MathFont.GetSpace(environment.Style), 0, 0, 0);
            else
                return new StrutBox(width * GetConversionFactor(widthUnit, environment), height * GetConversionFactor(
                    heightUnit, environment), depth * GetConversionFactor(depthUnit, environment), 0);
        }

        private double GetConversionFactor(TexUnit unit, TexEnvironment environment)
        {
            return unitConversions[(int)unit](environment);
        }
    }
}
