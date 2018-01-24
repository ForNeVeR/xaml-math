module WpfMath.Tests.ParserTests

open System

open DeepEqual.Syntax
open Xunit

open WpfMath
open WpfMath.Exceptions
open WpfMath.Tests.Utils

let assertParseResult formula expected =
    let parser = TexFormulaParser()
    let result = parser.Parse(formula)
    result.WithDeepEqual(expected)
        .ExposeInternalsOf<TexFormula>()
        .ExposeInternalsOf<FencedAtom>()
        .Assert()

let assertParseThrows<'ex when 'ex :> exn> formula =
    let parser = TexFormulaParser()
    Assert.Throws<'ex>(Func<obj>(fun () -> upcast parser.Parse(formula)))

let textStyle = "text"
let rmStyle = "mathrm"
let itStyle = "mathit"
let calStyle = "mathcal"

let ``2+2`` = row [char '2'; symbol "plus"; char '2']
let ``\mathrm{2+2}`` = row [styledChar('2', rmStyle); symbol "plus"; styledChar('2', rmStyle)]
let ``\lim`` = row [styledChar('l', rmStyle); styledChar('i', rmStyle); styledChar('m', rmStyle)]
let ``\sin`` = row [styledChar('s', rmStyle); styledChar('i', rmStyle); styledChar('n', rmStyle)]

[<Fact>]
let ``2+2 should be parsed properly`` () =
    assertParseResult
    <| "2+2"
    <| formula ``2+2``

[<Theory>]
[<InlineData("(", ")", "lbrack", "rbrack")>]
[<InlineData("[", "]", "lsqbrack", "rsqbrack")>]
[<InlineData("{", "}", "lbrace", "rbrace")>]
[<InlineData("<", ">", "langle", "rangle")>]
let ``Delimiters should work`` (left : string, right : string, lResult : string, rResult : string) =
    assertParseResult
    <| sprintf @"\left%sa\right%s" left right
    <| (formula <| fenced (openBrace lResult) (char 'a') (closeBrace rResult))

[<Theory>]
[<InlineData(".", ")", true, false)>]
[<InlineData("(", ".", false, true)>]
let ``Empty delimiters should work`` (left : string, right : string, isLeftEmpty : bool, isRightEmpty : bool) =
    let empty = brace SymbolAtom.EmptyDelimiterName TexAtomType.Ordinary
    let leftBrace = if isLeftEmpty then empty else (openBrace "lbrack")
    let rightBrace = if isRightEmpty then empty else (closeBrace "rbrack")

    assertParseResult
    <| sprintf @"\left%sa\right%s" left right
    <| (formula <| fenced leftBrace (char 'a') rightBrace)

[<Fact>]
let ``Unmatched delimiters should work`` () =
    assertParseResult
    <| @"\left)a\right|"
    <| (formula <| fenced (closeBrace "rbrack") (char 'a') (brace "vert" TexAtomType.Ordinary))

[<Fact>]
let ``Non-existing delimiter should throw exception`` () =
    let markup = @"\left x\right)"
    Assert.Throws<TexParseException>(fun () -> TexFormulaParser().Parse(markup) |> ignore)

[<Fact>]
let ``Expression in braces should be parsed`` () =
    assertParseResult
    <| @"\left(2+2\right)"
    <| (formula <| fenced (openBrace "lbrack") ``2+2`` (closeBrace "rbrack"))

[<Fact>]
let ``Expression after the braces should be parsed`` () =
    assertParseResult
    <| @"\left(2+2\right) + 1"
    <| (formula <| row [ fenced (openBrace "lbrack") ``2+2`` (closeBrace "rbrack")
                         symbol "plus"
                         char '1' ])

[<Fact>]
let ``\text command should be supported`` () =
    assertParseResult
    <| @"\text{test}"
    <| (formula <| styledString textStyle "test")

[<Fact>]
let ``Spaces in \text shouldn't be ignored`` () =
    let textChar c = styledChar (c, textStyle)
    assertParseResult
    <| @"\text{a b c}"
    <| (formula <| row [textChar 'a'; space; textChar 'b'; space; textChar 'c'])

[<Fact>]
let ``\text should support Cyrillic`` () =
    let textChar c = styledChar (c, textStyle)
    assertParseResult
    <| @"\text{абв}"
    <| (formula <| styledString textStyle "абв")

[<Fact>]
let ``\mathrm should be parsed properly`` () =
    assertParseResult
    <| @"\mathrm{sin}"
    <| (formula <| row [styledChar('s', rmStyle); styledChar('i', rmStyle); styledChar('n', rmStyle)])

[<Fact>]
let ``\mathrm should be parsed properly for complex eqs`` () =
    assertParseResult
    <| @"\mathrm{\left(2+2\right)} + 1"
    <| (formula <| row [ fenced (openBrace "lbrack") ``\mathrm{2+2}`` (closeBrace "rbrack")
                         symbol "plus"
                         char '1' ])

[<Fact>]
let ``\mathit should be parsed properly`` () =
    assertParseResult
    <| @"\mathit{sin}"
    <| (formula <| row [styledChar('s', itStyle); styledChar('i', itStyle); styledChar('n', itStyle)])


[<Fact>]
let ``\mathcal should be parsed properly`` () =
    assertParseResult
    <| @"\mathcal{sin}"
    <| (formula <| row [styledChar('s', calStyle); styledChar('i', calStyle); styledChar('n', calStyle)])

[<Fact>]
let ``\mathrm{} should throw exn`` () =
    let parser = TexFormulaParser()
    let methodcall = (fun () -> parser.Parse(@"\mathrm{}") |> ignore)
    Assert.Throws<TexParseException>(methodcall)

[<Fact>]
let ``\lim should be parsed properly`` () =
    assertParseResult
    <| @"\lim_{n} x"
    <| (formula <| row [
                    opWithScripts ``\lim`` (char 'n') null (System.Nullable true);
                    char 'x'
                        ])

[<Fact>]
let ``{\lim} x should be parsed properly`` () =
    assertParseResult
    <| @"{\lim} x"
    <| (formula <| row [
                    group (op ``\lim`` (System.Nullable true));
                    char 'x'
                        ])

[<Fact>]
let ``\sin should be parsed properly`` () =
    assertParseResult
    <| @"\sin^{n} x"
    <| (formula <| row [
                    opWithScripts ``\sin`` null (char 'n') (System.Nullable false);
                    char 'x'
                        ])

[<Fact>]
let ``\int f should be parser properly`` () =
    assertParseResult
    <| @"\int f"
    <| (formula <| row [
                    op (symbolOp "int") (System.Nullable ())
                    char 'f'
                        ])

[<Fact>]
let ``{} should be parsed properly`` () =
    assertParseResult
    <| @"{}"
    <| (formula <| group (row []))

[<Fact>]
let ``Delimiter with scripts should be parsed properly`` () =
    assertParseResult
    <| @"\left(2+2\right)_a^b"
    <| (formula <| scripts (fenced (openBrace "lbrack") ``2+2`` (closeBrace "rbrack")) (char 'a') (char 'b'))

[<Fact>]
let ``\sqrt{} should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sqrt{}"

[<Fact>]
let ``"\sum_ " should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sum_ "
