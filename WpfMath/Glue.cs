using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    // Represents glueElement for holding together boxes.
    internal class Glue
    {
        private static readonly IList<Glue> glueTypes;
        private static readonly int[, ,] glueRules;

        static Glue()
        {
            var parser = new WpfMath.GlueSettingsParser();
            glueTypes = parser.GetGlueTypes();
            glueRules = parser.GetGlueRules();
        }

        public static Box CreateBox(TexAtomType leftAtomType, TexAtomType rightAtomType, WpfMath.TexEnvironment environment)
        {
            leftAtomType = leftAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : leftAtomType;
            rightAtomType = rightAtomType > TexAtomType.Inner ? TexAtomType.Ordinary : rightAtomType;
            var glueType = glueRules[(int)leftAtomType, (int)rightAtomType, (int)environment.Style / 2];
            return glueTypes[glueType].CreateBox(environment);
        }

        public Glue(double space, double stretch, double shrink, string name)
        {
            this.Space = space;
            this.Stretch = stretch;
            this.Shrink = shrink;
            this.Name = name;
        }

        public double Space
        {
            get;
            private set;
        }

        public double Stretch
        {
            get;
            private set;
        }

        public double Shrink
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        private Box CreateBox(WpfMath.TexEnvironment environment)
        {
            var texFont = environment.TexFont;
            var quad = texFont.GetQuad(texFont.GetMuFontId(), environment.Style);
            return new WpfMath.GlueBox((this.Space / 18.0f) * quad, (this.Stretch / 18.0f) * quad, (this.Shrink / 18.0f) * quad);
        }
    }
}
