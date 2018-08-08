using System;
using WpfMath.Atoms;
using WpfMath.Boxes;

namespace WpfMath
{
    // Atom representing radical (nth-root) construction.
    internal class Radical : Atom
    {
        private const string sqrtSymbol = "sqrt";

        private const double scale = 0.55;

        public Radical(SourceSpan source, Atom baseAtom, Atom degreeAtom = null)
            : base(source)
        {
            this.BaseAtom = baseAtom;
            this.DegreeAtom = degreeAtom;
        }

        public Atom BaseAtom { get; }

        public Atom DegreeAtom { get; }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            var texFont = environment.MathFont;
            var style = environment.Style;

            // Calculate minimum clearance amount.
            double clearance;
            var defaultRuleThickness = texFont.GetDefaultLineThickness(style);
            if (style < TexStyle.Text)
                clearance = texFont.GetXHeight(style, texFont.GetCharInfo(sqrtSymbol, style).Value.FontId);
            else
                clearance = defaultRuleThickness;
            clearance = defaultRuleThickness + Math.Abs(clearance) / 4;

            // Create box for base atom, in cramped style.
            var baseBox = this.BaseAtom.CreateBox(environment.GetCrampedStyle());

            // Create box for radical sign.
            var totalHeight = baseBox.Height + baseBox.Depth;
            var radicalSignBox = DelimiterFactory.CreateBox(sqrtSymbol, totalHeight + clearance + defaultRuleThickness, environment, Source);
            radicalSignBox.Source = Source;

            // Add half of excess height to clearance.
            var delta = radicalSignBox.Depth - (totalHeight + clearance);
            clearance += delta / 2;

            // Create box for radical sign.
            var totalHeight = baseBox.Height + baseBox.Depth;
            var radicalSignBox = DelimiterFactory.CreateBox(sqrtSymbol, totalHeight + clearance + defaultRuleThickness,environment, Source);
            radicalSignBox.Source = Source;
            
            // Add half of excess height to clearance.
            var delta = radicalSignBox.Depth - (totalHeight + clearance);
            clearance += delta / 2;

            // Create box for square-root containing base box.
            var overBar = new OverBar(environment, baseBox, clearance, radicalSignBox.Height)
            {
                Shift = -defaultRuleThickness,
            };

            //Create box to hold the radical and the degree atom(if it exists)
            var radicalContainerBox = new VerticalBox();
            radicalContainerBox.Add(radicalSignBox);
            
            // Create box for root atom.
            var  radrootBox =this.DegreeAtom==null?StrutBox.Empty: this.DegreeAtom.CreateBox(environment.GetRootStyle());
            var bottomShift = scale * (radicalSignBox.Height + radicalSignBox.Depth);
            var rcbItemsdiff = radicalSignBox.TotalHeight -radrootBox.TotalHeight;
            bottomShift = rcbItemsdiff;
            if (rcbItemsdiff < radicalSignBox.Height/2)
            {
                //bottomShift += radicalSignBox.Height / 4;
                var gh = (radicalSignBox.Height / 2) - bottomShift;
                bottomShift = (radicalSignBox.Height / 2);
            }
            var Vnegspace = new StrutBox(0, -bottomShift, 0, 0);
            radicalContainerBox.Add(Vnegspace);
            radicalContainerBox.Add(radrootBox);
            //var Hnegspace = new StrutBox(-radicalSignBox.TotalWidth / 2, -bottomShift, 0, 0);
            
            //var rsignboxshift = overBar.TotalHeight - radicalSignBox.TotalHeight;
            radicalSignBox.Shift =radrootBox.TotalWidth-radicalSignBox.TotalWidth/2;
            
            // Create result box.
            var resultBox = new HorizontalBox();

            resultBox.Add(radicalContainerBox);
            var leftpad = radicalContainerBox.TotalWidth -radicalSignBox.Shift - radicalSignBox.TotalWidth;
            resultBox.Add(new StrutBox(-leftpad, 0, 0, 0));
            resultBox.Add(overBar);
            resultBox.Shift = -(baseBox.Height + clearance + defaultRuleThickness);

            return resultBox;
        }
    }
}
