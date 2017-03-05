using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath
{
    // Box representing glue.
    internal class GlueBox : Box
    {
        public GlueBox(double space, double stretch, double shrink)
        {
            this.Width = space;
            this.Stretch = stretch;
            this.Shrink = shrink;
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

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
