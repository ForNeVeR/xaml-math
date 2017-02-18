namespace WpfMath.Tests

open DeepEqual.Syntax
open Xunit

open WpfMath
open WpfMath.Tests.Utils

type ParserTests() =
    do TexFormulaParser.Initialize()

    let assertParseResult formula expected =
        let parser = TexFormulaParser()
        let result = parser.Parse(formula)
        result.WithDeepEqual(expected).ExposeInternalsOf<TexFormula>().Assert()

    [<Fact>]
    let ``2+2 should be parsed properly`` () =
        assertParseResult
        <| "2+2"
        <| (formula [char '2'; plus; char '2'])
