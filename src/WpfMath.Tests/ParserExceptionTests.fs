module WpfMath.Tests.ParserExceptionTests

open System
open System.Collections.Generic

open Xunit

open WpfMath
open WpfMath.Atoms
open WpfMath.Colors
open WpfMath.Exceptions
open WpfMath.Parsers
open WpfMath.Tests.Utils

[<Fact>]
let ``Non-existing delimiter should throw a TexParseException``(): unit =
    let markup = @"\left x\right)"
    assertParseThrows<TexParseException> markup |> ignore

[<Fact>]
let ``Non-existing escaped delimiter should throw a TexParseException``(): unit =
    let markup = @"\left\x\right)"
    assertParseThrows<TexParseException> markup |> ignore

[<Fact>]
let ``{ shouldn't be considered as a valid delimiter``(): unit =
    let markup = @"\left{"
    let ex = assertParseThrows<TexParseException> markup
    Assert.Contains("Illegal end,  missing '}'", ex.Message)

[<Fact>]
let ``\ shouldn't be considered as a valid delimiter``(): unit =
    let markup = @"\left\"
    let ex = assertParseThrows<TexParseException> markup
    Assert.Contains("unfinished escape sequence", ex.Message, StringComparison.InvariantCultureIgnoreCase)

[<Fact>]
let ``\sqrt should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sqrt"

[<Fact>]
let ``"\sum_ " should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sum_ "

[<Fact>]
let ``"\frac{}" should throw a TexParseException``(): unit =
    let ex = assertParseThrows<TexParseException> @"\frac{}"
    Assert.Equal("An element is missing", ex.Message)

[<Fact>]
let ``"\binom{}" should throw a TexParseException``(): unit =
    let ex = assertParseThrows<TexParseException> @"\binom{}"
    Assert.Equal("An element is missing", ex.Message)

[<Fact>]
let ``"\color" should throw a TexParseException``(): unit =
    let ex = assertParseThrows<TexParseException> @"\color"
    Assert.Equal("An element is missing", ex.Message)

[<Fact>]
let ``"\color{red}" should throw a TexParseException``(): unit =
    let ex = assertParseThrows<TexParseException> @"\color{red}"
    Assert.Equal("An element is missing", ex.Message)

[<Fact>]
let ``Incorrect command parser behavior should be detected``(): unit =
    let incorrectParser =
        { new ICommandParser with
             member _.ProcessCommand _ =
                 CommandProcessingResult(SpaceAtom(null), 0) }
    let parserRegistry = Map([| "dummy", incorrectParser |])
    let parser = TexFormulaParser(parserRegistry, Dictionary(), PredefinedColorParser.Instance)
    let ex = Assert.Throws<TexParseException>(Action(fun () -> ignore <| parser.Parse("\dummy")))
    Assert.Contains("NextPosition = 0", ex.Message)

[<Theory>]
[<InlineData(@"\color [nonexistent123] {red} x");
  InlineData(@"\colorbox [nonexistent123] {red} x")>]
let ``Nonexistent color model throws a TexParseException``(text: string): unit =
    let ex = assertParseThrows<TexParseException> text
    Assert.Contains("nonexistent123", ex.Message)

[<Theory>]
[<InlineData(@"\color {reddit} x");
  InlineData(@"\colorbox {reddit} x")>]
let ``Nonexistent color throws a TexParseException``(text: string): unit =
    let ex = assertParseThrows<TexParseException> text
    Assert.Contains("reddit", ex.Message)

[<Theory>]
[<InlineData(@"\color [gray] {x} x", "gray");
  InlineData(@"\color [gray] {1.01} x", "gray");
  InlineData(@"\color [argb] {2, 0.5, 0.5, 0.5} x", "argb");
  InlineData(@"\color [argb] {x, 0.5, 0.5, 0.5} x", "argb");
  InlineData(@"\color [ARGB] {256, 128, 128, 128} x", "ARGB");
  InlineData(@"\color [ARGB] {x, 128, 128, 128} x", "ARGB");
  InlineData(@"\color [cmyk] {2, 0.5, 0.5, 0.5, 0.1} x", "cmyk");
  InlineData(@"\color [cmyk] {x, 0.5, 0.5, 0.5, 0.1} x", "cmyk");
  InlineData(@"\color [HTML] {wwwwwwww} x", "HTML")>]
let ``Invalid color numbers throw exceptions``(formula: string, colorModel: string): unit =
    let ex = assertParseThrows<TexParseException> formula
    Assert.Contains(colorModel, ex.Message)
