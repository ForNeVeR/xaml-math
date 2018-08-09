using WpfMath.Boxes;
using WpfMath.Exceptions;

namespace WpfMath.Atoms
{
    internal class ImageAtom : Atom
    {
        public ImageAtom(SourceSpan source,Atom baseAtom,string path,string bounds):base(source)
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

        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            return new ImageBox(environment,ImageLocation,ImageHeight,ImageWidth,ImageHeight/2);
        }
    }
}
