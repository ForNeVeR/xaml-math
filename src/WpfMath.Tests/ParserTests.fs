module WpfMath.Tests.ParserTests

open Xunit

open WpfMath.Tests.ApprovalTestUtils

[<Fact>]
let ``2+2``() =
    verifyParseResult "2+2"

[<Theory>]
[<InlineData("(", ")", "(", ")")>]
[<InlineData("[", "]", "lbrack", "rbrack")>]
[<InlineData("{", "}", "lbrace", "rbrace")>]
[<InlineData("<", ">", "langle", "rangle")>]
let delimiters(left : string, right : string, lResult : string, rResult : string) =
    verifyParseResultScenario
    <| sprintf "%s,%s" lResult rResult
    <| sprintf @"\left%sa\right%s" left right

[<Theory>]
[<InlineData(".", ")", true, false)>]
[<InlineData("(", ".", false, true)>]
let emptyDelimiters(left : string, right : string, isLeftEmpty : bool, isRightEmpty : bool) =
    verifyParseResultScenario
    <| sprintf "(%s,%s,%A,%A)" left right isLeftEmpty isRightEmpty
    <| sprintf @"\left%sa\right%s" left right

[<Fact>]
let unmatchedDelimiters() =
    verifyParseResult @"\left)a\right|"

[<Fact>]
let expressionInBraces() =
    verifyParseResult @"\left(2+2\right)"

[<Fact>]
let expressionAfterBraces() =
    verifyParseResult @"\left(2+2\right) + 1"

[<Fact>]
let textCommand() =
    verifyParseResult @"\text{test}"

[<Fact>]
let spacesInText() =
    verifyParseResult @"\text{a b c}"

[<Fact>]
let cyrillicText() =
    verifyParseResult @"\text{абв}"

[<Fact>]
let underscoreText() =
    verifyParseResult @"\text{_}"

[<Fact>]
let commandsInText() =
    verifyParseResult @"\text{\alpha \beta \unknowncommand}"

[<Fact>]
let mathrm() =
    verifyParseResult @"\mathrm{sin}"

[<Fact>]
let complexMathrm() =
    verifyParseResult @"\mathrm{\left(2+2\right)} + 1"

[<Fact>]
let mathit() =
    verifyParseResult @"\mathit{sin}"

[<Fact>]
let mathcal() =
    verifyParseResult @"\mathcal{sin}"

[<Fact>]
let lim() =
    verifyParseResult @"\lim_{n} x"

[<Fact>]
let limInCurlyBraces() =
    verifyParseResult @"{\lim} x"

[<Fact>]
let sin() =
    verifyParseResult @"\sin^{n} x"

[<Fact>]
let intF() =
    verifyParseResult @"\int f"

[<Fact>]
let emptyCurlyBraces() =
    verifyParseResult @"{}"

[<Fact>]
let delimiterWithScripts() =
    verifyParseResult @"\left(2+2\right)_a^b"

[<Fact>]
let textWithExpression() =
    verifyParseResult @"\text{2+2}"

[<Theory>]
[<InlineData("{red}1123");
  InlineData("{red}{1}123");
  InlineData("{red} 1123");
  InlineData("{red} {1}123")>]
let color(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\color%s" text

[<Theory>]
[<InlineData(@"{red}1123");
  InlineData(@"{red}{1}123");
  InlineData(@"{red} 1123");
  InlineData(@"{red} {1}123")>]
let colorbox(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\colorbox%s" text

[<Theory>]
[<InlineData("2x123");
  InlineData("2{x}123");
  InlineData("{2}x123");
  InlineData("{2}{x}123");
  InlineData(" 2 x123");
  InlineData("2 {x}123");
  InlineData(" 2{x}123")>]
let frac(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\frac%s" text

[<Theory>]
[<InlineData("1123");
  InlineData("{1}123");
  InlineData(" 1123");
  InlineData(" {1}123")>]
let overline(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\overline%s" text

[<Theory>]
[<InlineData("1123");
  InlineData("{1}123");
  InlineData(" 1123");
  InlineData(" {1}123")>]
let sqrt(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\sqrt%s" text

[<Theory>]
[<InlineData(" [2]1123");
  InlineData(" [ 2]{1}123");
  InlineData("[2 ] 1123");
  InlineData("[ 2 ] {1}123")>]
let sqrtWithOptArgument(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\sqrt%s" text

[<Theory>]
[<InlineData("1123");
  InlineData("{1}123");
  InlineData(" 1123");
  InlineData(" {1}123")>]
let underline(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\underline%s" text

[<Theory>]
[<InlineData("x^y_z");
  InlineData("x^y_{z}");
  InlineData("x^{y}_z");
  InlineData("x^{y}_{z}");
  InlineData("x^y_ z");
  InlineData("x ^ {y} _ {z}")>]
let scripts(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(" 1123");
  InlineData(" {1}123")>]
let textArgumentParsing(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\text%s" text

[<Fact>]
let hat() : unit =
    verifyParseResult @"{\hat T}"

[<Fact>]
let integral() =
    verifyParseResult @"\int_a^b"
