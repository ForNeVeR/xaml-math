module WpfMath.Tests.ParserTests

open Xunit

open WpfMath.Tests.ApprovalTestUtils

[<Fact>]
let ``2+2``() =
    verifyParseResult "2+2"

[<Theory>]
[<InlineData("(", ")", "(", ")")>]
[<InlineData("[", "]", "lbrack", "rbrack")>]
[<InlineData(@"\{", @"\}", "lbrace", "rbrace")>]
[<InlineData("<", ">", "langle", "rangle")>]
[<InlineData(@"\vert ", @"\vert", "vert", "vert")>]
[<InlineData(@"\\ ", @"\\", "backslash", "backslash")>]
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
let oneNonAsciiSymbolText() =
    verifyParseResult @"\text{А}_{\text{И}}"

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
[<InlineData("2x123");
  InlineData("2{x}123");
  InlineData("{2}x123");
  InlineData("{2}{x}123");
  InlineData(" 2 x123");
  InlineData("2 {x}123");
  InlineData(" 2{x}123")>]
let binom(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| sprintf @"\binom%s" text

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

[<Fact>]
let simpleMatrix() =
    verifyParseResult @"\matrix{4&78&3 \\ 5 & 9  & 82 }"

[<Fact>]
let simpleCases() =
    verifyParseResult @"\cases{x,&if x > 0;\cr -x,& otherwise.}"

[<Fact>]
let nestedMatrix() =
    verifyParseResult @"\matrix{4&78&3\\ 57 & {\matrix{78 \\ 12}}  & 20782 }"

[<Fact>]
let piecewiseDefinedFunction() =
    verifyParseResult @"f(x) = \cases{1/3 & if \thinspace 0\le x\le 1;\cr 2/3 & if \thinspace 3\le x \le 4; \cr 0 & elsewhere.\cr}"

[<Fact>]
let matrixExpression() =
    verifyParseResult @"v \times w = \left( \matrix{v_2 w_3 - v_3 w_2 \\ v_3 w_1 - v_1 w_3 \\ v_1 w_2 - v_2 w_1} \right) where v= \left(\matrix{ v_1 \\ v_2 \\ v_3 }\right), w= \left( \matrix{w_1 \\ w_2  \\ w_3} \right)"

[<Fact>]
let bigMatrixExpression() =
    verifyParseResult @"\Gamma_{\mu \rho} ^{\sigma}= \pmatrix{\pmatrix{0 & 0 & 0 \\ 0 & -r & 0 \\ 0 & 0 & -r sin^2(\theta)} \\ \pmatrix{0 & \frac{1}{r} & 0 \\ \frac{1}{r} & 0 & 0 \\ 0 & 0 & -\sin(\theta) \cos(\theta)} \\ \pmatrix{0 & 0 & \frac{1}{r} \\ 0 & 0 & \frac{1}{\tan(\theta)} \\ \frac{1}{r} & \frac{1}{\tan(\theta)} & 0 }}"

[<Theory>]
[<InlineData(@"\color {red} x");
  InlineData(@"\color [] {red} x");
  InlineData(@"\color [gray] {0.5} x");
  InlineData(@"\color [rgb] {0.5, 0.5, 0.5} x");
  InlineData(@"\color [RGB] {128, 128, 128} x");
  InlineData(@"\color [cmyk] {0.5, 0.5, 0.5, 0.5} x");
  InlineData(@"\color [HTML] {abcdef} x");
  InlineData(@"\colorbox {red} x");
  InlineData(@"\colorbox [] {red} x");
  InlineData(@"\colorbox [gray] {0.5} x");
  InlineData(@"\colorbox [rgb] {0.5, 0.5, 0.5} x");
  InlineData(@"\colorbox [RGB] {128, 128, 128} x");
  InlineData(@"\colorbox [cmyk] {0.5, 0.5, 0.5, 0.5} x");
  InlineData(@"\colorbox [HTML] {abcdef} x")>]
let colorModels(text: string): unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\color {red, 0.1} x");
  InlineData(@"\color [] {red, 0.1} x");
  InlineData(@"\color [gray] {0.5, 0.1} x");
  InlineData(@"\color [argb] {0.1, 0.5, 0.5, 0.5} x");
  InlineData(@"\color [rgba] {0.5, 0.5, 0.5, 0.1} x");
  InlineData(@"\color [ARGB] {25, 128, 128, 128} x");
  InlineData(@"\color [RGBA] {128, 128, 128, 25} x");
  InlineData(@"\color [cmyk] {0.5, 0.5, 0.5, 0.5, 0.1} x");
  InlineData(@"\color [HTML] {abcdef19} x");
  InlineData(@"\colorbox {red, 0.1} x");
  InlineData(@"\colorbox [] {red, 0.1} x");
  InlineData(@"\colorbox [gray] {0.5, 0.1} x");
  InlineData(@"\colorbox [argb] {0.1, 0.5, 0.5, 0.5} x");
  InlineData(@"\colorbox [rgba] {0.5, 0.5, 0.5, 0.1} x");
  InlineData(@"\colorbox [ARGB] {25, 128, 128, 128} x");
  InlineData(@"\colorbox [RGBA] {128, 128, 128, 25} x");
  InlineData(@"\colorbox [cmyk] {0.5, 0.5, 0.5, 0.5, 0.1} x");
  InlineData(@"\colorbox [HTML] {abcdef19} x")>]
let colorModelsWithOpacity(text: string): unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Fact>]
let multilineFormula(): unit =
    verifyParseResult @"line 1\\line 2\\line 3"

[<Theory>]
[<InlineData(@"\matrix{line 1}\\line 2");
  InlineData(@"\pmatrix{line 1}\\line 2")>]
let newLineAfterMatrix(text: string): unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text
