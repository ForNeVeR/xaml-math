module WpfMath.Tests.Utils

open WpfMath

let formula (atoms : Atom seq) : TexFormula =
    let root = RowAtom()
    root.Elements.AddRange(atoms)
    TexFormula(RootAtom = root)

let char (c : char) : CharAtom = CharAtom(c)
let plus : SymbolAtom = SymbolAtom("plus", TexAtomType.BinaryOperator, false)
