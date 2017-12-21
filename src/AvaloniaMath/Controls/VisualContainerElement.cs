using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Media;
using Avalonia;
using Avalonia.Controls;

namespace WpfMath.Controls
{
    public class VisualContainerElement : Control
    {
        private Visual visual;

        public VisualContainerElement()
            : base()
        {
            this.visual = null;
        }

        public Visual Visual
        {
            get { return this.visual; }
            set
            {
//                RemoveVisualChild(this.visual);
                this.visual = value;
  //              AddVisualChild(this.visual);

                InvalidateMeasure();
                InvalidateVisual();
            }
        }

//        protected override int VisualChildrenCount
//        {
//            get { return 1; }
//        }
//        protected override Visual GetVisualChild(int index)
//        {
//            return this.visual;
//        }

        protected override Size MeasureOverride(Size availableSize)
        {
//            if (this.visual != null)
//                return this.visual.ContentBounds.Size;
            return base.MeasureOverride(availableSize);
        }
    }
}
