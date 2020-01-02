using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WpfMath.Controls
{
    public class VisualContainerElement : FrameworkElement
    {
        private DrawingVisual visual;

        public VisualContainerElement()
            : base()
        {
            this.visual = null;
        }

        public DrawingVisual Visual
        {
            get { return this.visual; }
            set
            {
                RemoveVisualChild(this.visual);
                this.visual = value;
                AddVisualChild(this.visual);

                InvalidateMeasure();
                InvalidateVisual();
            }
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visual;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.visual != null)
                return this.visual.ContentBounds.Size;
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }
    }
}
