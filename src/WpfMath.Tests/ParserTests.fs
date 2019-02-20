module WpfMath.Tests.ParserTests

open Xunit

open WpfMath.Exceptions
open WpfMath.Tests.ApprovalTestUtils
open WpfMath.Tests.Utils

[<Fact>]
let ``2+2 should be parsed properly`` () =
    verifyParseResult "2+2"

[<Theory>]
[<InlineData("(", ")", "(", ")")>]
[<InlineData("[", "]", "lbrack", "rbrack")>]
[<InlineData("{", "}", "lbrace", "rbrace")>]
[<InlineData("<", ">", "langle", "rangle")>]
let delimiters (left : string, right : string, lResult : string, rResult : string) =
    verifyParseResultScenario
    <| sprintf "%s,%s" lResult rResult
    <| sprintf @"\left%sa\right%s" left right

[<Theory>]
[<InlineData(".", ")", true, false)>]
[<InlineData("(", ".", false, true)>]
let ``Empty delimiters should work`` (left : string, right : string, isLeftEmpty : bool, isRightEmpty : bool) =
    verifyParseResultScenario
    <| sprintf "(%s,%s,%A,%A)" left right isLeftEmpty isRightEmpty
    <| sprintf @"\left%sa\right%s" left right

[<Fact>]
let ``Unmatched delimiters should work`` () =
    verifyParseResult @"\left)a\right|"

[<Fact>]
let ``Non-existing delimiter should throw exception`` () =
    let markup = @"\left x\right)"
    assertParseThrows<TexParseException> markup

[<Fact>]
let ``Expression in braces should be parsed`` () =
    verifyParseResult @"\left(2+2\right)"

[<Fact>]
let ``Expression after the braces should be parsed`` () =
    verifyParseResult @"\left(2+2\right) + 1"

[<Fact>]
let ``\text command should be supported`` () =
    verifyParseResult @"\text{test}"

[<Fact>]
let ``Spaces in \text shouldn't be ignored`` () =
    verifyParseResult @"\text{a b c}"

[<Fact>]
let ``\text should support Cyrillic`` () =
    verifyParseResult @"\text{абв}"

[<Fact>]
let ``\mathrm should be parsed properly`` () =
    verifyParseResult @"\mathrm{sin}"

[<Fact>]
let ``\mathrm should be parsed properly for complex eqs`` () =
    verifyParseResult @"\mathrm{\left(2+2\right)} + 1"

[<Fact>]
let ``\mathit should be parsed properly`` () =
    verifyParseResult @"\mathit{sin}"


[<Fact>]
let ``\mathcal should be parsed properly`` () =
    verifyParseResult @"\mathcal{sin}"

[<Fact>]
let ``\mathrm{} should throw exn`` () =
    assertParseThrows<TexParseException> @"\mathrm{}"

[<Fact>]
let ``\lim should be parsed properly`` () =
    verifyParseResult @"\lim_{n} x"

[<Fact>]
let ``{\lim} x should be parsed properly`` () =
    verifyParseResult @"{\lim} x"

[<Fact>]
let ``\sin should be parsed properly`` () =
    verifyParseResult @"\sin^{n} x"

[<Fact>]
let ``\int f should be parsed properly`` () =
    verifyParseResult @"\int f"

[<Fact>]
let ``{} should be parsed properly`` () =
    verifyParseResult @"{}"

[<Fact>]
let ``Delimiter with scripts should be parsed properly`` () =
    verifyParseResult @"\left(2+2\right)_a^b"

let ``\text doesn't create any SymbolAtoms``() =
    verifyParseResult @"\text{2+2}"

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
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\colorbox{red}1123");
  InlineData(@"\colorbox{red}{1}123");
  InlineData(@"\colorbox{red} 1123");
  InlineData(@"\colorbox{red} {1}123")>]
let ``\colorbox should parse arguments properly``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\frac2x123");
  InlineData(@"\frac2{x}123");
  InlineData(@"\frac{2}x123");
  InlineData(@"\frac{2}{x}123");
  InlineData(@"\frac 2 x123");
  InlineData(@"\frac2 {x}123");
  InlineData(@"\frac 2{x}123")>]
let ``\frac should parse arguments properly``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\overline1123");
  InlineData(@"\overline{1}123");
  InlineData(@"\overline 1123");
  InlineData(@"\overline {1}123")>]
let ``\overline should parse arguments properly``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\sqrt1123");
  InlineData(@"\sqrt{1}123");
  InlineData(@"\sqrt 1123");
  InlineData(@"\sqrt {1}123")>]
let ``\sqrt should parse arguments properly``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\sqrt [2]1123");
  InlineData(@"\sqrt [ 2]{1}123");
  InlineData(@"\sqrt[2 ] 1123");
  InlineData(@"\sqrt[ 2 ] {1}123")>]
let ``\sqrt should parse optional argument properly``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\underline1123");
  InlineData(@"\underline{1}123");
  InlineData(@"\underline 1123");
  InlineData(@"\underline {1}123")>]
let ``\underline should parse arguments properly``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData("x^y_z");
  InlineData("x^y_{z}");
  InlineData("x^{y}_z");
  InlineData("x^{y}_{z}");
  InlineData("x^y_ z");
  InlineData("x ^ {y} _ {z}")>]
let ``Scripts should be parsed properly``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Theory>]
[<InlineData(@"\text 1123");
  InlineData(@"\text {1}123")>]
let ``\text command should support extended argument parsing``(text : string) : unit =
    verifyParseResultScenario
    <| processSpecialChars text
    <| text

[<Fact>]
let ``{\hat T} should parse successfully``() : unit =
    verifyParseResult @"{\hat T}"

[<Fact>]
let ``integral expression should be parsed properly`` () =
    verifyParseResult @"\int_a^b"
