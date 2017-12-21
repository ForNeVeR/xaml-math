using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using WpfMath.Avalonia;


namespace WpfMath
{
    // Represents graphical box that is part of math expression, and can itself contain child boxes.
    public abstract class Box
    {
        private List<Box> children;
        private ReadOnlyCollection<Box> childrenReadOnly;

        internal Box(TexEnvironment environment)
            : this(environment.Foreground, environment.Background)
        {
        }

        protected Box()
            : this(null, null)
        {
        }

        protected Box(Brush foreground, Brush background)
        {
            this.children = new List<Box>();
            this.childrenReadOnly = new ReadOnlyCollection<Box>(this.children);
            this.Foreground = foreground;
            this.Background = background;
        }

        public ReadOnlyCollection<Box> Children
        {
            get { return this.childrenReadOnly; }
        }

        public Brush Foreground
        {
            get;
            set;
        }

        public Brush Background
        {
            get;
            set;
        }

        public double TotalHeight
        {
            get { return this.Height + this.Depth; }
        }

        public double TotalWidth
        {
            get { return this.Width + this.Italic; }
        }

        public double Italic
        {
            get;
            set;
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }

        public double Depth
        {
            get;
            set;
        }

        public double Shift
        {
            get;
            set;
        }

        /// <summary>
        /// Draws the box into a <see cref="DrawingContext"/> while providing guidelines for WPF render to snap
        /// the box boundaries onto the device pixel grid.
        /// </summary>
        public void DrawWithGuidelines(DrawingContext drawingContext, double scale, double x, double y)
        {
            //TODO: need to restore guidelines!!
           
//            var guidelines = new GuidelineSet
//            {
//                GuidelinesX = { scale * x, scale * (x + TotalWidth) },
//                GuidelinesY = { scale * y, scale * (y + TotalHeight) }
//            };
//            drawingContext.PushGuidelineSet(guidelines);

            DrawBackground(drawingContext, scale, x, y);
            Draw(drawingContext, scale, x, y);

//            drawingContext.Pop();
        }

        public abstract void Draw(DrawingContext drawingContext, double scale, double x, double y);

        public virtual void RenderGeometry(GeometryGroup geometry, double scale, double x, double y)
        {

        }

        public virtual void Add(Box box)
        {
            this.children.Add(box);
        }

        public virtual void Add(int position, Box box)
        {
            this.children.Insert(position, box);
        }

        public abstract int GetLastFontId();

        private void DrawBackground(DrawingContext drawingContext, double scale, double x, double y)
        {
            if (Background != null)
            {
                //TODO: creating pen every time
                var pen = new Pen(Background);
                // Fill background of box with color:
                drawingContext.DrawRectangle(
                    pen,
                    new Rect(scale * x, scale * (y - Height),
                    scale * TotalWidth,
                    scale * TotalHeight));
            }
        }
    }
}
