using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using WpfMath.Rendering;

namespace WpfMath
{
    /// <summary>
    /// Formula Utilities.
    /// </summary>
    public static class FormulaUtils
    {
        public static IDictionary<Box, Rect> GetVisibleBoxes(Box rootBox, double scale, double x, double y, Func<Box, bool> filter)
        {
            var result = new Dictionary<Box, Rect>();
            var walker = new BoxWalker(result, scale, box => box is CharBox && filter(box));
            walker.RenderElement(rootBox, x / scale, y / scale + rootBox.Height);
            return result;
        }
    }
}
