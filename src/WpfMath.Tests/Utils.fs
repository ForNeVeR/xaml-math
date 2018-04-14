module WpfMath.Tests.Utils

open System
open System.Windows

open FSharp.Linq.RuntimeHelpers
open DeepEqual.Syntax
open Xunit

open WpfMath

let textStyle = "text"
let rmStyle = "mathrm"
let itStyle = "mathit"
let calStyle = "mathcal"

let initializeFontResourceLoading =
    let monitor = obj()
    fun () ->
        lock monitor (fun () ->
            if not(UriParser.IsKnownScheme "pack")
            then new Application() |> ignore)

let formula (root : Atom) : TexFormula =
    TexFormula(RootAtom = root)

let private createComparer formula expected =
    let parser = TexFormulaParser()
    let result = parser.Parse(formula)
    result.WithDeepEqual(expected)
        .ExposeInternalsOf<TexFormula>()
        .ExposeInternalsOf<FencedAtom>()

let assertParseResultWithSource (formula : string) (expected : TexFormula) : unit =
    (createComparer formula expected).Assert()

let assertParseResult (formula : string) (expected : TexFormula) : unit =
    let toExpression = LeafExpressionConverter.QuotationToLambdaExpression
    (createComparer formula expected)
        .IgnoreProperty<Atom>(toExpression(<@ Func<Atom, obj>(fun a -> upcast a.Source) @>))
        .Assert()

let assertParseThrows<'ex when 'ex :> exn> formula =
    let parser = TexFormulaParser()
    Assert.Throws<'ex>(Func<obj>(fun () -> upcast parser.Parse(formula)))

let src (string : string) (start : int) (len : int) = SourceSpan(string, start, len)

let charSrc (c : char) (source : SourceSpan) : CharAtom = CharAtom(source, c)
let symbolSrc (name : string) (source : SourceSpan) : SymbolAtom =
    SymbolAtom(source, name, TexAtomType.BinaryOperator, false)
let rowSrc (children : Atom seq) (source : SourceSpan) : RowAtom =
    children
    |> Seq.fold (fun row atom -> row.Add atom) (RowAtom(source))

let space = SpaceAtom(null)
let char (c : char) : CharAtom = charSrc c null
let styledChar (c : char) (style : string) : CharAtom = CharAtom(null, c, style)
let textChar c = styledChar c textStyle
let op (baseAtom : Atom) (useVertScripts : System.Nullable<bool>)  : BigOperatorAtom = BigOperatorAtom(baseAtom, null, null, useVertScripts)
let opWithScripts (baseAtom : Atom) (subscript : Atom) (superscript : Atom) (useVertScripts : System.Nullable<bool>)
            : BigOperatorAtom = BigOperatorAtom(baseAtom, subscript, superscript, useVertScripts)
let scripts (baseAtom : Atom) (subscript : Atom) (superscript : Atom)
            : ScriptsAtom = ScriptsAtom(null, baseAtom, subscript, superscript)
let group (groupedAtom: Atom) : TypedAtom = TypedAtom(null, groupedAtom, TexAtomType.Ordinary, TexAtomType.Ordinary)
let symbol (name : string) : SymbolAtom = symbolSrc name null
let symbolOp (name : string) : SymbolAtom = SymbolAtom(null, name, TexAtomType.BigOperator, false)
let row (children : Atom seq) : RowAtom = rowSrc children null
let fenced left body right : FencedAtom = FencedAtom(null, body, left, right)
let styledString (style : string) (text : string) : RowAtom =
    text
    |> Seq.map (fun c -> styledChar c style :> Atom)
    |> row

let brace (name : string) (braceType : TexAtomType) : SymbolAtom = SymbolAtom(null, name, braceType, true)
let openBrace (name : string) : SymbolAtom = brace name TexAtomType.Opening
let closeBrace (name : string) : SymbolAtom = brace name TexAtomType.Closing
