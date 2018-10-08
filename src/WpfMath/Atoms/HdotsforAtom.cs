using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WpfMath.Boxes;
using WpfMath.Exceptions;

namespace WpfMath.Atoms
{
    /// <summary>
    /// Represents an atom that creates dots spanning a given number of columns.
    /// </summary>
    internal class HdotsforAtom : MulticolumnAtom
    {
        /// <summary>
        /// Initializes a new <see cref="HdotsforAtom"/> with the specified <paramref name="column"/>, <paramref name="columnspan"/> and <paramref name="spaceCoefficient"/>.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="column"></param>
        /// <param name="columnspan"></param>
        /// <param name="spaceCoefficient"></param>
        /// <param name="type"></param>
        public HdotsforAtom(SourceSpan source, byte column, byte columnspan, double spaceCoefficient, TexAtomType type = TexAtomType.Ordinary) : base(source, type)
        {
            Column = column;
            ColumnSpan = columnspan;
            SpaceCoefficient = spaceCoefficient;
            Width = 0.4;
            Height = 4;

        }

        /// <summary>
        /// Gets or sets the thin space multiplier for the dots.
        /// </summary>
        public double SpaceCoefficient { get; private set; }


        protected override Box CreateBoxCore(TexEnvironment environment)
        {
            Box dotbox = null;
            if (SymbolAtom.TryGetAtom(new SourceSpan("ldotp", 0, 5), out SymbolAtom dotatom))
            {
                dotbox = dotatom.CreateBox(environment);
            }
            else
            {
                throw new TexParseException($"The current math font does not contain a definition for the \"ldotp\" symbol.");
            }

            var spacebox = new SpaceAtom(this.Source, TexUnit.Mu, SpaceCoefficient * 3, 0, 0).CreateBox(environment);

            var resultBox = new HorizontalBox(dotbox);

            while (resultBox.TotalWidth < this.Width)
            {
                resultBox.Add(spacebox);
                resultBox.Add(dotbox);
            }

            return resultBox;
        }

    }
}
