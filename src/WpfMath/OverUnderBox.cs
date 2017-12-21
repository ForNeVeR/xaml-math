using System;
using Avalonia;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Media;
using WpfMath.Avalonia;

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

        public override void Draw(DrawingContext drawingContext, double scale, double x, double y)
        {
            this.BaseBox.DrawWithGuidelines(drawingContext, scale, x, y);

            if (this.Over)
            {
                // Draw delimeter and script boxes over base box.
                var centerY = y - this.BaseBox.Height - this.DelimeterBox.Width;
                var translationX = x + this.DelimeterBox.Width / 2;
                var translationY = centerY + this.DelimeterBox.Width / 2;
                var transform = new TranslateTransform(translationX * scale, translationY * scale);
                var rt = MatrixHelper.Rotation(Math.PI/2); //90 degrees
                var pt = MatrixHelper.TransformPoint(rt, new Point(translationX * scale, translationY * scale));

                //todo: восстанови, что тут должно куда вращаться
                //хоть в исходниках впфных ковыряй, хоть что

                using (var d = drawingContext.PushPreTransform(rt))
                {
                    this.DelimeterBox.DrawWithGuidelines(drawingContext, scale, -this.DelimeterBox.Width / 2,
                        -this.DelimeterBox.Depth + this.DelimeterBox.Width / 2);
                }

                // Draw script box as superscript.
                if (this.ScriptBox != null)
                    this.ScriptBox.DrawWithGuidelines(drawingContext, scale, x, centerY - this.Kern - this.ScriptBox.Depth);
            }
            else
            {
                // Draw delimeter and script boxes under base box.
                var centerY = y + this.BaseBox.Depth + this.DelimeterBox.Width;
                var translationX = x + this.DelimeterBox.Width / 2;
                var translationY = centerY - this.DelimeterBox.Width / 2;

                var rt = MatrixHelper.Rotation(Math.PI / 2); //90 degrees
                var pt = MatrixHelper.TransformPoint(rt, new Point(translationX * scale, translationY * scale));

                //todo: восстанови, что тут должно куда вращаться
                //хоть в исходниках впфных ковыряй, хоть что

                using (var d = drawingContext.PushPreTransform(rt))
                {
                    this.DelimeterBox.DrawWithGuidelines(drawingContext, scale, -this.DelimeterBox.Width / 2,
                        -this.DelimeterBox.Depth + this.DelimeterBox.Width / 2);
                }

                // Draw script box as subscript.
                ScriptBox?.DrawWithGuidelines(drawingContext, scale, x, centerY + Kern + ScriptBox.Height);
            }

        }

        public override void RenderGeometry(GeometryGroup geometry, double scale, double x, double y)
        {
            //just like code above

            //GeometryGroup group = new GeometryGroup();
            //if (this.Over)
            //{
            //    // Draw delimeter and script boxes over base box.
            //    var centerY = y - this.BaseBox.Height - this.DelimeterBox.Width;
            //    var translationX = x + this.DelimeterBox.Width / 2;
            //    var translationY = centerY + this.DelimeterBox.Width / 2;

            //    var transform = new TranslateTransform(translationX * scale, translationY * scale);
            //    var matrix = group.Transform
            //    group.Transform.Value.Translate(translationX * scale, translationY * scale);
            //    group.Transform.Value.Rotate(90);

            //    this.DelimeterBox.RenderGeometry(group, scale, -this.DelimeterBox.Width / 2,
            //        -this.DelimeterBox.Depth + this.DelimeterBox.Width / 2);

            //    // Draw script box as superscript.
            //    if (this.ScriptBox != null)
            //        this.ScriptBox.RenderGeometry(geometry, scale, x, centerY - this.Kern - this.ScriptBox.Depth);
            //}
            //else
            //{
            //    // Draw delimeter and script boxes under base box.
            //    var centerY = y + this.BaseBox.Depth + this.DelimeterBox.Width;
            //    var translationX = x + this.DelimeterBox.Width / 2;
            //    var translationY = centerY - this.DelimeterBox.Width / 2;

            //    group.Transform.Value.Translate(translationX * scale, translationY * scale);
            //    group.Transform.Value.Rotate(90);
            //    this.DelimeterBox.RenderGeometry(group, scale, -this.DelimeterBox.Width / 2,
            //        -this.DelimeterBox.Depth + this.DelimeterBox.Width / 2);

            //    // Draw script box as subscript.
            //    if (this.ScriptBox != null)
            //        this.ScriptBox.RenderGeometry(geometry, scale, x, centerY + this.Kern + this.ScriptBox.Height);
            //}
        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
