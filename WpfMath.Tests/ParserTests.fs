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

    let ``2+2`` = row [char '2'; symbol "plus"; char '2']

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