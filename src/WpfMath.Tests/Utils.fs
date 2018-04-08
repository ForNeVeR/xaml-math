module WpfMath.Tests.Utils

open System
open System.Windows

open WpfMath

let initializeFontResourceLoading =
    let monitor = obj()
    fun () ->
        lock monitor (fun () ->
            if not(UriParser.IsKnownScheme "pack")
            then new Application() |> ignore)

let formula (root : Atom) : TexFormula =
    TexFormula(RootAtom = root)

let space = SpaceAtom()
let char (c : char) : CharAtom = CharAtom(c)
let styledChar (c : char, style:string) : CharAtom = CharAtom(c, style)
let op (baseAtom : Atom) (useVertScripts : System.Nullable<bool>)  : BigOperatorAtom = BigOperatorAtom(baseAtom, null, null, useVertScripts)
let opWithScripts (baseAtom : Atom) (subscript : Atom) (superscript : Atom) (useVertScripts : System.Nullable<bool>)
            : BigOperatorAtom = BigOperatorAtom(baseAtom, subscript, superscript, useVertScripts)
let scripts (baseAtom : Atom) (subscript : Atom) (superscript : Atom)
            : ScriptsAtom = ScriptsAtom(baseAtom, subscript, superscript)
let group (groupedAtom: Atom) : TypedAtom = TypedAtom(groupedAtom, TexAtomType.Ordinary, TexAtomType.Ordinary)
let symbol (name : string) : SymbolAtom = SymbolAtom(name, TexAtomType.BinaryOperator, false)
let symbolOp (name : string) : SymbolAtom = SymbolAtom(name, TexAtomType.BigOperator, false)
let row (children : Atom seq) : RowAtom =
    children
    |> Seq.fold (fun row atom -> row.Add atom) (RowAtom())
let fenced left body right : FencedAtom = FencedAtom(body, left, right)
let styledString (style : string) (text : string) : RowAtom =
    text
    |> Seq.map (fun c -> styledChar (c, style) :> Atom)
    |> row

let brace (name : string) (braceType : TexAtomType) : SymbolAtom = SymbolAtom(name, braceType, true)
let openBrace (name : string) : SymbolAtom = brace name TexAtomType.Opening
let closeBrace (name : string) : SymbolAtom = brace name TexAtomType.Closing
