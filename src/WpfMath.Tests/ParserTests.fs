module WpfMath.Tests.ParserTests

open Xunit

open WpfMath
open WpfMath.Atoms
open WpfMath.Exceptions
open WpfMath.Tests.Utils

let private ``123`` : Atom seq = [| char '1'; char '2'; char '3' |] |> Seq.map (fun x -> upcast x)
let ``2+2`` = row [char '2'; symbol "plus"; char '2']
let ``\mathrm{2+2}`` = row [styledChar '2' rmStyle; symbol "plus"; styledChar '2' rmStyle]
let ``\lim`` = row [styledChar 'l' rmStyle; styledChar 'i' rmStyle; styledChar 'm' rmStyle]
let ``\sin`` = row [styledChar 's' rmStyle; styledChar 'i' rmStyle; styledChar 'n' rmStyle]
let redBrush = brush "#ed1b23"

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
    assertParseResult
    <| @"\text{a b c}"
    <| (formula <| row [textChar 'a'; space; textChar 'b'; space; textChar 'c'])

[<Fact>]
let ``\text should support Cyrillic`` () =
    assertParseResult
    <| @"\text{абв}"
    <| (formula <| styledString textStyle "абв")

[<Fact>]
let ``\mathrm should be parsed properly`` () =
    assertParseResult
    <| @"\mathrm{sin}"
    <| (formula <| row [styledChar 's' rmStyle; styledChar 'i' rmStyle; styledChar 'n' rmStyle])

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
    <| (formula <| row [styledChar 's' itStyle; styledChar 'i' itStyle; styledChar 'n' itStyle])


[<Fact>]
let ``\mathcal should be parsed properly`` () =
    assertParseResult
    <| @"\mathcal{sin}"
    <| (formula <| row [styledChar 's' calStyle; styledChar 'i' calStyle; styledChar 'n' calStyle])

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
                    opWithScripts ``\lim`` (char 'n') null (Some true)
                    char 'x'
                        ])

[<Fact>]
let ``{\lim} x should be parsed properly`` () =
    assertParseResult
    <| @"{\lim} x"
    <| (formula <| row [
                    group (op ``\lim`` (Some true))
                    char 'x'
                        ])

[<Fact>]
let ``\sin should be parsed properly`` () =
    assertParseResult
    <| @"\sin^{n} x"
    <| (formula <| row [
                    opWithScripts ``\sin`` null (char 'n') (Some false)
                    char 'x'
                        ])

[<Fact>]
let ``\int f should be parser properly`` () =
    assertParseResult
    <| @"\int f"
    <| (formula <| row [
                    op (symbolOp "int") (None)
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

let ``\text doesn't create any SymbolAtoms``() =
    assertParseResult
    <| @"\text{2+2}"
    <| (formula <| row [char '2'; char '+'; char '2'])

[<Fact>]
let ``\sqrt should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sqrt"

[<Fact>]
let ``"\sum_ " should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sum_ "

[<Theory>]
[<InlineData(@"\color{red}1123");
  InlineData(@"\color{red}{1}123");
  InlineData(@"\color{red} 1123");
  InlineData(@"\color{red} {1}123")>]
let ``\color should parse arguments properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast foreColor (char '1') redBrush; yield! ``123`` }))

[<Theory>]
[<InlineData(@"\colorbox{red}1123");
  InlineData(@"\colorbox{red}{1}123");
  InlineData(@"\colorbox{red} 1123");
  InlineData(@"\colorbox{red} {1}123")>]
let ``\colorbox should parse arguments properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast backColor (char '1') redBrush; yield! ``123`` }))

[<Theory>]
[<InlineData(@"\frac2x123");
  InlineData(@"\frac2{x}123");
  InlineData(@"\frac{2}x123");
  InlineData(@"\frac{2}{x}123");
  InlineData(@"\frac 2 x123");
  InlineData(@"\frac2 {x}123");
  InlineData(@"\frac 2{x}123")>]
let ``\frac should parse arguments properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast fraction (char '2') (char 'x'); yield! ``123`` }))

[<Theory>]
[<InlineData(@"\overline1123");
  InlineData(@"\overline{1}123");
  InlineData(@"\overline 1123");
  InlineData(@"\overline {1}123")>]
let ``\overline should parse arguments properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast overline(char '1'); yield! ``123`` }))

[<Theory>]
[<InlineData(@"\sqrt1123");
  InlineData(@"\sqrt{1}123");
  InlineData(@"\sqrt 1123");
  InlineData(@"\sqrt {1}123")>]
let ``\sqrt should parse arguments properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast radical(char '1'); yield! ``123`` }))

[<Theory>]
[<InlineData(@"\sqrt [2]1123");
  InlineData(@"\sqrt [ 2]{1}123");
  InlineData(@"\sqrt[2 ] 1123");
  InlineData(@"\sqrt[ 2 ] {1}123")>]
let ``\sqrt should parse optional argument properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast radicalWithDegree (char '2') (char '1'); yield! ``123`` }))

[<Theory>]
[<InlineData(@"\underline1123");
  InlineData(@"\underline{1}123");
  InlineData(@"\underline 1123");
  InlineData(@"\underline {1}123")>]
let ``\underline should parse arguments properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast underline(char '1'); yield! ``123`` }))

[<Theory>]
[<InlineData("x^y_z");
  InlineData("x^y_{z}");
  InlineData("x^{y}_z");
  InlineData("x^{y}_{z}");
  InlineData("x^y_ z");
  InlineData("x ^ {y} _ {z}")>]
let ``Scripts should be parsed properly``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula <| scripts (char 'x') (char 'z') (char 'y'))

[<Theory>]
[<InlineData(@"\text 1123");
  InlineData(@"\text {1}123")>]
let ``\text command should support extended argument parsing``(text : string) : unit =
    assertParseResult
    <| text
    <| (formula (row <| seq { yield upcast styledChar '1' textStyle; yield! ``123`` }))
