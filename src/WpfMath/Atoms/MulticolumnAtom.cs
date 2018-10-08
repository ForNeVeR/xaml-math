using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMath.Boxes;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents an atom that can span >1 column.
    /// </summary>
    class MulticolumnAtom:Atom
    {
        protected MulticolumnAtom(SourceSpan source, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public byte Column { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public byte ColumnSpan { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected Atom MainAtom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Width { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool CanbeShown { get; internal set; }



        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            return MainAtom.CreateBox(environment);
        }
    }
}
