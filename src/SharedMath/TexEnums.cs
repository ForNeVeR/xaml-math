using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMath
{
    public enum TexDelimeterType
    {
        Over = 0,
        Under = 1
    }

    public enum TexAtomType
    {
        None = -1,
        Ordinary = 0,
        BigOperator = 1,
        BinaryOperator = 2,
        Relation = 3,
        Opening = 4,
        Closing = 5,
        Punctuation = 6,
        Inner = 7,
        Accent = 10,
    }

    public enum TexAlignment
    {
        Left = 0,
        Right = 1,
        Center = 2,
        Top = 3,
        Bottom = 4
    }

    public enum TexDelimeter
    {
        Brace = 0,
        SquareBracket = 1,
        Bracket = 2,
        LeftArrow = 3,
        RightArrow = 4,
        LeftRightArrow = 5,
        DoubleLeftArrow = 6,
        DoubleRightArrow = 7,
        DoubleLeftRightArrow = 8,
        SingleLine = 9,
        DoubleLine = 10,
    }

    public enum TexStyle
    {
        Display = 0,
        Text = 2,
        Script = 4,
        ScriptScript = 6,
    }

    public enum TexUnit
    {
        Em = 0,
        Ex = 1,
        Pixel = 2,
        Point = 3,
        Pica = 4,
        Mu = 5
    }
}
