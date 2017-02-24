namespace WpfMath.Tests

open DeepEqual.Syntax
open Xunit

open WpfMath
open WpfMath.Tests.Utils

type ParserTests() =
    let assertParseResult formula expected =
        let parser = TexFormulaParser()
        let result = parser.Parse(formula)
        result.WithDeepEqual(expected)
            .ExposeInternalsOf<TexFormula>()
            .ExposeInternalsOf<FencedAtom>()
            .Assert()

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
    let ``Expression with symbols from unsupported charset should throw exn`` () =
        let parser = TexFormulaParser()
        let methodcall = (fun () -> parser.Parse("абвгд") |> ignore)
        Assert.Throws<TexParseException>(methodcall)

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
    let ``\mathrm should be parsed properly`` () =
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