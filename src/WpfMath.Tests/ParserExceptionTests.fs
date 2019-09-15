module WpfMath.Tests.ParserExceptionTests

open System

open Xunit

open WpfMath
open WpfMath.Atoms
open WpfMath.Exceptions
open WpfMath.Parsers
open WpfMath.Tests.Utils

[<Fact>]
let ``Non-existing delimiter should throw exception``() =
    let markup = @"\left x\right)"
    assertParseThrows<TexParseException> markup

[<Fact>]
let ``\mathrm{} should throw exn``() =
    assertParseThrows<TexParseException> @"\mathrm{}"

[<Fact>]
let ``\sqrt should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sqrt"

[<Fact>]
let ``"\sum_ " should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sum_ "

[<Fact>]
let ``Incorrect command parser behavior should be detected``(): unit =
    let incorrectParser =
        { new ICommandParser with
             member __.ProcessCommand _ =
                 CommandProcessingResult(SpaceAtom(null), 0) }
    let parserRegistry = Map([| "dummy", incorrectParser |])
    let parser = TexFormulaParser(parserRegistry)
    let ex = Assert.Throws<TexParseException>(Action(fun () -> ignore <| parser.Parse("\dummy")))
    Assert.Contains("NextPosition = 0", ex.Message)
