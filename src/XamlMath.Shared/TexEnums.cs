namespace XamlMath;

public enum MatrixCellAlignment
{
    Left,
    Center,
    Aligned
}

public enum TexDelimeterType
{
    Over = 0,
    Under = 1
}

/// <summary>
/// Indicates the type of atom a mathematical symbol is.
/// </summary>
public enum TexAtomType
{
    None = -1,
    /// <summary>
    /// The atom is an ordinary atom like "&#x3a9;" or "&#x211c;".
    /// </summary>
    Ordinary = 0,
    /// <summary>
    /// The atom is a large operator like "&#x2211;" or "&#x222c;".
    /// </summary>
    BigOperator = 1,
    /// <summary>
    /// The atom is a binary operation atom like "&#xf7;" or "&#xd7;".
    /// </summary>
    BinaryOperator = 2,
    /// <summary>
    /// The atom is a relational atom like "&#x224c;" or "&#x224b;".
    /// </summary>
    Relation = 3,
    /// <summary>
    /// The atom is an opening atom like "&#x27e6;" or "&#x7b;".
    /// </summary>
    Opening = 4,
    /// <summary>
    /// The atom is a closing atom like "&#x27e7;" or "&#x7d;".
    /// </summary>
    Closing = 5,
    /// <summary>
    /// The atom is a punctuation atom like "&#x2c;" or "&#x3a;".
    /// </summary>
    Punctuation = 6,
    Inner = 7,
    /// <summary>
    /// The atom is an accented atom like "X&#x306;" or "O&#x308;".
    /// </summary>
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

/// <remarks>The numbers here correspond to the indices in <see cref="TexFormulaParser.DelimiterNames"/>.</remarks>
public enum TexDelimiter
{
    Brace = 0,
    Parenthesis = 1,
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
