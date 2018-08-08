using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfMath.Exceptions;
using WpfMath.Rendering;
using WpfMath.Utils;

namespace WpfMath.Boxes
{
    /// <summary>
    /// Box representing an image.
    /// </summary>
    internal class ImageBox:Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBox"/>.
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="imagepath">The path of the image.</param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="shift"></param>
        public ImageBox(TexEnvironment environment,string imagepath, double height, double width, double shift)
        {
            ImageLocation = imagepath;
            this.Width = width;
            this.Height = height;
            this.Shift = shift;
            this.Foreground = environment.Foreground;//not needed
            this.Background = environment.Background;	//Not needed
        }

        /// <summary>
        /// Gets or sets the location of the image.
        /// </summary>
        public string ImageLocation
        {
            get;private set;
        }

        public override void RenderTo(IElementRenderer renderer, double x, double y)
        {
            var color = Foreground ?? Brushes.Black;
            var rectangle = new Rect(x, y - Height, Width, Height);
            renderer.RenderRectangle(rectangle, color, Brushes.Transparent);
            if (File.Exists(ImageLocation))
            {
                renderer.RenderImage(rectangle, ImageLocation);
            }
            else
            {
                throw new TexParseException($"The given file: {ImageLocation} could not be found");
            }

        }

        public override int GetLastFontId()
        {
            return TexFontUtilities.NoFontId;
        }
    }
}
