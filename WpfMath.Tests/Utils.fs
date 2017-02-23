module WpfMath.Tests.Utils

open WpfMath

let formula (root : Atom) : TexFormula =
    TexFormula(RootAtom = root)

let char (c : char) : CharAtom = CharAtom(c)
let styledChar(c : char, style:string) : CharAtom = CharAtom(c, style)
let symbol (name : string) : SymbolAtom = SymbolAtom(name, TexAtomType.BinaryOperator, false)
let row (children : Atom seq) : RowAtom =
    let result = RowAtom()
    result.Elements.AddRange(children)
    result
let fenced left body right : FencedAtom = FencedAtom(body, left, right)

let openBrace (name : string) : SymbolAtom = SymbolAtom(name, TexAtomType.Opening, true)
let closeBrace (name : string) : SymbolAtom = SymbolAtom(name, TexAtomType.Closing, true)
