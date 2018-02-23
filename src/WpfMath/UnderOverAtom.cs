using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Atom representing other atom with atoms optionally over and under it.
    internal class UnderOverAtom : Atom
    {
        private static Box ChangeWidth(Box box, double maxWidth)
        {
            if (box != null && Math.Abs(maxWidth - box.Width) > TexUtilities.FloatPrecision)
                return new HorizontalBox(box, maxWidth, TexAlignment.Center);
            else
                return box;
        }

        public UnderOverAtom(Atom baseAtom, Atom underOver, TexUnit underOverUnit, double underOverSpace,
            bool underOverScriptSize, bool over)
        {
            SpaceAtom.CheckUnit(underOverUnit);

            this.BaseAtom = baseAtom;

            if (over)
            {
                this.UnderAtom = null;
                this.UnderSpace = 0;
                this.UnderSpaceUnit = 0;
                this.UnderScriptSmaller = false;
                this.OverAtom = underOver;
                this.OverSpaceUnit = underOverUnit;
                this.OverSpace = underOverSpace;
                this.OverScriptSmaller = underOverScriptSize;
            }
            else
            {
                this.UnderAtom = underOver;
                this.UnderSpaceUnit = underOverUnit;
                this.UnderSpace = underOverSpace;
                this.UnderScriptSmaller = underOverScriptSize;
                this.OverSpace = 0;
                this.OverAtom = null;
                this.OverSpaceUnit = 0;
                this.OverScriptSmaller = false;
            }
        }

        public UnderOverAtom(Atom baseAtom, Atom under, TexUnit underUnit, double underSpace, bool underScriptSize,
            Atom over, TexUnit overUnit, double overSpace, bool overScriptSize)
        {
            SpaceAtom.CheckUnit(underUnit);
            SpaceAtom.CheckUnit(overUnit);

            this.BaseAtom = baseAtom;
            this.UnderAtom = under;
            this.UnderSpaceUnit = underUnit;
            this.UnderSpace = underSpace;
            this.UnderScriptSmaller = underScriptSize;
            this.OverAtom = over;
            this.OverSpaceUnit = overUnit;
            this.OverSpace = overSpace;
            this.OverScriptSmaller = overScriptSize;
        }

        public Atom BaseAtom
        {
            get;
            private set;
        }

        public Atom UnderAtom
        {
            get;
            private set;
        }

        public Atom OverAtom
        {
            get;
            private set;
        }

        // Kern between base and under atom.
        public double UnderSpace
        {
            get;
            set;
        }

        // Kern between base and over atom.
        public double OverSpace
        {
            get;
            set;
        }

        public TexUnit UnderSpaceUnit
        {
            get;
            set;
        }

        public TexUnit OverSpaceUnit
        {
            get;
            set;
        }

        public bool UnderScriptSmaller
        {
            get;
            set;
        }

        public bool OverScriptSmaller
        {
            get;
            set;
        }

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            // Create box for base atom.
            var baseBox = this.BaseAtom == null ? StrutBox.Empty : this.BaseAtom.CreateBox(environment);

            // Create boxes for over and under atoms.
            Box overBox = null, underBox = null;
            var maxWidth = baseBox.Width;

            if (this.OverAtom != null)
            {
                overBox = OverAtom.CreateBox(OverScriptSmaller ? environment.GetSubscriptStyle() : environment);
                maxWidth = Math.Max(maxWidth, overBox.Width);
            }

            if (this.UnderAtom != null)
            {
                underBox = UnderAtom.CreateBox(UnderScriptSmaller ? environment.GetSubscriptStyle() : environment);
                maxWidth = Math.Max(maxWidth, underBox.Width);
            }

            // Create result box.
            var resultBox = new VerticalBox();

            environment.LastFontId = baseBox.GetLastFontId();

            // Create and add box for over atom.
            if (this.OverAtom != null)
            {
                resultBox.Add(ChangeWidth(overBox, maxWidth));
                resultBox.Add(new SpaceAtom(OverSpaceUnit, 0, OverSpace, 0).CreateBox(environment));
            }

            // Add box for base atom.
            resultBox.Add(ChangeWidth(baseBox, maxWidth));

            double totalHeight = resultBox.Height + resultBox.Depth - baseBox.Depth;

            // Create and add box for under atom.
            if (this.UnderAtom != null)
            {
                resultBox.Add(new SpaceAtom(OverSpaceUnit, 0, UnderSpace, 0).CreateBox(environment));
                resultBox.Add(ChangeWidth(underBox, maxWidth));
            }

            resultBox.Depth = resultBox.Height + resultBox.Depth - totalHeight;
            resultBox.Height = totalHeight;

            return resultBox;
        }
    }
}
