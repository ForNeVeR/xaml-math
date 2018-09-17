module WpfMath.Tests.Utils

open System
open System.Windows
open System.Windows.Media

open FSharp.Linq.RuntimeHelpers
open DeepEqual.Syntax
open Xunit

open WpfMath
open WpfMath.Atoms

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

let private toNullable = function
    | Some x -> Nullable x
    | None -> Nullable()

let charSrc (c : char) (source : SourceSpan) : CharAtom = CharAtom(source, c)
let symbolSrc (name : string) (aType : TexAtomType) (source : SourceSpan) : SymbolAtom =
    SymbolAtom(source, name, aType, false)
let opWithScriptsSrc (baseAtom : Atom)
                     (subscript : Atom)
                     (superscript : Atom)
                     (useVertScripts : bool option)
                     (source : SourceSpan)
            : BigOperatorAtom = BigOperatorAtom(source, baseAtom, subscript, superscript, toNullable useVertScripts)
let rowSrc (children : Atom seq) (source : SourceSpan) : RowAtom =
    children
    |> Seq.fold (fun row atom -> row.Add atom) (RowAtom(source))

let space = SpaceAtom(null)
let char (c : char) : CharAtom = charSrc c null
let styledChar (c : char) (style : string) : CharAtom = CharAtom(null, c, style)
let textChar c = styledChar c textStyle
let opWithScripts (baseAtom : Atom) (subscript : Atom) (superscript : Atom) (useVertScripts : bool option)
                  : BigOperatorAtom = opWithScriptsSrc baseAtom subscript superscript useVertScripts null
let op (baseAtom : Atom) (useVertScripts : bool option) : BigOperatorAtom =
    opWithScripts baseAtom null null useVertScripts
let scripts (baseAtom : Atom) (subscript : Atom) (superscript : Atom)
            : ScriptsAtom = ScriptsAtom(null, baseAtom, subscript, superscript)
let group (groupedAtom: Atom) : TypedAtom = TypedAtom(null, groupedAtom, TexAtomType.Ordinary, TexAtomType.Ordinary)
let symbol (name : string) : SymbolAtom = symbolSrc name TexAtomType.BinaryOperator null
let symbolOp (name : string) : SymbolAtom = SymbolAtom(null, name, TexAtomType.BigOperator, false)
let underline(body : Atom) : UnderlinedAtom = UnderlinedAtom(null, body)
let radical(body : Atom) : Radical = Radical(null, body)
let radicalWithDegree (degree : Atom) (body : Atom) : Radical = Radical(null, body, degree)
let row (children : Atom seq) : RowAtom = rowSrc children null
let fenced left body right : FencedAtom = FencedAtom(null, body, left, right)
let fraction (num : Atom) (denom : Atom) : FractionAtom = FractionAtom(null, num, denom, true)
let styledString (style : string) (text : string) : RowAtom =
    text
    |> Seq.map (fun c -> styledChar c style :> Atom)
    |> row
let overline(body : Atom) : OverlinedAtom = OverlinedAtom(null, body)
let foreColor (body : Atom) (color : Brush) = StyledAtom(null, body, null, color)
let backColor (body : Atom) (color : Brush) = StyledAtom(null, body, color, null)

let brace (name : string) (braceType : TexAtomType) : SymbolAtom = SymbolAtom(null, name, braceType, true)
let openBrace (name : string) : SymbolAtom = brace name TexAtomType.Opening
let closeBrace (name : string) : SymbolAtom = brace name TexAtomType.Closing

let brush : string -> Brush =
    let converter = BrushConverter()
    fun color -> downcast converter.ConvertFrom color
