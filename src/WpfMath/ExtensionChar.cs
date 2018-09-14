using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    /// <summary>
    /// Extension character that contains character information for each of its parts.
    /// </summary>
    internal class ExtensionChar
    {
        /// <summary>
        /// Initializes a new extension character with the specified parts.
        /// </summary>
        /// <param name="top"></param>
        /// <param name="middle"></param>
        /// <param name="bottom"></param>
        /// <param name="repeat"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public ExtensionChar(CharInfo top, CharInfo middle, CharInfo bottom, CharInfo repeat,CharInfo left=null,CharInfo right=null)
        {
            this.Top = top;
            this.Middle = middle;
            this.Repeat = repeat;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
        }
        
        /// <summary>
        /// Gets or sets the top part of the extension character.
        /// </summary>
        public CharInfo Top{get;private set;}

        /// <summary>
        /// Gets or sets the middle part of the extension character.
        /// </summary>
        public CharInfo Middle{get;private set;}

        /// <summary>
        /// Gets or sets the bottom part of the extension character.
        /// </summary>
        public CharInfo Bottom{get;private set;}

        /// <summary>
        /// Gets or sets the repeated part of the extension character.
        /// </summary>
        public CharInfo Repeat{get;private set;}
        
        /// <summary>
        /// Gets or sets the left part of the extension character.
        /// </summary>
        /// <remarks>
        /// This should be used for only horizontal delimiters(eg: overbrace).
        /// <remarks>
        public CharInfo Left { get; private set; }
        
        /// <summary>
        /// Gets or sets the right part of the extension character.
        /// </summary>
        /// <remarks>
        /// This should be used for only horizontal delimiters(eg: underbrace).
        /// <remarks>
        public CharInfo Right { get; private set; }

    }
}
