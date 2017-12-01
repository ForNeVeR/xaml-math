using System.Windows.Media;
using WpfMath.Rendering;
using WpfMath.Rendering.Transformations;

namespace WpfMath
{
    // Box representing other box with delimeter and script box over or under it.
    internal class OverUnderBox : Box
    {
        public OverUnderBox(Box baseBox, Box delimeterBox, Box scriptBox, double kern, bool over)
            : base()
        {
            this.BaseBox = baseBox;
            this.DelimeterBox = delimeterBox;
            this.ScriptBox = scriptBox;
            this.Kern = kern;
            this.Over = over;

            // Calculate dimensions of box.
            this.Width = baseBox.Width;
            this.Height = baseBox.Height + (over ? delimeterBox.Width : 0) +
                (over && scriptBox != null ? scriptBox.Height + scriptBox.Depth + kern : 0);
            this.Depth = baseBox.Depth + (over ? 0 : delimeterBox.Width) +
                (!over && scriptBox == null ? 0 : scriptBox.Height + scriptBox.Depth + kern);
        }

        public Box BaseBox
        {
            get;
            private set;
        }

        public Box DelimeterBox
        {
            get;
            private set;
        }

        public Box ScriptBox
        {
            get;
            private set;
        }

        // Kern between delimeter and Script.
        public double Kern
        {
            get;
            private set;
        }

        // True to draw delimeter and script over base; false to draw under base.
        public bool Over
        {
            get;
            private set;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            renderer.RenderElement(BaseBox, x, y);

            if (Over)
            {
                // Draw delimeter and script boxes over base box.
                var centerY = y - BaseBox.Height - DelimeterBox.Width;
                var translationX = x + DelimeterBox.Width / 2;
                var translationY = centerY + DelimeterBox.Width / 2;

                RenderDelimiter(translationX, translationY);

                // Draw script box as superscript.
                RenderScriptBox(centerY - Kern - ScriptBox.Depth);
            }
            else
            {
                // Draw delimeter and script boxes under base box.
                var centerY = y + BaseBox.Depth + DelimeterBox.Width;
                var translationX = x + DelimeterBox.Width / 2;
                var translationY = centerY - DelimeterBox.Width / 2;

                RenderDelimiter(translationX, translationY);

                // Draw script box as subscript.
                RenderScriptBox(centerY + Kern + ScriptBox.Height);
            }

            void RenderDelimiter(double translationX, double translationY)
            {
                var transformations = new Transformation[]
                {
                    new Transformation.Translate(translationX, translationY),
                    new Transformation.Rotate(90)
                };

                renderer.RenderTransformed(
                    DelimeterBox,
                    transformations,
                    -DelimeterBox.Width / 2,
                    -DelimeterBox.Depth + DelimeterBox.Width / 2);
            }

            void RenderScriptBox(double yPosition)
            {
                if (ScriptBox != null)
                {
                    renderer.RenderElement(ScriptBox, x, yPosition);
                }
            }
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
