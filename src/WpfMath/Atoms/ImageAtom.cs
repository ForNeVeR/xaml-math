using WpfMath.Boxes;
using WpfMath.Exceptions;

namespace WpfMath.Atoms
{
    internal class ImageAtom : Atom
    {
        public ImageAtom(Atom baseAtom,string path,string bounds)
        {
            ImageLocation = path;
            string[] boundsarr = bounds.Split(':');
            
            if (boundsarr.Length==2&&double.TryParse(boundsarr[0],out double imgW)&& double.TryParse(boundsarr[1], out double imgH))
            {
                ImageWidth = imgW;  ImageHeight = imgH;
            }
            else
            {
                throw new TexParseException($"The given bounds: {bounds} are invalid");
            }
        }

        public double ImageWidth
        {
            get;private set;
        }

        public double ImageHeight
        {
            get; private set;
        }

        public string ImageLocation
        {
            get; private set;
        }

        public override Box CreateBox(TexEnvironment environment)
        {
            return new ImageBox(environment,ImageLocation,ImageHeight,ImageWidth,ImageHeight/2);
        }
    }
}
