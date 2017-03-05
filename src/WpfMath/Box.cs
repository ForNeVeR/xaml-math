using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

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
    }
}
